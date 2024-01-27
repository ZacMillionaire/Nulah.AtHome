using System.ComponentModel.DataAnnotations;

namespace Nulah.AtHome.Data.Models;

internal class BaseEntity
{
	[Key]
	public int Id { get; set; }

	//https://www.npgsql.org/efcore/modeling/concurrency.html?tabs=data-annotations
	[Timestamp]
	public uint Version { get; set; }

	public DateTime CreatedUtc { get; internal set; }
	public DateTime UpdatedUtc { get; internal set; }
}