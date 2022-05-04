namespace Core.Users;

public class User
{
    public long Id { get; set; }
    public string DefaultCity { get; set; }
    public string[] FavouriteCities { get; set; }
    public TimeOnly DefaultNotificationTime { get; set; }
}