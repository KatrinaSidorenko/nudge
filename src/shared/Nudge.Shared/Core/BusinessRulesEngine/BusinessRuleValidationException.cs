namespace Nudge.Shared.Core.BusinessRulesEngine;

using Nudge.Shared.Core.Exception;
using System.Net;

public class BusinessRuleValidationException : CustomException
{
    public BusinessRuleValidationException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            : base(message, statusCode)
    {
    }

    public BusinessRuleValidationException(
        string message,
        System.Exception innerException,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        int? code = null)
        : base(message, innerException, statusCode, code)
    {
    }
}
