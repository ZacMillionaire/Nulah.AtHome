using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Nulah.AtHome.Data.Converters;

public class DateTimeOffsetConverter : ValueConverter<DateTimeOffset, DateTimeOffset>
{
	public DateTimeOffsetConverter()
		: base(
			d => d.ToUniversalTime(),
			d => d.ToUniversalTime())
	{
	}
}