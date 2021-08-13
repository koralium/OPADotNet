# Example

This example shows an example of an AspNetCore application.
It is set up to check access more inline to how an API can check access, but it uses MVC to provide an easy testable interface.

Important classes and files are:

* Startup.cs
* Controllers/DataController.cs
* policies/*.rego

The sample also shows how to use the AutoMapper support to get a permissions object on each resource.
This can be viewed in Startup on how to configure, and in DataController under the Index method.