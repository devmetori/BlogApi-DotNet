
using BlogApi.Data.Context;
using BlogApi.Data.Entity;
using BlogApi.Data.Repository.Interfaces;

namespace BlogApi.Data.Repository;

public class RoleRepository(BlogDbContext context) : Repository<Role>(context), IRoleRepository;