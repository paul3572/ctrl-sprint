using System.ComponentModel.DataAnnotations;

namespace cts.core.svc.contracts;

public class Transport
{
    public Transport(string name)
    {
        this.Name = name;
    }
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public Transport() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    
    [Key]
    public int TransportId { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private List<Tour> tours;
    public IReadOnlyList<Tour> Tours => tours;
}