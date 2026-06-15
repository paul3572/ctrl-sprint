using System.ComponentModel.DataAnnotations;

namespace cts.core.svc.contracts;

public class TourLog
{
    public TourLog(Tour tour, DateTime timestamp, string comment, int difficulty, int totalDistanceKm, int totalTimeMin, int rating)
    {
        Tour = tour;
        Timestamp = timestamp;
        Comment = comment;
        Difficulty = difficulty;
        TotalDistanceKm = totalDistanceKm;
        TotalTimeMin = totalTimeMin;
        Rating = rating;
    }
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public TourLog() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    [Key]
    public int TourLogId { get; private set; }
    public Guid TourLogGuid { get; private set; }
    public int TourId { get; set; }
    public virtual Tour Tour { get; set; }
    public DateTime Timestamp { get; set; }
    public string Comment { get; set; }
    public int Difficulty { get; set; }    
    public int TotalDistanceKm { get; set; }
    public int TotalTimeMin { get; set; }
    public int Rating { get; set; }
}