namespace StackOverflowLoaderAPI.Models.ApiTags;

public class ItemParameters
{
    const int maxPageSize = 50;
    const int minPageSize = 1;
    public int PageNumber { get; set; } = 1; //default

    private int _pageSize = 10; //default
    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            _pageSize = value;
            if (value < minPageSize) _pageSize = minPageSize;
            if (value > maxPageSize) _pageSize = maxPageSize;

        }
    }
    public string OrderDirection { get; set; } = "ascending"; //default
    public string OrderBy { get; set; } = "name"; //default
}
