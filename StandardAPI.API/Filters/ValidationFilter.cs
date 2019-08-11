using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace StandardAPI.API.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errorsInModelState = context.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(x => x.ErrorMessage)).ToArray();

                 var errors = new List<ErrorModel>();

                foreach (var error in errorsInModelState)
                {
                    foreach (var subError in error.Value)
                    {
                        var errorModel = new ErrorModel
                        {
                            Field = error.Key,
                            Message = subError
                        };

                        errors.Add(errorModel);
                    }
                }

                context.Result = new JsonResult(errors)
                {
                    StatusCode = (int)HttpStatusCode.UnprocessableEntity
                };


                return;
            }

            await next();
        }

        
    }

    internal class ErrorModel
    {
        public string Field { get; set; }

        public string Message { get; set; }
    }

}
