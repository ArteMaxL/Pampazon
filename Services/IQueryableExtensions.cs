using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Pampazon.Services;

public static class IQueryableExtensions
{
    public static async Task<PagedResult<T>> ApplyPagedResultAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        string? search,
        string? orderBy,
        bool desc,
        Expression<Func<T, bool>>? searchPredicate = null,
        Dictionary<string, string>? orderMappings = null)
    {
        if (searchPredicate != null && !string.IsNullOrWhiteSpace(search))
            query = query.Where(searchPredicate);
        int total = await query.CountAsync();
        if (!string.IsNullOrWhiteSpace(orderBy) && orderMappings != null && orderMappings.TryGetValue(orderBy.ToLower(), out var mapped))
        {
            query = query.OrderBy(mapped + (desc ? " descending" : ""));
        }
        else
        {
            query = query.OrderBy("1"); // fallback, no-op
        }
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<T>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }
}
