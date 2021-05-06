namespace ServiceDiscoveryWebApi.Models
{
    public class ApplicationSettings
    {
        public string ConsulHost { get; set; }
        public string ServiceName { get; set; }
        public string ServiceID { get; set; }
        public string ServicePort { get; set; }
    }
}