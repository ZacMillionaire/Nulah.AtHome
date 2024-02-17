using System.ComponentModel.DataAnnotations;
using Nulah.AtHome.Data.Models.Events;

namespace Nulah.AtHome.Data.Models;

internal class Tag : BaseEntity
{
	[Required]
	public string Name { get; set; } = null!;

	public virtual List<BasicEvent> Events { get; set; } = new();
}