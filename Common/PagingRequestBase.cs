namespace ApiWebsite.Common
{
  public class PagingRequestBase
  {
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    // public string OrderField { get; set; }
    // public string OrderDirection { get; set; }
  }
}