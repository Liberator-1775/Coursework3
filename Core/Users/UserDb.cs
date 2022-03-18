namespace Core.Users;

public class UserDb
{
    public long Id { get; set; }
    public string DefaultCity { get; set; }
    public string[] FavouriteCities { get; set; }
    public TimeOnly DefaultNotificationTime { get; set; }
}