using System.Diagnostics;
using Coravel.Invocable;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Basic;

public class ProcessAllStockItemTransactionsInvocable : IInvocable
{
    private string connectionString;

    public ProcessAllStockItemTransactionsInvocable(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public async Task Invoke()
    {
        var lastIdProcessed = 0;
        var watch = new Stopwatch();
        watch.Start();

        await using var connection = new SqlConnection(this.connectionString);

        while (true)
        {
            var items = (await connection.QueryAsync<(int StockItemTransactionId, string Quantity)>
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

            lastIdProcessed = items.Last().StockItemTransactionId;
            Interlocked.Add(ref TotalRecordsProcessed.Value, items.Count);
        }

        watch.Stop();
        Console.WriteLine(
            $"### {nameof(ProcessAllStockItemTransactionsInvocable)} took {watch.ElapsedMilliseconds} ms");
    }

    private static async Task SimulateProcessOrderAsync(object order)
    {
        await Task.Delay(10);
    }

    private const string SQL = @"
SELECT TOP 5000
    *
FROM Warehouse.StockItemTransactions 
WHERE 
    StockItemTransactionId > @LastIdProcessed
ORDER BY StockItemTransactionId";
}
