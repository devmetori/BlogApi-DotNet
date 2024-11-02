using System.Security.Claims;
using BlogApi.Data.Entity;
using BlogApi.Shared.DTOs;
using BlogApi.Shared.DTOs.Auth;
using BlogApi.Shared.Models.Auth;


namespace BlogApi.Shared.Utils;

public static class ObjectMapper
{
    public static Article ToEntity(this ArticleDto articleDto)
    {
        return new Article
        {
            Topic = articleDto.Topic,
            Title = articleDto.Title,
            Content = articleDto.Content,
            Words = articleDto.Content.Length,
        };
    }


    public static Article MapToArticleEntity(ArticleDto articleDto)
    {
        return new Article
        {
            Topic = articleDto.Topic,
            Title = articleDto.Title,
            Content = articleDto.Content,
            Words = articleDto.Content.Length,
        };
    }

    public static UserDto MapToDto(this User model, JwtDto tokens)
    {
        return new UserDto
        {
            Id = model.Id,
            Name = model.Name,
            Surname = model.Surname,
            Email = model.Email,
            Tokens = tokens
        };
    }

    public static UserDto MapToDto(this User model)
    {
        return new UserDto
        {
            Id = model.Id,
            Name = model.Name,
            Surname = model.Surname,
            Email = model.Email
        };
    }

    public static JwtPayload MapClaimsToPayload(this ClaimsPrincipal claimsPrincipal)
    {
        return new()
        {
            Id = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            Role = int.Parse(claimsPrincipal.FindFirst(ClaimTypes.Role)?.Value),
            SessionId = claimsPrincipal.FindFirst(t => t.Type == "sessionId")?.Value
        };
    }
}