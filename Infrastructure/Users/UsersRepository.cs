using Core.Base;
using Core.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Users;

public class UsersRepository : BaseRepository<UserDb>, IUsersRepository
{
    public UsersRepository(DbContext context, DbSet<UserDb> set) : base(context, set) { }
}