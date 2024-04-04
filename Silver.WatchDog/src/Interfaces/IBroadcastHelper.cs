using System.Threading.Tasks;
using  Silver.WatchDog.src.Models;

namespace  Silver.WatchDog.src.Interfaces
{
    public interface IBroadcastHelper
    {
        Task BroadcastWatchLog(WatchLog log);
        Task BroadcastExLog(WatchExceptionLog log);
        Task BroadcastLog(WatchLoggerModel log);
    }
}
