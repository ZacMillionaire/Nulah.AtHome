using System.ComponentModel.DataAnnotations;

namespace Nulah.AtHome.Data.Models.Events;

internal class BasicEvent : BaseEntity
{
	[Required]
	public string Description { get; set; } = null!;

	[Required]
	public DateTime Start { get; set; }

	public DateTime? End { get; set; }

	public virtual List<Tag> Tags { get; set; } = new();
}