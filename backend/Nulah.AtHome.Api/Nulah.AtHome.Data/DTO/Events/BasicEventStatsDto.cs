namespace Nulah.AtHome.Data.DTO.Events;

public class BasicEventStatsDto
{
	public int Total { get; set; }
	public int WithEndDate { get; set; }
	public int WithoutEndDate { get; set; }
	public int WithTags { get; set; }
	public int WithoutTags { get; set; }
}