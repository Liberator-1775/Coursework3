using Telegram.Bot.Application.Common.Interfaces;
using Telegram.Bot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Infrastructure.Common.Repositories;

namespace Telegram.Bot.Infrastructure.Users;

public class UsersRepository : BaseRepository<User>, IUsersRepository
{
    public UsersRepository(DbContext context, DbSet<User> set) : base(context, set) { }
}