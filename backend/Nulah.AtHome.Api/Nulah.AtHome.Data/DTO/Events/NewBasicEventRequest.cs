namespace Nulah.AtHome.Data.DTO;

public class NewBasicEventRequest
{
	public string? Description { get; set; }
	public DateTimeOffset? Start { get; set; }
	public DateTimeOffset? End { get; set; }

	public List<string> Tags { get; set; } = new();
}