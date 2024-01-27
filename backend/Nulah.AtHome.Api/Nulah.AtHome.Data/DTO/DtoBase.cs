namespace Nulah.AtHome.Data.DTO;

public abstract class DtoBase
{
	public int Id { get; set; }
	public uint Version { get; set; }
	public DateTime CreatedUtc { get; internal set; }
	public DateTime UpdatedUtc { get; internal set; }
}