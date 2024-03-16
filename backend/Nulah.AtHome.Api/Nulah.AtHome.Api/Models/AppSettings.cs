namespace Nulah.AtHome.Api.Models;

public class AppSettings
{
	public Logging Logging { get; set; }
	public ConnectionStrings ConnectionStrings { get; set; }
}

public class Logging
{
	public Seq Seq { get; set; }
}

public class Seq
{
	public string ApiKey { get; set; }
}

public class ConnectionStrings
{
	public string Postgres { get; set; }
	public string Seq { get; set; }
}