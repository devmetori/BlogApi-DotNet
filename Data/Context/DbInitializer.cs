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
                var users = SeedUsers(context, roles);
                SeedArticles(context, users);
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

    private static void SeedArticles(BlogDbContext context, List<User> users)
    {
        
        if (context.Articles.Any()) return;
        var author1 = users.FirstOrDefault(u => u.Email == "pedro.martinez@example.com");
        var author2 = users.FirstOrDefault(u => u.Email == "juan.perez@example.com");
        var author3 = users.FirstOrDefault(u => u.Email == "ana.gomez@example.com");
        var articles = new List<Article>
        {
            new()
            {
                Topic = "Tecnología",
                Title = "Introducción a la programación",
                Content = "La programación es una disciplina que se encarga de crear instrucciones que una computadora puede entender y ejecutar. " +
                          "Existen muchos lenguajes de programación, cada uno con sus propias reglas y sintaxis. " +
                          "Algunos de los lenguajes de programación más populares son Python, Java, C#, JavaScript y Ruby.",
                Words = 100,
                Status = ArticleStatus.PUBLISHED.ToString(),
                AuthorId = author1.Id
            },
            new()
            {
                Topic = "Salud",
                Title = "Beneficios de la actividad física",
                Content = "La actividad física es esencial para mantener una buena salud. " +
                          "Algunos de los beneficios de la actividad física son: " +
                          "1. Mejora la salud cardiovascular. " +
                          "2. Ayuda a controlar el peso. " +
                          "3. Fortalece los músculos y los huesos. " +
                            "4. Mejora la salud mental y reduce el estrés. " +
                            "5. Aumenta la energía y la resistencia. " +
                            "Es importante realizar al menos 30 minutos de actividad física moderada al día para mantener una buena salud.",
                Words = 200,
                Status = ArticleStatus.REQUIRES_CHANGES.ToString(),
                AuthorId = author2.Id
            },
            new()
            {
                Topic = "Cultura",
                Title = "Historia de la música",
                Content = "La música es una forma de arte que ha existido desde tiempos inmemoriales. " +
                          "La historia de la música se remonta a la prehistoria, cuando los seres humanos comenzaron a crear sonidos y ritmos con sus voces y con instrumentos rudimentarios. " +
                          "A lo largo de los siglos, la música ha evolucionado y se ha diversificado en una amplia variedad de estilos y géneros. " +
                          "Hoy en día, la música es una parte importante de la cultura de todas las sociedades y desempeña un papel fundamental en la vida de las personas.",
                Words = 300,
                Status = ArticleStatus.DRAFT.ToString(),
                AuthorId = author3.Id
            },
            new()
            {
                Topic = "Deportes",
                Title = "Historia del fútbol",
                Content = "El fútbol es el deporte más popular del mundo y tiene una historia que se remonta a la antigüedad. " +
                          "El fútbol moderno se originó en Inglaterra en el siglo XIX y se ha expandido por todo el mundo, convirtiéndose en un fenómeno global. " +
                          "El fútbol se juega en casi todos los países y tiene una gran cantidad de seguidores y aficionados. " +
                          "La Copa del Mundo de la FIFA es el torneo de fútbol más importante a nivel internacional y se celebra cada cuatro años.",
                Words = 400,
                Status = ArticleStatus.PUBLISHED.ToString(),
                AuthorId = author2.Id
            },
            new()
            {
                Topic = "Tecnología",
                Title = "Inteligencia artificial",
                Content = "La inteligencia artificial es una rama de la informática que se encarga de crear sistemas que pueden realizar tareas que requieren inteligencia humana. " +
                          "Algunas de las aplicaciones de la inteligencia artificial son: " +
                          "1. Reconocimiento de voz. " +
                          "2. Traducción automática. " +
                          "3. Diagnóstico médico. " +
                          "4. Conducción autónoma. " +
                          "5. Juegos de estrategia. " +
                          "La inteligencia artificial está revolucionando muchos campos, como la medicina, la industria, la educación y el entretenimiento.",
                Words = 500,
                Status = ArticleStatus.ARCHIVED.ToString(),
                AuthorId = author3.Id
            },
            new()
            {
                Topic = "Salud",
                Title = "Alimentación saludable",
                Content = "Una alimentación saludable es fundamental para mantener una buena salud y prevenir enfermedades. " +
                          "Algunos consejos para llevar una alimentación saludable son: " +
                          "1. Consumir una variedad de alimentos. " +
                          "2. Incluir frutas y verduras en la dieta. " +
                          "3. Limitar el consumo de alimentos procesados y azucarados. " +
                          "4. Beber suficiente agua. " +
                          "5. Moderar el consumo de sal y grasas. " +
                          "Es importante llevar una alimentación equilibrada y variada para obtener todos los nutrientes necesarios para el buen funcionamiento del organismo.",
                Words = 600,
                Status = ArticleStatus.REVIEWED.ToString(),
                AuthorId = author2.Id
            },
            new()
            {
                Topic = "Cultura",
                Title = "Arte y cultura",
                Content = "El arte y la cultura son elementos fundamentales de la sociedad humana. " +
                          "El arte se define como la expresión de la creatividad y la imaginación, mientras que la cultura se refiere al conjunto de costumbres, tradiciones y valores de una sociedad. " +
                          "El arte y la cultura son formas de expresión que nos permiten comunicarnos, reflexionar y comprender el mundo que nos rodea. " +
                          "El arte y la cultura son parte de nuestra identidad y nos ayudan a construir una sociedad más justa, inclusiva y respetuosa.",
                Words = 700,
                Status = ArticleStatus.APPROVED.ToString(),
                AuthorId = author1.Id
            },
            new()
            {
                Topic = "Deportes",
                Title = "Beneficios del ejercicio físico",
                Content = "El ejercicio físico es esencial para mantener una buena salud y prevenir enfermedades. " +
                          "Algunos de los beneficios del ejercicio físico son: " +
                          "1. Mejora la salud cardiovascular. " +
                          "2. Ayuda a controlar el peso. " +
                          "3. Fortalece los músculos y los huesos. " +
                            "4. Mejora la salud mental y reduce el estrés. " +
                            "5. Aumenta la energía y la resistencia. " +
                            "Es importante realizar al menos 30 minutos de ejercicio físico al día para mantener una buena salud.",
                Words = 800,
                Status = ArticleStatus.WAITING_FOR_REVIEW.ToString(),
                AuthorId = author3.Id
            },
            new()
            {
                Topic = "Tecnología",
                Title = "Internet de las cosas",
                Content = "El Internet de las cosas (IoT) es una tecnología que permite la interconexión de objetos cotidianos a través de internet. " +
                          "El IoT tiene aplicaciones en diversos campos, como la domótica, la salud, la industria y el transporte. " +
                          "Algunos ejemplos de dispositivos IoT son los termostatos inteligentes, las cámaras de seguridad, los relojes inteligentes y los coches autónomos. " +
                          "El IoT está revolucionando la forma en que interactuamos con el mundo y está creando nuevas oportunidades para la innovación y el desarrollo.",
                Words = 900,
                Status = ArticleStatus.DRAFT.ToString(),
                AuthorId = author1.Id
            },
            new()
            {
                Topic = "Salud",
                Title = "Importancia del descanso",
                Content = "El descanso es fundamental para mantener una buena salud y prevenir enfermedades. " +
                          "Algunos de los beneficios del descanso son: " +
                          "1. Mejora la concentración y la memoria. " +
                          "2. Reduce el estrés y la ansiedad. " +
                          "3. Fortalece el sistema inmunológico. " +
                          "4. Ayuda a recuperarse después del ejercicio físico. " +
                          "5. Mejora el estado de ánimo y la calidad de vida. " +
                          "Es importante dormir al menos 7-8 horas al día para garantizar un buen descanso y una buena salud.",
                Words = 1000,
                Status = ArticleStatus.PUBLISHED.ToString(),
                AuthorId = author2.Id
            },
        };

        context.Articles.AddRange(articles);
        context.SaveChanges();
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

    private static List<User> SeedUsers(BlogDbContext context, Dictionary<RoleName, Guid> roles)
    {
        if (context.Users.Any()) return context.Users.ToList();
        var SuperUserRole = roles.FirstOrDefault(r => r.Key == RoleName.SUPERUSUARIO);
        var AdminRole = roles.FirstOrDefault(r => r.Key == RoleName.ADMIN);
        var ReviewerRole = roles.FirstOrDefault(r => r.Key == RoleName.REVIEWER);
        var EditorRole = roles.FirstOrDefault(r => r.Key == RoleName.EDITOR);
        var authorRole = roles.FirstOrDefault(r => r.Key == RoleName.AUTHOR);
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
        return users;
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