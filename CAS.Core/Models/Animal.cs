namespace CAS.Core.Models;

public class Animal
{
    public required string Id { get; set; }
    public DateTimeOffset Dob { get; set; }
    public DateTimeOffset? Dod { get; set; }
}
