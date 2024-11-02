namespace BlogApi.Shared.Models.Email;

public class MailPayload
{
    public string ToEmail { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public List<IFormFile> Attachments { get; set; }
    
    public TemplateModel Model { get; set; }
}