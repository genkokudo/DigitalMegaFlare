namespace DigitalMegaFlare.Settings
{
    public class Rootobject
    {
        public DefaultParameters DefaultParameter { get; set; }
    }

    /// <summary>
    /// appSettingsの内容を読み込む
    /// </summary>
    public class DefaultParameters
    {
        public object[][] DefaultValues { get; set; }
        //public bool IsDemoMode { get; set; }
        //public DefaultUser DefaultUser { get; set; }

    }
}
