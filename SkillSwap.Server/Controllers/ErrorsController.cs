using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace SkillSwap.Server.Controllers;

[Route("[controller]")]
[ApiController]
public class ErrorsController : ControllerBase
{
    [Route("/error")]
    public IActionResult HandleError()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = context?.Error;

        if (exception == null)
            return Problem("An unknown error occurred.");

        var statusCode = exception switch
        {
            BadHttpRequestException => (int)HttpStatusCode.BadRequest,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            ConflictException => (int)HttpStatusCode.Conflict,
            _ => (int)HttpStatusCode.InternalServerError
        };

        return Problem(detail: exception.Message, statusCode: statusCode);
    }
}

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}
