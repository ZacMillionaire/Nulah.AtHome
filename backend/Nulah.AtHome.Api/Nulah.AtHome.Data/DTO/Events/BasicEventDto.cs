namespace Nulah.AtHome.Data.DTO;

public class BasicEventDto : DtoBase
{
	public string Description { get; set; } = null!;

	public DateTime Start { get; set; }

	public DateTime? End { get; set; }

	public List<TagDto> Tags { get; set; } = new();
}