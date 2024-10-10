using BlogApi.Models;
using BlogApi.Repository.Interfaces;

namespace BlogApi.Repository;

public class RoleRepository(BlogDbContext context) : Repository<Role>(context), IRoleRepository;