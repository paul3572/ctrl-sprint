using System.ComponentModel.DataAnnotations;

namespace cts.core.svc.domain;

public class Tour
{
    public Tour(User user, Transport transport, double tourDistanceInMeters, int estimatedTimeMinutes, int rating)
    {
        this.User = user;
        this.Transport = transport;
        this.TourDistanceInMeters = tourDistanceInMeters;
        this.EstimatedTimeMinutes = estimatedTimeMinutes;
        this.Rating = rating;
    }
    
    public Tour(User user, string name, string description, string from, string to, Transport transport, double tourDistanceInMeters, int estimatedTimeMinutes, int rating)
    {
        this.User = user;
        this.Name = name;
        this.Description = description;
        this.From = from;
        this.To = to;
        this.Transport = transport;
        this.TourDistanceInMeters = tourDistanceInMeters;
        this.EstimatedTimeMinutes = estimatedTimeMinutes;
        this.Rating = rating;
    }
    
    public Tour()
    {
        this.User = new User("init@test.com", "init", DateTime.Now);
        this.Transport = new Transport("init", "init");
    }
    
    [Key]
    public int TourId { get; private set; }
    public Guid TourGuid { get;  private set; }
    public int UserId { get; private set; }
    public virtual User User { get; private set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string From  { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public int TransportId { get; init; }
    public virtual Transport Transport { get; set; }
    public double TourDistanceInMeters { get; set; }
    public int EstimatedTimeMinutes { get; set; }
    public int Rating { get; set; }

    private List<TourLog> tourLogs = [];
    public virtual IReadOnlyList<TourLog> TourLogs => tourLogs;
}