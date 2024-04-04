using  Silver.WatchDog.src.Enums;

namespace  Silver.WatchDog.src.Models
{
    public class WatchDogOptionsModel
    {
        public string WatchPageUsername { get; set; } = "";
        public string WatchPagePassword { get; set; } = "";
        public string Blacklist { get; set; } = "";
        public string CorsPolicy { get; set; } = "";
        public WatchDogSerializerEnum Serializer { get; set; } = WatchDogSerializerEnum.Default;
    }
}
