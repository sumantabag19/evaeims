using System.Threading;
using System.Threading.Tasks;

namespace EVA.EIMS.AutoEmailerScheduler.SchedulerConfiguration
{
    public interface IScheduledTask
    {
        string Schedule { get; }
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}