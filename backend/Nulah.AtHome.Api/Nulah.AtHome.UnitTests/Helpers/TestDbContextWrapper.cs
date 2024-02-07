using System.Reflection;
using Nulah.AtHome.Data;

namespace Nulah.AtHome.UnitTests.Helpers;

/// <summary>
/// Automatically handles clearing context tracking for any object that has an instance of <see cref="AppDbContext"/>
/// </summary>
public class TestDbContextWrapper : IDisposable
{
	private readonly object _contextConsumingClass;

	/// <summary>
	/// Creates a context wrapper around the given class that contains a reference or instance to <see cref="AppDbContext"/>
	/// </summary>
	/// <param name="contextConsumingClass"></param>
	public TestDbContextWrapper(object contextConsumingClass)
	{
		_contextConsumingClass = contextConsumingClass;
	}

	public void Dispose()
	{
		ClearContextTracking(_contextConsumingClass);
	}

	/// <summary>
	/// Clears the change tracker on the given object, that has a private field for <see cref="AppDbContext"/>.
	/// <para>
	///	This is used for when tests create local entities that can affect subsequent calls where an Includes or otherwise may be missing.
	/// </para>
	/// <para>
	///	As save changes tracks changes locally, this can cause issues where entities that shouldn't be included are, absent a proper entity include.
	/// </para>
	/// </summary>
	/// <param name="objectWithContextField"></param>
	private void ClearContextTracking(object objectWithContextField)
	{
		var t = objectWithContextField.GetType();
		// Get the first field that contains our context.
		// If the object has multiple context fields...why
		// private fields are also just a suggestion, nothing can hide from reflection
		var contextField = t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
			.FirstOrDefault(x => x.FieldType == typeof(AppDbContext));

		var context = contextField?.GetValue(objectWithContextField) as AppDbContext;
		context?.ChangeTracker.Clear();
	}
}