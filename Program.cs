using System.Collections.Concurrent;
using System.Diagnostics;

// Create bounded collection.
var collection = new BlockingCollection<int>(boundedCapacity: 1000);

// Create and start a background thread that consumes items from the collection.
var stoppingCancellationTokenSource = new CancellationTokenSource();
Thread backgroundThread = new Thread(BackgroundThreadDelegate)
{
  IsBackground = true
};
backgroundThread.Start(parameter: (stoppingCancellationTokenSource.Token, collection));

// Measure the time it takes to add items to the collection with one given delay between Adds.
int delayBetweenAddsInMilliseconds = 15;
var sw = Stopwatch.StartNew();
for (int i = 0; i < collection.BoundedCapacity; i++)
{
  collection.Add(i);
  await Task.Delay(millisecondsDelay: delayBetweenAddsInMilliseconds); 
}
sw.Stop();

// The following will always print in Red meaning the elapsed time is always greater than the max expected time.
// Even the more interesting thing is that under debugger, the times are in the range of 3.2 - 4 seconds,
// while when running without debugger, the times are in the range of 15 - 16 seconds!
int maxExpectedMilliseconds = (2 * collection.BoundedCapacity) * delayBetweenAddsInMilliseconds;
Console.ForegroundColor = (sw.ElapsedMilliseconds <= maxExpectedMilliseconds) ? ConsoleColor.Green : ConsoleColor.Red;
Console.WriteLine($"Elapsed milliseconds: {sw.ElapsedMilliseconds}, Max. expected milliseconds: {maxExpectedMilliseconds}");

// Cleanup.
stoppingCancellationTokenSource.Cancel();
backgroundThread.Join(1000);

static void BackgroundThreadDelegate(object? state)
{
  (CancellationToken stoppingToken, BlockingCollection<int> collection) = ((CancellationToken, BlockingCollection<int>))state!;
  while (!stoppingToken.IsCancellationRequested)
  {
    try
    {
      if (collection.TryTake(out int item, millisecondsTimeout: 1, cancellationToken: stoppingToken))
      {
        Debug.WriteLine($"Item --> {item}");
      }
    }
    catch (OperationCanceledException ex) when (ex.CancellationToken == stoppingToken)
    {
      // Normal cancellation; ignore.
    }
  }
}