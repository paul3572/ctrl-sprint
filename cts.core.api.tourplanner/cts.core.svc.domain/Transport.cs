using System.ComponentModel.DataAnnotations;

namespace cts.core.svc.domain;

public class Transport
{
    public Transport(string name, string openRouteProfile)
    {
        this.Name = name;
        this.OpenRouteProfile = openRouteProfile;
    }
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public Transport() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    
    [Key]
    public int TransportId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string OpenRouteProfile { get; set; } = string.Empty;

    private List<Tour> tours = [];
    public IReadOnlyList<Tour> Tours => tours;
}