using Quartz;

namespace QuizSystem.Jobs;

[DisallowConcurrentExecution]
public class UpdateSharedTestStatusJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        throw new NotImplementedException();
    }
}
