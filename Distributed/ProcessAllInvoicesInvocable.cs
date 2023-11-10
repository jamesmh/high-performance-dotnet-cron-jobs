using System.Diagnostics;
using Coravel.Invocable;
using Dapper;
using Medallion.Threading.SqlServer;
using Microsoft.Data.SqlClient;

namespace Distributed;

public class ProcessAllInvoicesInvocable : IInvocable
{
    private string connectionString;

    public ProcessAllInvoicesInvocable(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public async Task Invoke()
    {
        var @lock = new SqlDistributedLock(nameof(ProcessAllInvoicesInvocable), this.connectionString);
        await using var handle = await @lock.TryAcquireAsync();
        if (handle != null)
        {
            var lastIdProcessed = 0;
            var watch = new Stopwatch();
            watch.Start();

            await using var connection = new SqlConnection(this.connectionString);
            
            while (true)
            {
                var items = (await connection.QueryAsync<(int InvoiceId, string ReturnedDeliveryData)>
                    (SQL, new { LastIdProcessed = lastIdProcessed })).AsList();

                if (!items.Any())
                {
                    break;
                }

                var tasks = new List<Task>(items.Count);
                foreach (var item in items)
                {
                    tasks.Add(SimulateProcessOrderAsync(item));
                }

                await Task.WhenAll(tasks);

                lastIdProcessed = items.Last().InvoiceId;
                Interlocked.Add(ref TotalRecordsProcessed.Value, items.Count);
            }
            
            watch.Stop();
            Console.WriteLine($"### {nameof(ProcessAllInvoicesInvocable)} took {watch.ElapsedMilliseconds} ms. Finished at {DateTime.UtcNow.ToUniversalTime()}");
        }
    }

    private static async Task SimulateProcessOrderAsync(object order)
    {
        await Task.Delay(10);
    }

    private const string SQL = @"
SELECT TOP 5000
    *
FROM Sales.Invoices 
WHERE 
    InvoiceID > @LastIdProcessed
ORDER BY InvoiceID";
}
