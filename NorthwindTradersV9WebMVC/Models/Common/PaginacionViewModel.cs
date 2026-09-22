namespace NorthwindTradersV9WebMVC.Models.Common
{
    public class PaginacionViewModel
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalRegistros { get; set; }

        public int TotalPages =>
            (int)Math.Ceiling((double)TotalRegistros / PageSize);
    }
}
