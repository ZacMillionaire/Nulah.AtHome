using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Nulah.AtHome.Data.Converters;

/// <summary>
/// This is used for Postgres to make sure entity framework creates the correct data types for postgres
/// https://github.com/npgsql/npgsql/issues/4176#issuecomment-1064250712
/// </summary>
public class DateTimeOffsetConverter : ValueConverter<DateTimeOffset, DateTimeOffset>
{
	public DateTimeOffsetConverter()
		: base(
			d => d.ToUniversalTime(),
			d => d.ToUniversalTime())
	{
	}
}