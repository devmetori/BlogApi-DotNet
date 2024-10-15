namespace BlogApi.Shared.DTOs;
public record ArticleDto(
    string Topic,
    string Title,
    string Content
);