
using BlogApi.Data.Repository.Interfaces;
using BlogApi.Data.Context;
using BlogApi.Data.Entity;
using BlogApi.Shared.Models;
using MailKit;
using BlogApi.Shared.Enums;


namespace BlogApi.Data.Repository;

public class SessionRepository(BlogDbContext context) : Repository<Session>(context), ISessionRepository
{
    public async Task<Result<Session>> CreateNewSession(Guid userId)
    {

        var result = await FindFirstAsync(m => m.UserId == userId);
        if (!result.IsSuccess) return await AddAsync(new()
        {
            UserId = userId,
            LastAccess = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsActive = true,
            UpdatedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
        });

        Session session = result.Data;
        session.LastAccess = DateTime.UtcNow;
        session.ExpiresAt = DateTime.UtcNow.AddDays(7);
        session.IsActive = true;
        session.UpdatedAt = DateTime.UtcNow;

        var updateResult = await UpdateAsync(session);
        if(!updateResult.IsSuccess) return Result<Session>.Failure(updateResult.Code, updateResult.Message);
        return Result<Session>.Success(session);


    }
}