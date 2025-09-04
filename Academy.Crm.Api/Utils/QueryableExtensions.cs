using System.Linq.Expressions;

namespace Academy.Crm.Api.Utils;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplySearch<T>(this IQueryable<T> query, string? q, params string[] properties)
    {
        if (string.IsNullOrWhiteSpace(q) || properties.Length == 0) return query;
        var param = Expression.Parameter(typeof(T), "x");
        Expression? combined = null;
        foreach (var propName in properties)
        {
            var prop = Expression.PropertyOrField(param, propName);
            var toStringCall = Expression.Call(prop, typeof(object).GetMethod("ToString")!);
            var contains = Expression.Call(toStringCall, typeof(string).GetMethod("Contains", new[] { typeof(string), typeof(StringComparison) })!, Expression.Constant(q), Expression.Constant(StringComparison.InvariantCultureIgnoreCase));
            combined = combined == null ? contains : Expression.OrElse(combined, contains);
        }
        if (combined == null) return query;
        var lambda = Expression.Lambda<Func<T, bool>>(combined, param);
        return query.Where(lambda);
    }

    public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort)) return query;
        var fields = sort.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        bool first = true;
        foreach (var field in fields)
        {
            var desc = field.StartsWith('-');
            var name = desc ? field[1..] : field;
            var param = Expression.Parameter(typeof(T), "x");
            var body = Expression.PropertyOrField(param, name);
            var keySelector = Expression.Lambda(body, param);
            string method = first ? (desc ? "OrderByDescending" : "OrderBy") : (desc ? "ThenByDescending" : "ThenBy");
            query = (IQueryable<T>)typeof(Queryable).GetMethods()
                .First(m => m.Name == method && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), body.Type)
                .Invoke(null, new object[] { query, keySelector })!;
            first = false;
        }
        return query;
    }

    public static (IQueryable<T> query, int total) ApplyPaging<T>(this IQueryable<T> query, int page, int pageSize)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 10 : Math.Min(pageSize, 100);
        var total = query.Count();
        query = query.Skip((page - 1) * pageSize).Take(pageSize);
        return (query, total);
    }

    public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, IQueryCollection queryCollection)
    {
        foreach (var (key, value) in queryCollection)
        {
            if (!key.StartsWith("filter[")) continue;
            // filter[Field][op]=value
            // parse Field and op
            var inside = key[7..]; // after 'filter['
            var closeIdx = inside.IndexOf(']');
            if (closeIdx <= 0) continue;
            var field = inside[..closeIdx];
            var opPart = inside[(closeIdx + 1)..]; // expect [op]
            if (opPart.Length < 3 || opPart[0] != '[' || opPart[^1] != ']') continue;
            var op = opPart[1..^1];
            var val = value.ToString();

            var param = Expression.Parameter(typeof(T), "x");
            var member = Expression.PropertyOrField(param, field);
            Expression? predicate = op switch
            {
                "eq" => BuildComparison(member, val, "=="),
                "gte" => BuildComparison(member, val, ">="),
                "lte" => BuildComparison(member, val, "<="),
                "contains" => BuildContains(member, val),
                "in" => BuildIn(member, val),
                "between" => BuildBetween(member, val),
                _ => null
            };
            if (predicate == null) continue;
            var lambda = Expression.Lambda<Func<T, bool>>(predicate, param);
            query = query.Where(lambda);
        }
        return query;
    }

    private static Expression? BuildComparison(MemberExpression member, string val, string op)
    {
        try
        {
            var constant = Expression.Constant(ConvertTo(member.Type, val), member.Type);
            return op switch
            {
                "==" => Expression.Equal(member, constant),
                ">=" => Expression.GreaterThanOrEqual(member, constant),
                "<=" => Expression.LessThanOrEqual(member, constant),
                _ => null
            };
        }
        catch { return null; }
    }

    private static Expression? BuildContains(MemberExpression member, string val)
    {
        if (member.Type != typeof(string)) return null;
        return Expression.Call(member, typeof(string).GetMethod("Contains", new[] { typeof(string), typeof(StringComparison) })!, Expression.Constant(val), Expression.Constant(StringComparison.InvariantCultureIgnoreCase));
    }

    private static Expression? BuildIn(MemberExpression member, string val)
    {
        var parts = val.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(v => ConvertTo(member.Type, v)).ToArray();
        var arr = Array.CreateInstance(member.Type, parts.Length);
        for (int i = 0; i < parts.Length; i++) arr.SetValue(parts[i], i);
        var contains = typeof(Enumerable).GetMethods().First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
            .MakeGenericMethod(member.Type);
        return Expression.Call(null, contains, Expression.Constant(arr), member);
    }

    private static Expression? BuildBetween(MemberExpression member, string val)
    {
        var parts = val.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 2) return null;
        var a = Expression.Constant(ConvertTo(member.Type, parts[0]), member.Type);
        var b = Expression.Constant(ConvertTo(member.Type, parts[1]), member.Type);
        var ge = Expression.GreaterThanOrEqual(member, a);
        var le = Expression.LessThanOrEqual(member, b);
        return Expression.AndAlso(ge, le);
    }

    private static object ConvertTo(Type type, string val)
    {
        if (type == typeof(string)) return val;
        if (type == typeof(int) || type == typeof(int?)) return int.Parse(val);
        if (type == typeof(decimal) || type == typeof(decimal?)) return decimal.Parse(val);
        if (type == typeof(DateTime) || type == typeof(DateTime?)) return DateTime.Parse(val);
        if (type == typeof(DateOnly) || type == typeof(DateOnly?)) return DateOnly.Parse(val);
        if (type == typeof(bool) || type == typeof(bool?)) return bool.Parse(val);
        if (type.IsEnum) return Enum.Parse(type, val, true);
        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying != null && underlying.IsEnum) return Enum.Parse(underlying, val, true);
        throw new NotSupportedException($"Unsupported filter type: {type.Name}");
    }
}

