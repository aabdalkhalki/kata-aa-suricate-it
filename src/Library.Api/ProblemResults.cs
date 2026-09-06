using Library.Domain.Results;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api;

public static class ProblemResults
{
    extension(ControllerBase controller)
    {
        public ObjectResult ProblemFor(Error error)
        {
            var status = error.Kind switch
            {
                ErrorKind.NotFound => StatusCodes.Status404NotFound,
                ErrorKind.Conflict => StatusCodes.Status409Conflict,
                _ => throw new ArgumentOutOfRangeException(nameof(error), error.Kind, "Unknown error kind."),
            };

            var problemDetails = controller.ProblemDetailsFactory.CreateProblemDetails(
                controller.HttpContext,
                statusCode: status,
                title: error.Message);

            problemDetails.Extensions["code"] = error.Code;

            return new ObjectResult(problemDetails) { StatusCode = status };
        }
    }
}
