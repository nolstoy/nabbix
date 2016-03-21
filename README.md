# What?

A lightweight Zabbix Agent that can be embedded in any .Net program ~ ASP.Net, Windows Services or Console Apps.

# Why?

Zabbix does have a Windows Agent. It's largely limited to Performance Counters, Log Files and the Windows Event Log. When monitoring a .Net application, it's a non trivial task to install the agent and register custom performance counters. 

With Nabbix, monitoring a .Net program with Zabbix only requires referencing a NuGet package and creating a class that you want to be monitored.

# How?

## 1. Add NuGetPackage.

```
Install-Package Nabbix
```


## 2. Create a class.

3. Decorated the class with NabbixItem Attributes.

4. Create a Nabbix Agent and register your class.