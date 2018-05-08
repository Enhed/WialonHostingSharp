# WialonHostingSharp
.NET Standard 2.0 Library to access Wialon Hosting API

## Getting started

First of all, install `WialonHostingSharp` into your project using a NuGet client.

    Install-Package WialonHostingSharp

## Example

### Display objects with pattern by name

```c#

using System;
using WialonHostingSharp;
using WialonHostingSharp.Search;

async void DisplayObjects()
{
    const string token = "token";
    const string host = "host";

    // create current session
    var session = await SessionManager.Login(host, token);

    // create search service
    var search = new SearchService(session);

    // search pattren, default by name
    var pattern = "*123*";
    var objects = await search.GetObjects(pattern);

    Console.WriteLine($"Get x{objects.Length} objects");

    foreach(var obj in objects)
    {
        Console.WriteLine($"\t{obj.Name} : {obj.Id}");
    }

    // logout current session
    var logoutResult = await SessionManager.Logout(session);
    Console.WriteLine($"{nameof(logoutResult)}: {logoutResult}");
}

```

### Display data messages

```c#

private async void DisplayDataMessages()
{
    const string token = "token";
    const string host = "host";

    // create current session
    var session = await SessionManager.Login(BifHost, BifToken);

    // object id
    var id = 000;

    // create message service
    var ms = new MessageService(session);

    // load 3 messages
    var messages = await ms.GetMessageDatas(id, DateTime.Today, DateTime.Now, 3);

    Console.WriteLine($"Get x{messages.Length} messages");

    foreach(var msg in messages)
    {
        Console.WriteLine($"\t{msg.Time}: {msg.Position}");
    }

    await SessionManager.Logout(session);
}

```