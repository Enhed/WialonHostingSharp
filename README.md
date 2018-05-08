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