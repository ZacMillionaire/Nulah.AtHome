using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Nulah.AtHome.Data.Criteria;

namespace Nulah.AtHome.Data;

public static class CriteriaExtension
{
	/// <summary>
	/// Wrapper around dbSet.Where(criteriaMethod) to ensure if no criteria are supplied, the original dbSet is returned
	/// with no null reference attempt.
	/// <para>
	/// This is useful for when the supplied <paramref name="criteriaMethod"/> is contained within an implemented <see cref="ICritera"/>,
	/// but the caller accepts criteria as nullable.
	/// </para>
	/// <para>
	/// If your <paramref name="criteriaMethod"/> comes from a direct method that cannot be null, use .Where([expression returning method]) directly instead.
	/// </para>
	/// </summary>
	/// <param name="dbSet"></param>
	/// <param name="criteriaMethod"></param>
	/// <typeparam name="TEntity"></typeparam>
	/// <returns></returns>
	public static IQueryable<TEntity> WithCriteria<TEntity>(this DbSet<TEntity> dbSet,
		Expression<Func<TEntity, bool>>? criteriaMethod)
		where TEntity : class
	{
		return criteriaMethod == null ? dbSet : dbSet.Where(criteriaMethod);
	}
}