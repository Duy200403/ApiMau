using ApiWebsite.Model.Interface;

namespace ApiWebsite.Model
{
    public class AppSetting : IAppSetting
    {
        public string Domain { get ; set; }
        public string DomainNewsCrawler { get ; set; }
        public string FileSizeLimit { get ; set; }
    }
}