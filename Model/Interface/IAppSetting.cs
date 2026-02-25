namespace ApiWebsite.Model.Interface
{
    public interface IAppSetting
    {
        public string Domain { get; set; }
        public string DomainNewsCrawler { get; set; }
        public string FileSizeLimit { get; set; }
    }
}