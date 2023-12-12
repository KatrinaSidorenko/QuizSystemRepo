using BLL.Interfaces;
using Quartz;

namespace QuizSystem.Jobs;

[DisallowConcurrentExecution]
public class UpdateSharedTestStatusJob : IJob
{
    private readonly ISharedTestService _sharedTestService;
    public UpdateSharedTestStatusJob(ISharedTestService sharedTestService)
    {
        _sharedTestService = sharedTestService;
    }
    public async Task Execute(IJobExecutionContext context)
    {
        await _sharedTestService.UpdateSharedTestStatus();
    }
}
