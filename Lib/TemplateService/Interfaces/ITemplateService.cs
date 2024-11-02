namespace BlogApi.Lib.TemplateEngine.Interfaces;

public interface ITemplateService
{
    Task<string> GetTemplateAsync<TModel>(string viewName, TModel model);

}