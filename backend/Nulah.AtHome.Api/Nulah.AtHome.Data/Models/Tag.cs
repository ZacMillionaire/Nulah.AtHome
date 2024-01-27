using System.ComponentModel.DataAnnotations;

namespace Nulah.AtHome.Data.Models;

internal class Tag : BaseEntity
{
	[Required]
	public string Name { get; set; } = null!;
}