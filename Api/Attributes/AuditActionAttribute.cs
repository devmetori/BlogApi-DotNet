using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using BlogApi.Api.Services.Interfaces;
using BlogApi.Shared.Enums;
using BlogApi.Shared.Models;
using BlogApi.Shared.Models.Article;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlogApi.Api.Attributes;

public class AuditActionAttribute : Attribute, IAsyncActionFilter

{

    private readonly ILogger<AuditActionAttribute> _logger;
    private readonly PermissionName _requiredPermission;


    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var requestId = context.HttpContext.Request.Cookies["X-Tracker-ID"];
        var user = context.HttpContext.User;
        if (user == null || !user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedObjectResult(new { success = false, error = new { message = "No hay datos en la sesión del usuario.", requestId } });
            return;
        }
        if (!user.HasClaim(c => c.Type == "permission"))
        {

            await next();
        }


        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userPermission = user.FindFirst(m => m.Type == "permission")?.Value;
        var hasPermission = userPermission is null ? false : Enum.IsDefined(typeof(PermissionName), userPermission);
        var articleId = context.ActionArguments.ContainsKey("id") ? context.ActionArguments["id"]?.ToString() : null;

        var resultContext = await next();

        using var scope = context.HttpContext.RequestServices.CreateScope();
        var articleService = scope.ServiceProvider.GetService<IArticleService>();
        var payload = new AuditPayload(
           userPermission ?? string.Empty,
           hasPermission,
          false,
          string.Empty,
           Guid.Parse(articleId),
           Guid.Parse(userId)
       );

        var responsePayload = CreateAuditPayload(resultContext, payload);
        if (responsePayload is null) return;

        var auditResult = await articleService.AuditActionAsync(responsePayload);

        if (!auditResult.IsSuccess)
        {
            Console.WriteLine("Error al guardar la auditoría");
        }


    }

    public AuditPayload CreateAuditPayload(ActionExecutedContext context, AuditPayload ResponsePayload)
    {
        try
        {
            if (context.Result is ObjectResult objectResult && objectResult.Value != null)
            {
                var result = JsonSerializer.Serialize(objectResult.Value);
                var response = JsonSerializer.Deserialize<Dictionary<string, object>>(result);

                if (response is null || !response.TryGetValue("Success", out var successValue)) return null;
                bool isSuccess = Convert.ToBoolean(successValue.ToString());
                if (isSuccess)
                {
                    var payload = JsonSerializer.Deserialize<SuccessResponse<object>>(result);

                    if (payload is null) return null;

                    ResponsePayload.Success = true;
                    ResponsePayload.Message = payload.Message;
                    return ResponsePayload;
                }
                else
                {
                    var payload = JsonSerializer.Deserialize<ErrorResponse>(result);

                    if (payload is null) return null;

                    ResponsePayload.Success = false;
                    ResponsePayload.Message = payload.Error.Message;

                    return ResponsePayload;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error al crear el payload de auditoría");
            return null;
        }

        return null;
    }


}
public class SuccessResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
}

public class ErrorResponse
{
    public bool Success { get; set; }
    public ErrorDetails Error { get; set; }
}

public class ErrorDetails
{
    public string Code { get; set; }
    public string Message { get; set; }
    public string RequestId { get; set; }
}