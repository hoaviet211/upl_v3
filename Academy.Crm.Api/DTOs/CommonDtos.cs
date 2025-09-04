namespace Academy.Crm.Api.DTOs;

public record PagingDto(int Page, int PageSize, int Total);

public record PagedResult<T>(IEnumerable<T> Items, PagingDto Paging);
