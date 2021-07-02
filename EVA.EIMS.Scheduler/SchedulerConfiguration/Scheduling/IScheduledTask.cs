using System.Threading;
using System.Threading.Tasks;

namespace EVA.EIMS.Scheduler.SchedulerConfiguration
{
    public interface IScheduledTask
    {
        string Schedule { get; }
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}