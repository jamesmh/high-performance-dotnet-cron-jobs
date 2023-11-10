using Workers;
using Coravel;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddScheduler();
builder.Services.AddTransient<string>(p => "Server=host.docker.internal,1433;Database=WideWorldImporters-Standard;User Id=sa;Password=P@assword;TrustServerCertificate=True;");
builder.Services.AddTransient<ProcessAllOrdersInvocable>();
builder.Services.AddTransient<ProcessAllCitiesInvocable>();
builder.Services.AddTransient<ProcessAllInvoicesInvocable>();
builder.Services.AddTransient<ProcessAllStockItemTransactionsInvocable>();

var host = builder.Build();

host.Services.UseScheduler(scheduler =>
{
    scheduler.Schedule(() =>
        {
            Console.WriteLine("### Total records processed: " + TotalRecordsProcessed.Value);
        })
        .EverySecond();
    
    scheduler.Schedule<ProcessAllOrdersInvocable>()
        .EverySeconds(5)
        .PreventOverlapping(nameof(ProcessAllOrdersInvocable));

    scheduler.Schedule<ProcessAllCitiesInvocable>()
        .EverySeconds(5)
        .PreventOverlapping(nameof(ProcessAllCitiesInvocable));

    // Dedicating a separate thread for this job.
    scheduler.OnWorker(nameof(ProcessAllStockItemTransactionsInvocable));
    scheduler.Schedule<ProcessAllStockItemTransactionsInvocable>()
        .EverySeconds(5)
        .PreventOverlapping(nameof(ProcessAllStockItemTransactionsInvocable));

    // Dedicating a separate thread for this job too.
    scheduler.OnWorker(nameof(ProcessAllInvoicesInvocable));
    scheduler.Schedule<ProcessAllInvoicesInvocable>()
        .EverySeconds(5)
        .PreventOverlapping(nameof(ProcessAllInvoicesInvocable));

}).LogScheduledTaskProgress(host.Services.GetService<ILogger<IScheduler>>());

await host.RunAsync();
