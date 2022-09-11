using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TamboliyaApi.Data;

namespace TamboliyaApi.Services
{
	public class GenericRepository<TEntity> where TEntity : class
	{
		internal AppDbContext context;
		internal DbSet<TEntity> dbSet;

		public GenericRepository(AppDbContext context)
		{
			this.context = context;
			this.dbSet = context.Set<TEntity>();
		}

		public virtual async Task<IEnumerable<TEntity>> GetAsync(
			Expression<Func<TEntity, bool>> filter = null!,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null!,
			string includeProperties = "")
		{
			IQueryable<TEntity> query = dbSet;

			if (filter != null)
			{
				query = query.Where(filter);
			}

			foreach (var includeProperty in includeProperties.Split
				(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
			{
				query = query.Include(includeProperty);
			}

			if (orderBy != null)
			{
				return await orderBy(query).ToListAsync();
			}
			else
			{
				return await query.ToListAsync();
			}
		}

		public virtual async Task<TEntity?> GetByIDAsync(Expression<Func<TEntity, bool>> filter, string includeProperties = "")
		{
			IQueryable<TEntity> query = dbSet;

			if (filter != null)
			{
				query = query.Where(filter);
			}

			foreach (var includeProperty in includeProperties.Split
				(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
			{
				query = query.Include(includeProperty);
			}

			return (await query.ToListAsync()).FirstOrDefault();
		}

		public virtual void Insert(TEntity entity)
		{
			dbSet.Add(entity);
		}

		public virtual void Delete(object id)
		{
			TEntity entityToDelete = dbSet.Find(id)!;
			Delete(entityToDelete);
		}

		public virtual void Delete(TEntity entityToDelete)
		{
			if (context.Entry(entityToDelete).State == EntityState.Detached)
			{
				dbSet.Attach(entityToDelete);
			}
			dbSet.Remove(entityToDelete);
		}

		public virtual void Update(TEntity entityToUpdate)
		{
			dbSet.Attach(entityToUpdate);
			context.Entry(entityToUpdate).State = EntityState.Modified;
		}
	}
}
