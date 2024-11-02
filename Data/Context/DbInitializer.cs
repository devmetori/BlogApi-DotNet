using BlogApi.Data.Entity;
using BlogApi.Shared.Enums;

namespace BlogApi.Data.Context;

public class DbInitializer
{
    private static readonly string _defaultPassword =
        "AQAAAAIAAYagAAAAECZlH0Hnc+7AZiSM3eQar4EzrUcuwvkyrHkRJ540l28s5SNWzPKPX7/cKyD+5mHPQA==";

    public static void Initialize(BlogDbContext context)
    {
        Console.WriteLine("Iniciando la inicialización de la base de datos...");
        if (context.Database.EnsureCreated())
        {
            Console.WriteLine("La base de datos ha sido creada exitosamente.");
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var roles = SeedRoles(context);
                var permissions = SeedPermissions(context);
                SeedRolePermissions(context, roles, permissions);
                SeedUsers(context, roles);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Error al inicializar la base de datos: {ex.Message}");
                throw;
            }
        }
    }


    private static Dictionary<RoleName, Guid> SeedRoles(BlogDbContext context)
    {
        if (context.Permissions.Any()) return context.Roles.ToDictionary(r => r.Code, r => r.Id);
        var roles = new List<Role>
        {
            new() { Name = RoleName.SUPERUSUARIO.ToString(), Code = RoleName.SUPERUSUARIO },
            new() { Name = RoleName.ADMIN.ToString(), Code = RoleName.ADMIN },
            new() { Name = RoleName.EDITOR.ToString(), Code = RoleName.EDITOR },
            new() { Name = RoleName.REVIEWER.ToString(), Code = RoleName.REVIEWER },
            new() { Name = RoleName.AUTHOR.ToString(), Code = RoleName.AUTHOR },
            new() { Name = RoleName.READER.ToString(), Code = RoleName.READER }
        };

        context.Roles.AddRange(roles);
        context.SaveChanges();
        return roles.ToDictionary(r => r.Code, r => r.Id);
    }

    private static Dictionary<PermissionName, Guid> SeedPermissions(BlogDbContext context)
    {
        if (context.Permissions.Any())
            return context.Permissions.ToDictionary(
                p => (PermissionName)Enum.Parse(typeof(PermissionName), p.Name, true), p => p.Id);
        var permissions = new List<Permission>
        {
            new()
            {
                Name = PermissionName.CREATE_ARTICLE.ToString(), Description = "Permiso para crear nuevos artículos."
            },
            new()
            {
                Name = PermissionName.EDIT_ARTICLE.ToString(), Description = "Permiso para editar artículos existentes."
            },
            new() { Name = PermissionName.DELETE_ARTICLE.ToString(), Description = "Permiso para eliminar artículos." },
            new()
            {
                Name = PermissionName.APPROVE_ARTICLE.ToString(),
                Description = "Permiso para aprobar artículos antes de su publicación."
            },
            new()
            {
                Name = PermissionName.REVIEW_ARTICLE.ToString(),
                Description = "Permiso para revisar artículos antes de su publicación."
            },
            new() { Name = PermissionName.READ_ARTICLE.ToString(), Description = "Permiso para leer artículos." },
            new()
            {
                Name = PermissionName.PUBLISH_ARTICLE.ToString(), Description = "Permiso para publicar artículos."
            },
            new()
            {
                Name = PermissionName.VIEW_AUDIT_LOGS.ToString(),
                Description = "Permiso para ver los registros de auditoría."
            }
        };

        context.Permissions.AddRange(permissions);
        context.SaveChanges();
        return permissions.ToDictionary(p => (PermissionName)Enum.Parse(typeof(PermissionName), p.Name, true),
            p => p.Id);
    }


    private static void SeedRolePermissions(BlogDbContext context, Dictionary<RoleName, Guid> roles,
        Dictionary<PermissionName, Guid> permissions)
    {
        if (context.RolePermissions.Any()) return;
        List<RolePermission> data = [];
        var relations = GetRelationsRoleAndPermissions();
        foreach (var relation in relations)
        {
            var roleId = roles.FirstOrDefault(r => r.Key == relation.Key).Value;
            foreach (var permName in relation.Value)
            {
                var permissionId = permissions.FirstOrDefault(p => p.Key == permName).Value;
                data.Add(new()
                {
                    RoleId = roleId,
                    PermissionId = permissionId
                });
            }
        }

        if (!data.Any()) return;
        context.RolePermissions.AddRange(data);
        context.SaveChanges();
    }

    private static void SeedUsers(BlogDbContext context, Dictionary<RoleName, Guid> roles)
    {
        if (context.Users.Any()) return;
        var SuperUserRole = roles.FirstOrDefault(r => r.Key == RoleName.SUPERUSUARIO);
        var AdminRole = roles.FirstOrDefault(r => r.Key == RoleName.ADMIN);
        var ReviewerRole = roles.FirstOrDefault(r => r.Key == RoleName.READER);
        var EditorRole = roles.FirstOrDefault(r => r.Key == RoleName.READER);
        var authorRole = roles.FirstOrDefault(r => r.Key == RoleName.READER);
        var ReaderRole = roles.FirstOrDefault(r => r.Key == RoleName.READER);

        var users = new List<User>
        {
            new()
            {
                Name = "Juan",
                Surname = "Pérez",
                Email = "juan.perez@example.com",
                Password = _defaultPassword,
                RoleId = SuperUserRole.Value
            },
            new()
            {
                Name = "Ana",
                Surname = "Gómez",
                Email = "ana.gomez@example.com",
                Password =  _defaultPassword,
                RoleId = AdminRole.Value
            },
            new()
            {
                Name = "Carlos",
                Surname = "Sanchez",
                Email = "carlos.sanchez@example.com",
                Password =  _defaultPassword,
                RoleId = ReviewerRole.Value
            },
            new()
            {
                Name = "Maria",
                Surname = "Lopez",
                Email = "maria.lopez@example.com",
                Password =  _defaultPassword,
                RoleId = EditorRole.Value
            },
            new()
            {
                Name = "Pedro",
                Surname = "Martínez",
                Email = "pedro.martinez@example.com",
                Password =  _defaultPassword,
                RoleId = authorRole.Value
            },
            new()
            {
                Name = "Laura",
                Surname = "Hernández",
                Email = "laura.hernandez@example.com",
                Password =  _defaultPassword,
                RoleId = ReaderRole.Value
            },
            new()
            {
                Name = "Javier",
                Surname = "Castillo",
                Email = "javier.castillo@example.com",
                Password =  _defaultPassword,
                RoleId = ReaderRole.Value
            }
        };

        context.Users.AddRange(users);
        context.SaveChanges();
    }

    private static List<KeyValuePair<RoleName, List<PermissionName>>> GetRelationsRoleAndPermissions()
    {
        return
        [
            new(
                RoleName.SUPERUSUARIO,
                [
                    PermissionName.CREATE_ARTICLE,
                    PermissionName.EDIT_ARTICLE,
                    PermissionName.DELETE_ARTICLE,
                    PermissionName.APPROVE_ARTICLE,
                    PermissionName.REVIEW_ARTICLE,
                    PermissionName.READ_ARTICLE,
                    PermissionName.PUBLISH_ARTICLE,
                    PermissionName.VIEW_AUDIT_LOGS
                ]
            ),

            new(
                RoleName.ADMIN,
                [
                    PermissionName.CREATE_ARTICLE,
                    PermissionName.EDIT_ARTICLE,
                    PermissionName.DELETE_ARTICLE,
                    PermissionName.REVIEW_ARTICLE,
                    PermissionName.APPROVE_ARTICLE,
                    PermissionName.READ_ARTICLE,
                    PermissionName.PUBLISH_ARTICLE
                ]
            ),

            new(
                RoleName.EDITOR,
                new List<PermissionName>
                {
                    PermissionName.EDIT_ARTICLE,
                    PermissionName.APPROVE_ARTICLE,
                    PermissionName.READ_ARTICLE,
                    PermissionName.PUBLISH_ARTICLE
                }
            ),

            new(
                RoleName.REVIEWER,
                new List<PermissionName>
                {
                    PermissionName.REVIEW_ARTICLE,
                    PermissionName.READ_ARTICLE
                }
            ),

            new(
                RoleName.AUTHOR,
                [
                    PermissionName.CREATE_ARTICLE,
                    PermissionName.EDIT_ARTICLE,
                    PermissionName.DELETE_ARTICLE,
                    PermissionName.READ_ARTICLE
                ]
            ),

            new(
                RoleName.READER,
                [PermissionName.READ_ARTICLE]
            )
        ];
    }
}