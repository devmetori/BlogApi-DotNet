using BlogApi.Shared.Enums;

namespace BlogApi.Shared.DTOs;

public record ApprovalDto(string comment, ArticleStatus status);