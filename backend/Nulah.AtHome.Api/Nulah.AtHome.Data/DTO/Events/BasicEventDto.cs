using System.ComponentModel.DataAnnotations;

namespace Nulah.AtHome.Data.DTO.Events;

public class BasicEventDto : DtoBase
{
	public string Description { get; set; } = null!;

	public DateTimeOffset Start { get; set; }

	public DateTimeOffset? End { get; set; }

	public List<TagDto> Tags { get; set; } = new();
}