# BlockingCollectionConsoleApp

This is a simple repro of a weird issue I'm seeing with `BlockingCollection<T>` 
in a console app where items are `Add`-ed to the collection on the main thread
while they are being consumed by the `TryTake` method on a background thread,
but the main thread takes a considerable more time to execute that expected.