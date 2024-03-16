using System.Linq.Expressions;
using Nulah.AtHome.Data.Converters;
using Nulah.AtHome.Data.Models.Events;

namespace Nulah.AtHome.Data.Criteria;

public class EventListCriteria
{
	public bool? HasEventDate { get; set; }
	public DateTimeOffset? BeforeEndDate { get; set; }

	internal Expression<Func<BasicEvent, bool>> Build()
	{
		Expression<Func<BasicEvent, bool>>? baseFunc = null;

		if (HasEventDate.HasValue)
		{
			if (HasEventDate.Value)
			{
				baseFunc = baseFunc.And(x => x.End != null);
			}
			else
			{
				baseFunc = baseFunc.And(x => x.End == null);
			}
		}

		// Return an "empty" expression if we have a criteria object, but no criteria to act on
		baseFunc ??= x => true;

		if (baseFunc.CanReduce)
		{
			baseFunc.Reduce();
		}

		return baseFunc;
	}
}