using System.ComponentModel.DataAnnotations;

namespace Nulah.AtHome.Data.Models.Events;

internal class BasicEvent : BaseEntity
{
	[Required]
	public string Description { get; set; } = null!;

	[Required]
	public DateTimeOffset Start { get; set; }

	public DateTimeOffset? End { get; set; }

	public virtual List<Tag> Tags { get; set; } = new();
}