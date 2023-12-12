namespace QuizSystem.Configurations;
using Quartz;
using QuizSystem.Jobs;

public static class QuartzExtension
{
    public static void AddQuartzConfiguration(this IServiceCollection services)
    {
        services.AddQuartz(options =>
        {
            options.UseMicrosoftDependencyInjectionJobFactory();

            var jobKey = JobKey.Create(nameof(UpdateSharedTestStatusJob));
            options
                .AddJob<UpdateSharedTestStatusJob>(jobKey)
                .AddTrigger(trigger =>
                    trigger
                        .ForJob(jobKey)
                        .WithSimpleSchedule(schedule =>
                            schedule.WithIntervalInMinutes(1)
                            .RepeatForever()));
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
    }
}
