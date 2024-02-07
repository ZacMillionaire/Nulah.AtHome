namespace Nulah.AtHome.Data.DTO.Events;

/// <summary>
/// Base class for event CRUD methods and validations
/// </summary>
public abstract class BasicEventRequest
{
	public string? Description { get; set; }
	public DateTimeOffset? Start { get; set; }
	public DateTimeOffset? End { get; set; }

	public List<string>? Tags { get; set; } = new();
}