namespace Nulah.AtHome.Data.DTO.Events;

public class UpdateBasicEventRequest : BasicEventRequest
{
	public int Id { get; set; }
	public uint Version { get; set; }
}