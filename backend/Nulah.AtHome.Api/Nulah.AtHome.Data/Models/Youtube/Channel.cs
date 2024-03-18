using System.ComponentModel.DataAnnotations;

namespace Nulah.AtHome.Data.Models.Youtube;

internal class Channel : BaseEntity
{
	[Required]
	public string Name { get; set; } = null!;

	[Required]
	public byte[] ThumbnailBlob { get; set; } = null!;
}