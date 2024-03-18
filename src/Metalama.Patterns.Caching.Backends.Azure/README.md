<p align="center">
    <img width="450" src="https://github.com/postsharp/Metalama/raw/master/images/metalama-by-postsharp.svg" alt="Metalama logo" />
    <img referrerpolicy="no-referrer-when-downgrade" src="https://postsharp.matomo.cloud/matomo.php?idsite=1&amp;rec=1" style="border:0" alt="" />
</p>

## About

The `Metalama.Patterns.Caching.Backends.Azure` package enhances the `Metalama.Patterns.Caching` package by introducing the synchronization of multiple in-memory caches, each typically deployed on a different node, through Azure Service Bus.

## Key Features

* Enables invalidation of multiple local caches through Azure Service Bus.

## Main Types

* [AzureCachingFactory](https://doc.postsharp.net/metalama/api/metalama-patterns-caching-backends-azure-azurecachingfactory) introduces a `WithAzureSynchronization` method that adds synchronization to a memory bus.

## Additional Documentation

* Conceptual documentation: [https://doc.postsharp.net/metalama/patterns/caching/pubsub](https://doc.postsharp.net/metalama/patterns/caching/pubsub)
* API reference: [https://doc.postsharp.net/metalama/api/metalama-patterns-caching-backends-azure](https://doc.postsharp.net/metalama/api/metalama-patterns-caching-backends-azure)

## Related Packages

* `Metalama.Patterns.Caching` provides the cache API abstractions.
* `Metalama.Patterns.Caching.Aspects` is an aspect-oriented API that builds upon `Metalama.Patterns.Caching`, and is typically the entry point package.
* `Metalama.Patterns.Caching.Backends.Redis` offers the same features over Redis Pub/Sub as opposed to Azure Service Bus.
