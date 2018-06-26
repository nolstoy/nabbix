# What?

A lightweight Zabbix Agent that can be embedded in any .Net program ~ ASP.Net, Windows Service or Console Applications.

# Why?

Zabbix does have a Windows Agent. It's largely limited to Performance Counters, Log Files and the Windows Event Log. When monitoring a .Net application, it's a non trivial task to install the agent and register custom performance counters. 

With Nabbix, monitoring a .Net program only requires referencing a NuGet package and adding a few lines of code.

# How?

## 1. Add nabbix NuGet Package

```
Install-Package Nabbix
```

## 2. Create class(es) with the counter(s)

```

// Class containing a single Zabbix Item 'long_example'
private class MyCounter
{
	private long _incrementing;
    internal void Increment()
	{
		Interlocked.Increment(ref _incrementing);
	}

	[NabbixItem("long_example")]
    public long Incrementing => Interlocked.Read(ref _incrementing);
}
```

##3. Create a Nabbix Agent and register instances of the counter classes.

```
private static void Main()
{
	// Create the instance of the counter class with a single Zabbix Item
    var counters = new MyCounter();
            
    // Start the agent.
    var agent = new NabbixAgent(10052, counters);

    // Increment the counter. Normally done on API or method call.
    counters.Increment();

    // Shutdown
	Console.ReadKey();
    agent.Stop();
}
```

# More information?

https://github.com/nolstoy/nabbix/wiki
