using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using StandardAPI.Application.Exceptions;
using System;
using System.Net;

namespace StandardAPI.API.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            context.HttpContext.Response.ContentType = "application/json";

            if (context.Exception is NotFoundException)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Result = new JsonResult(new
                {
                    error = new[] { context.Exception.Message }
                });

                return;
            }

            if (context.Exception is BadRequestException)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Result = new JsonResult(new
                {
                    error = new[] { context.Exception.Message }
                });

                return;
            }

            var code = HttpStatusCode.InternalServerError;
            var Id = Guid.NewGuid().ToString();
            var error = new ApiError
            {
                Id = Id,
                Status = (short)code,
                Title =  "Some kind of error occurred in the API.  Please use the id and contact our support team if the problem persists."
            };

            Log.Error(context.Exception, "An exception was caught in the API request pipeline. {CorrelationId}{HttpContext}", Id, context);
   
            context.HttpContext.Response.StatusCode = (int)code;
            context.Result = new JsonResult(error);
        }
    }

    public class ApiError
    {
        public string Id { get; set; }
        public short Status { get; set; }     
        public string Title { get; set; }
    }

}

