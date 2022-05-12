using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Common.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Users;

public class UsersRepository : BaseRepository<User>, IUsersRepository
{
    public UsersRepository(DbContext context, DbSet<User> set) : base(context, set) { }
}