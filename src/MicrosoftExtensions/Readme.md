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
