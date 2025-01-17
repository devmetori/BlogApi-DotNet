﻿using BlogApi.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using BlogApi.Shared.Enums;

namespace BlogApi.Api.Attributes;

public class IsAuthenticatedAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var authorizationHeader = context.HttpContext.Request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            context.Result = UnauthorizedResponse(ERROR_CODE.UNAUTHORIZED,
                "Parece que no tienes un sesion activa en este momento. Inicia session para poder aceder este recurso.",
                context.HttpContext.Request.Cookies["X-Tracker-ID"]
            );
            return;
        }

        var jwtService = GetJwtService(context);
        
        var token = authorizationHeader.Substring("Bearer ".Length).Trim();
        var result = jwtService.IsValidToken(token);
        if (!result.IsValid)
        {
            context.Result = UnauthorizedResponse(ERROR_CODE.UNAUTHORIZED,
                "La sesión ha expirado, intenta iniciar de nuevo",
                context.HttpContext.Request.Cookies["X-Tracker-ID"]);
            return;
        }

        context.HttpContext.User = result.Claims;
    }

    private IJwtService GetJwtService(AuthorizationFilterContext context)
    {
        var jwtService = context.HttpContext.RequestServices.GetService<IJwtService>();
        if (jwtService is null) throw new NullReferenceException("IJwtService is null");
        return jwtService;
    }

    private JsonResult UnauthorizedResponse(ERROR_CODE code, string message, string requestId)
    {
        return new JsonResult(new
        {
            success = false,
            error = new
            {
                code,
                message,
                requestId,
            }
        }) { StatusCode = 401 };
    }
}