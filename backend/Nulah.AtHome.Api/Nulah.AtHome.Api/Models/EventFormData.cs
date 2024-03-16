using System.ComponentModel.DataAnnotations;

namespace Nulah.AtHome.Api.Models;

public class EventFormData : FormDataBase
{
	[Required]
	public int Id { get; set; }

	[Required]
	public uint Version { get; set; }

	[Required]
	public string Description { get; set; }

	[Required]
	public DateTimeOffset? Start { get; set; }

	[StartEndValidation]
	public DateTimeOffset? End { get; set; }

	public string Tags { get; set; }

	/// <summary>
	/// Provides
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	private sealed class StartEndValidation : ValidationAttribute
	{
		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
		{
			if (value is DateTimeOffset end && validationContext.ObjectInstance is EventFormData { Start: not null } data)
			{
				return StartIsBeforeEnd(data.Start.Value, end);
			}

			return null;
		}

		private ValidationResult? StartIsBeforeEnd(DateTimeOffset start, DateTimeOffset end)
		{
			return end <= start
				? new ValidationResult("End date cannot be equal or before start date.", new List<string>() { nameof(End) })
				: null;
		}
	}
}