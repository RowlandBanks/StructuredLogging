## Introduction

Integrates structured logging with the Microsoft Logging Extensions framework.

## Usage

```c#
ILogger<SomeType> logger;

// Add additional context to logging messages within the scope
using logger.BeginScope(new {
    HttpContext = httpContext
});

// Log an anonymous object.
logger.Log(LogLevel.Information, new {
    Event = "UserRegistered",
    Name = "Grace Hopper",
    Age = 42
});

// Log an object implementing IEvent
Logger.Log(new UserRegistered("Grace Hopper", 42));
```

## Scopes

Scopes let you attach the same state to all logs inside the scope. For example, this would
let you a transaction id if processing a batch of items.

You can nest scopes, so a common practice is to have a scope that is valid for the lifetime of your
application, containing your application name, version, hostname, and other useful global values.

```c#
logger.BeginScope(new {
    Application = new {
        Name = "my-service",
        Version = "1.2.3"
    },
    Host = new {
        Name = GetNameOfHost(),
        IP = GetIpAddress()
    }
})
```

To stop using the scope, just dispose it:

```c#
using (logger.BeginScope(new {
    TransactionId = "abc123"
}))
{
    logger.Log(new {
        Event = "TransactionProcessed"
    });
}
```
