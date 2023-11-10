using Distributed;
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
    scheduler.Schedule<ProcessAllOrdersInvocable>()
        .EverySeconds(5)
        .PreventOverlapping(nameof(ProcessAllOrdersInvocable));

    scheduler.Schedule<ProcessAllCitiesInvocable>()
        .EverySeconds(5)
        .PreventOverlapping(nameof(ProcessAllCitiesInvocable));
    
    scheduler.Schedule<ProcessAllStockItemTransactionsInvocable>()
        .EverySeconds(5)
        .PreventOverlapping(nameof(ProcessAllStockItemTransactionsInvocable));
    
    scheduler.Schedule<ProcessAllInvoicesInvocable>()
        .EverySeconds(5)
        .PreventOverlapping(nameof(ProcessAllInvoicesInvocable));
    
    scheduler.Schedule(() =>
        {
            Console.WriteLine("### Total records processed: " + TotalRecordsProcessed.Value);
        })
        .EverySecond();

}).LogScheduledTaskProgress(host.Services.GetService<ILogger<IScheduler>>());

await host.RunAsync();
