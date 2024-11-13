# BlockingCollectionConsoleApp

The following was the original description of the issue, which turned out to be **weird by itself**,
because according to the [documentation](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.delay?view=net-8.0), the `Task.Delay` resolution is around 15 ms and the program assumed that calling `Task.Delay(millisecondsDelay: 1)` will actually last around one millisecond (classical case of the [RTFM](https://en.wikipedia.org/wiki/RTFM) issue 😕).

FMI: See [this StackOverflow](https://stackoverflow.com/questions/79182433/blockingcollection-add-in-a-loop-with-await-taking-considerable-more-time-than-e) question

### This is the original repo description for history 😄:

This is a simple repro of a weird issue I'm seeing with `BlockingCollection<T>`
in a console app where items are `Add`-ed to the collection on the main thread
while they are being consumed by the `TryTake` method on a background thread,
but the main thread takes a considerable more time to execute that expected.
