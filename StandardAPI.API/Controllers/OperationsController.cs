using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;


namespace StandardAPI.API.Controllers
{

    public class OperationsController : BaseController
    {
        private readonly IConfiguration _config;

        public OperationsController(IConfiguration config)
        {
            _config = config;
        }

        [HttpOptions("reloadconfig")]
        public IActionResult ReloadConfig()
        {
            try
            {
                var root = (IConfigurationRoot)_config;
                root.Reload();
                return Ok();
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
