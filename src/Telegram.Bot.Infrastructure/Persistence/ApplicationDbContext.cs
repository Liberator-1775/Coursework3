using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Domain.Entities;

namespace Telegram.Bot.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions options) : base(options) { }
}