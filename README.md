# finbuckle-DataIsolationSample
Demo of Finbuckle.MultiTenant using an in-memory MultiTenant Store and per-tenant data isolation.

Finbuckle.MultiTenant is an open source multitenancy library for .NET.
https://github.com/Finbuckle/Finbuckle.MultiTenant

This is a modification of the original data isolation sample from the Finbuckle project.
https://github.com/Finbuckle/Finbuckle.MultiTenant/tree/develop/samples/ASP.NET%20Core%203/DataIsolationSample

A list of changes made to it:

* ASP.NET Core dependencies have been updated to use Net Core SDK 3.1.405

* Configured to use an in-memory tenant store instead of the read-only ConfigurationStore

* Uses EF data migrations with the tenant context rather than Database.EnsureCreated()

* Each tenant has its own SQLite database

* The home page was updated to reflect these changes
