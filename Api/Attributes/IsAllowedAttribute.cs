using BlogApi.Data.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BlogApi.Shared.Enums;
using BlogApi.Api.Services.Interfaces;


namespace BlogApi.Api.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class IsAllowedAttribute : Attribute, IAuthorizationFilter
{
    private readonly RoleName[] _requiredRole;
    private readonly PermissionName _requiredPermissions;

    public IsAllowedAttribute(RoleName[] requiredRole, PermissionName requiredPermissions)
    {
        _requiredRole = requiredRole;
        _requiredPermissions = requiredPermissions;
    }
    public IsAllowedAttribute(RoleName[] requiredRole)
    {
        _requiredRole = requiredRole;
    }


    public async void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity.IsAuthenticated)
        {
            context.Result = HttpResponse(ERROR_CODE.UNAUTHORIZED,
                "Parece que no tienes un sesion activa en este momento. Inicia sesión para poder acceder este recurso.");
            return;
        }


        var userRole = (RoleName)int.Parse(user.FindFirst(ClaimTypes.Role)?.Value);

        if (_requiredRole.Length > 0 && _requiredRole.All(r => r != userRole))
        {
            context.Result = HttpResponse(ERROR_CODE.FORBIDDEN,
                "No tienes los permisos suficientes para acceder a este recurso.");
            return;
        }



        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
        {
            context.Result = HttpResponse(ERROR_CODE.UNAUTHORIZED,
                "No se ha podido identificar al usuario.");
            return;
        }

        using var scope = context.HttpContext.RequestServices.CreateScope();
        var authServices = scope.ServiceProvider.GetRequiredService<IAuthService>();
        var result = await authServices.GetUserPermissionsByIdAsync(userId);
        if (!result.IsSuccess)
        {
            context.Result = HttpResponse(ERROR_CODE.FORBIDDEN,
                "No tienes los permisos suficientes para acceder a este recurso.");
            return;
        }

        var permissions = result.Data.Select(p => Enum.Parse<PermissionName>(p.Name));
        var currentPermissions = permissions.FirstOrDefault(p => p == _requiredPermissions);
        if (currentPermissions != null)
        {
            var newClaims = new List<Claim> { new Claim("permission", currentPermissions.ToString()), };
            var newIdentity = new ClaimsIdentity(user.Claims.Concat(newClaims), user.Identity.AuthenticationType);
            context.HttpContext.User = new ClaimsPrincipal(newIdentity); ;
            return;
        };

        context.Result = HttpResponse(ERROR_CODE.FORBIDDEN, "No tienes los permisos suficientes para acceder a este recurso.");
    }

    private JsonResult HttpResponse(ERROR_CODE code, string message)
    {
        return code switch
        {
            ERROR_CODE.UNAUTHORIZED => new JsonResult(new
            {
                success = false,
                error = new
                {
                    code,
                    message
                }
            })
            { StatusCode = 401 },
            ERROR_CODE.FORBIDDEN => new JsonResult(new
            {
                success = false,
                error = new
                {
                    code,
                    message
                }
            })
            { StatusCode = 403 },
            _ => new JsonResult(new
            {
                success = false,
                error = new
                {
                    code,
                    message
                }
            })
            { StatusCode = 401 }
        };
    }
}