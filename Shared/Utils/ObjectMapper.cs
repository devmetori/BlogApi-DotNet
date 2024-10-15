using BlogApi.Data.Entity;
using BlogApi.Shared.DTOs;


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
}