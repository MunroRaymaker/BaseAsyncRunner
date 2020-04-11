namespace BaseAsyncRunner
{
    public interface IConfig
    {
        string Environment { get; set; }
    }

    public class Config : IConfig
    {
        public string Environment { get; set; }
    }
}