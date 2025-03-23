using FluxoCaixa.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Net.Mime;
using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace FluxoCaixa.API.Filters
{
    /// <summary>
    /// Objeto de erros de validação em campos
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FieldError
    {
        /// <summary>
        /// Campo
        /// </summary>
        public string Field { get; set; } = string.Empty;

        /// <summary>
        /// Mensagem de erro
        /// </summary>
        public string Error { get; set; } = string.Empty;
    }

    /// <summary>
    /// Objeto de erro padrão
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="correlationId"></param>
    /// <param name="fieldErrors"></param>
    [ExcludeFromCodeCoverage]
    public class ErrorResult(Exception exception, string correlationId, List<FieldError>? fieldErrors)
    {
        /// <summary>
        /// CorrelationId do fluxo
        /// </summary>
        public string CorrelationId { get; set; } = correlationId;

        /// <summary>
        /// Mensagem de erro
        /// </summary>
        public string Error { get; set; } = exception.Message ?? string.Empty;

        /// <summary>
        /// Detalhes do erro
        /// </summary>
        public string Details { get; set; } = exception.InnerException?.Message ?? string.Empty;

        /// <summary>
        /// Rastreamento
        /// </summary>
        public string StackTrace { get; set; } = exception.StackTrace ?? string.Empty;

        /// <summary>
        /// Campos com erro de validação
        /// </summary>
        public List<FieldError>? FieldErrors { get; set; } = fieldErrors;
    }

    /// <summary>
    /// Filtro de exceção
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class ExceptionFilter(): IExceptionFilter
    {
        /// <summary>
        /// Padroniza retorno de erros
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            var correlationId = context.HttpContext.Request.Headers["X-Correlation-ID"].ToString();

            context.HttpContext.Response.ContentType = MediaTypeNames.Application.Json;

            if (context.Exception is NotFoundException exceptionNotFound)
            {
                context.Result = new NotFoundObjectResult(new ErrorResult(exceptionNotFound, correlationId, null));
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            else if (context.Exception is DuplicateEntryException exceptionExists)
            {
                context.Result = new ObjectResult(new ErrorResult(exceptionExists, correlationId, null));
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
            }
            else if (context.Exception is ValidationException exceptionValidation)
            {
                var fieldErrors = exceptionValidation.Errors.Select(e => new FieldError
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                }).ToList();

                context.Result = new BadRequestObjectResult(new ErrorResult(exceptionValidation, correlationId, fieldErrors));
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else
            {
                var exception = context.Exception;
                context.Result = new ObjectResult(new ErrorResult(exception, correlationId, null));
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }
    }
}
