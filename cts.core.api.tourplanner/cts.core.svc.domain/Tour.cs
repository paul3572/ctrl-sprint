using System.ComponentModel.DataAnnotations;

namespace cts.core.svc.domain;

public class Tour
{
    public Tour(User user, Transport transport, int tourDistanceKm, int estimatedTimeMinutes, int rating)
    {
        this.User = user;
        this.Transport = transport;
        this.TourDistanceKm = tourDistanceKm;
        this.EstimatedTimeMinutes = estimatedTimeMinutes;
        this.Rating = rating;
    }
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public Tour() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    
    [Key]
    public int TourId { get; private set; }
    public Guid TourGuid { get;  private set; }
    public int UserId { get; private set; }
    public virtual User User { get; private set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string From  { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public int TransportId { get; set; }
    public virtual Transport Transport { get; set; }
    public int TourDistanceKm { get; set; }
    public int EstimatedTimeMinutes { get; set; }
    public int Rating { get; set; }

    private List<TourLog> tourLogs = [];
    public virtual IReadOnlyList<TourLog> TourLogs => tourLogs;
}