using System.Linq.Expressions;
using BlogApi.Data.Context;
using BlogApi.Data.Entity;
using BlogApi.Data.Repository.Interfaces;
using BlogApi.Shared.Enums;
using BlogApi.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Data.Repository;

public class UserRepository(BlogDbContext context) : Repository<User>(context), IUserRepository
{
    public async Task<Result<User>> FindUserWithRoleAsync(Expression<Func<User, bool>> predicate)
    {
        try
        {
            var result = await context.Users.Join(context.Roles, u => u.RoleId, r => r.Id,
                (user, role) => new User
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    Password = user.Password,
                    PasswordToken = user.PasswordToken,
                    AccountVerified = user.AccountVerified,
                    Verified2fa = user.Verified2fa,
                    Enabled2fa = user.Enabled2fa,
                    Secret2fa = user.Secret2fa,
                    Code2fa = user.Code2fa,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    RoleId = user.RoleId,
                    Role = role,
                }).FirstOrDefaultAsync(predicate);

            return result == null
                ? Result<User>.Failure(ERROR_CODE.RECORD_NOT_FOUND, "Registro no encontrado")
                : Result<User>.Success(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Result<User>.Failure(ERROR_CODE.UNKNOWN_ERROR, "Error al obtener el registro");
        }
    }

  




    public async Task<Result<User>> FindUserWithSessionAndRoleAsync(Expression<Func<User, bool>> predicate)
    {
        try
        {
            var result = await context.Users
                .Join(context.Roles, u => u.RoleId, r => r.Id, (user, role) => new { user, role })
                .Join(context.Sessions, ur => ur.user.Id, s => s.UserId, (ur, session) => new User
                {
                    Id = ur.user.Id,
                    Name = ur.user.Name,
                    Surname = ur.user.Surname,
                    Email = ur.user.Email,
                    Password = ur.user.Password,
                    PasswordToken = ur.user.PasswordToken,
                    AccountVerified = ur.user.AccountVerified,
                    Verified2fa = ur.user.Verified2fa,
                    Enabled2fa = ur.user.Enabled2fa,
                    Secret2fa = ur.user.Secret2fa,
                    Code2fa = ur.user.Code2fa,
                    CreatedAt = ur.user.CreatedAt,
                    UpdatedAt = ur.user.UpdatedAt,
                    RoleId = ur.user.RoleId,
                    Role = ur.role,
                    Session = session
                }).FirstOrDefaultAsync(predicate);
            return result is null
                ? Result<User>.Failure(ERROR_CODE.RECORD_NOT_FOUND, "Registro no encontrado")
                : Result<User>.Success(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Result<User>.Failure(ERROR_CODE.UNKNOWN_ERROR, "Error al obtener el registro");
        }
    }

    public async Task<Result<List<Permission>>> GetUserPermissionsByIdAsync(string userId)
    {
        try
        {
            

            var result = await context.Users
                .Where(u => u.Id == Guid.Parse(userId))
                .SelectMany(u => u.Role.Authorizations)
                .Select(rp => rp.Permission)
                .ToListAsync();
            if (result is null)
            {
                return Result<List<Permission>>.Failure(ERROR_CODE.RECORD_NOT_FOUND,
                    "No se ha encontrado los permisos del usuario");
            }

            return Result<List<Permission>>.Success(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Result<List<Permission>>.Failure(ERROR_CODE.UNKNOWN_ERROR, "Hello world");
        }
    }
}