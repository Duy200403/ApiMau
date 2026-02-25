using Microsoft.Extensions.FileProviders;

namespace ApiWebsite.Helper
{
    public interface IMyFileProvider
    {

    }

    public class MyFileProvider : PhysicalFileProvider, IMyFileProvider
    {
      public MyFileProvider(string root, string alias) : base(root)
      {
        this.Alias = alias;
      }

      public MyFileProvider(string root, Microsoft.Extensions.FileProviders.Physical.ExclusionFilters filters, string alias) : base(root, filters)
      {
        this.Alias = alias;
      }

      /// 
      ///Alias
      /// 
      public string Alias { get; set; }
    }
}