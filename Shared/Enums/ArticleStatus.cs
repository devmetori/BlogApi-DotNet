namespace BlogApi.Shared.Enums;

public enum ArticleStatus
{
    DRAFT,
    WAITING_FOR_REVIEW,
    REVIEWED,
    REJECTED_BY_REVIEWER,
    PUBLISHED,
    APPROVED,
    REQUIRES_CHANGES,
    REJECTED_BY_EDITOR,
    ARCHIVED
}