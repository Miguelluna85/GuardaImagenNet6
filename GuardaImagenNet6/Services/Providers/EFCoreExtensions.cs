using Microsoft.EntityFrameworkCore;

namespace GuardaImagenNet6.Services.Providers;

public static class EFCoreExtensions
{
    public static async Task<PagedResult<TEntity>> GetPagedResultAsync<TEntity>
        (this IQueryable<TEntity> source, int pageSize, int currentPage)
        where TEntity : class
    {
        var rows = source.Count();// como usar con chachear
        var results = await source
            .Skip(pageSize * (currentPage - 1))
            .Take(pageSize)
            .ToListAsync();

        var list = new PagedResult<TEntity>()
        {
            CurrentPage = currentPage,
            PageCount = (int)Math.Ceiling((double)rows / pageSize),
            PageSize = pageSize,
            Results = results,
            RowsCount = rows
        };
        return list;
    }
}
