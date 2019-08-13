using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StandardAPI.Application.Exceptions;
using StandardAPI.Application.Interfaces.Security;
using StandardAPI.Application.Models.Security;
using StandardAPI.Peristence.Contexts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StandardAPI.API.SecurityService
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly SecurityDbContext _context;

        public IdentityService(UserManager<IdentityUser> userManager, TokenValidationParameters tokenValidationParameters, SecurityDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _tokenValidationParameters = tokenValidationParameters;
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<AuthResult> RegisterAsync(RegistrationRequest register)
        {
            var existingUser = await _userManager.FindByEmailAsync(register.Email);

            if (existingUser != null)
            {
                throw new BadRequestException("User with this email address already exists");
            }

            var newUserId = Guid.NewGuid();
            var newUser = new IdentityUser
            {
                Id = newUserId.ToString(),
                Email = register.Email,
                UserName = register.Email
            };

            var res = await _userManager.CreateAsync(newUser, register.Password);

            if (!res.Succeeded)
            {
                throw new BadRequestException(string.Join(".", res.Errors.Select(x => x.Description).ToArray()));
            }

            return await GenerateAuthResultForUserAsync(newUser);
        }

        public async Task<AuthResult> LoginAsync(LoginRequest login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email);

            if (user == null)
            {
                throw new BadRequestException("Username/Password is invalid");
            }

            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, login.Password);

            if (!userHasValidPassword)
            {
                throw new BadRequestException("Username/Password is invalid");
            }

            return await GenerateAuthResultForUserAsync(user);
        }

        public async Task<AuthResult> RefreshTokenAsync(RefreshTokenRequest refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(refreshToken.Token);

            if (validatedToken == null)
            {
                throw new BadRequestException("Invalid Token");
            }

            var expiryDateUnix =
                long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                throw new BadRequestException("This token hasn't expired yet");
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken.RefreshToken);

            if (storedRefreshToken == null)
            {
                throw new BadRequestException("This refresh token does not exist");
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                throw new BadRequestException("This refresh token has expired");
            }

            if (storedRefreshToken.Invalidated)
            {
                throw new BadRequestException("This refresh token has been invalidated");
            }

            if (storedRefreshToken.Used)
            {
                throw new BadRequestException("This refresh token has been used");
            }

            if (storedRefreshToken.JwtId != jti)
            {
                throw new BadRequestException("This refresh token does not match this JWT");
            }

            storedRefreshToken.Used = true;
            _context.RefreshTokens.Update(storedRefreshToken);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
            return await GenerateAuthResultForUserAsync(user);
        }


        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task<AuthResult> GenerateAuthResultForUserAsync(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = Common.Utility.AppConfiguration().GetSection("JwtSettings").GetSection("Secret").Value;
            var tokenLifetime = TimeSpan.Parse(Common.Utility.AppConfiguration().GetSection("JwtSettings").GetSection("TokenLifetime").Value);

            var key = Encoding.ASCII.GetBytes(secret);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("id", user.Id)
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role == null) continue;
                var roleClaims = await _roleManager.GetClaimsAsync(role);

                foreach (var roleClaim in roleClaims)
                {
                    if (claims.Contains(roleClaim))
                        continue;

                    claims.Add(roleClaim);
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(tokenLifetime),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthResult
            {
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token
            };
        }
    }
}
