using Domain.Common;

namespace Domain.Entities;

public class User : Entity
{
    public string DefaultCity { get; set; }
    public string[] FavouriteCities { get; set; }
    public TimeOnly DefaultNotificationTime { get; set; }
}