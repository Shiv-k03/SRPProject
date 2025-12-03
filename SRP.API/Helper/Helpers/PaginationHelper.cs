using SRP.API.Helper.Base;

namespace SRP.API.Helper.Helpers
{
    public static class PaginationHelper
    {
        public static PaginatedResult<T> CreatePaginationResult<T>(
            IQueryable<T> source,
            int pageNumber,
            int pageSize)
        {
            var count = source.Count();
            var items = source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new PaginatedResult<T>(items, count, pageNumber, pageSize);
        }

        public static int CalculateTotalPages(int totalCount, int pageSize)
        {
            return (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        public static bool IsValidPage(int pageNumber, int totalPages)
        {
            return pageNumber > 0 && pageNumber <= totalPages;
        }

        public static PaginatedResult<T> Create<T>(
            List<T> items,
            int totalCount,
            int pageNumber,
            int pageSize)
        {
            return new PaginatedResult<T>(items, totalCount, pageNumber, pageSize);
        }
    }
}
