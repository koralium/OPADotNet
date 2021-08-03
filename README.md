<br />
<p align="center">
  <h1 align="center">OPADotNet.AspNetCore</h1>

  <p align="center">
    AspNetCore OPA framework to use opa policies in your API's.
</p>



<!-- ABOUT THE PROJECT -->
## About

This framework allows an AspNetCore API to use OPA policies together with AspNetCore policies to authorize requests and resources. It also features a way to translate OPA partial queries to C# expressions that can be used to pass down permissions through EntityFrameworkCore.

The framework integrates with OPA using the rego golang sdk and has the rego sdk embedded into the nuget. It synchronizes with an OPA server to get the latest policies and data required for the policies that are used.

It also tries to follow the existing aspnetcore policy framework described in: https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-5.0 and https://docs.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased?view=aspnetcore-5.0

The project contains two main nuget packages:

* **OPADotNet.AspNetCore** - Required to add OPA to your API.
* **OPADotNet.TestFramework** - Test framework to run a simple OPA server required for the framework, this is useful for unit testing. 


## Setup

Install the nuget package:

* OPADotNet.AspNetCore

In startup.cs add the following:

```
public void ConfigureServices(IServiceCollection services)
{
    ...
    //Add OPA and connects to an OPA server to sync the policies and data required.
    services.AddOpa(x => x.AddSync(sync => sync
        .UseOpaServer("http://127.0.0.1:8181", TimeSpan.FromMinutes(5))
    ));
    services.AddAuthorization(opt =>
    {
        opt.AddPolicy("read", x => x.RequireOpaPolicy("testpolicy", "reports", "GET"));
    });
    ...
}
```

The following command:
```
services.AddOpa(x => x.AddSync(sync => sync
        .UseOpaServer("http://127.0.0.1:8181", TimeSpan.FromMinutes(5))
    ));
```
Adds a connection to an OPA server and begins syncing policies and the required data for the policies to the embedded OPA server. The second parameter sets a time span how often data and policies should be synced from the OPA server.

```
opt.AddPolicy("read", x => x.RequireOpaPolicy("testpolicy", "reports", "GET"));
```
This command adds a aspnetcore policy that uses an OPA policy, the first parameter is which policy to use, this will be used and be translated into the following query:

```
data.testpolicy.allow == true
```
The second parameter is the name of the dynamic data object which is used for resource based authorization. If one does not require resource based authorization, then this can have any value.

This value is translated into two objects:

* data.reports
* input.reports

When one does single resource authorization the input field will be set with the current resource.

The third parameter is set to the input value "input.operation"

## Input constants

When any Authorize command is ran, the following constants are added to the input:

* **input.subject** - The current user
* **input.subject.name** - The current user name, translated from claimsPrincipal.Identity.Name
* **input.subject.claims[]** - An array of the current user claims
* **input.subject.claims[_].type** - The type that the claim is, for example "role".
* **input.subject.claims[_].value** - The value of the claim.
* **input.operation** - The operation name set when configuring the policy.

When doing a single resource authorization (not queryable), the following input is also added:

* **input.resource** - This contains the resource that is being authorized. 

## Usage

When the framework has been configured, its time to use it. There are multiple ways to authorize resources.

### Queryable data authorization

This use case is when one wants to get a list of resources that a user has access to from a database or similar.

It builds on creating an expression from an OPA partial query that is then applied to the queryable that the user provides.

Example:,

Given the following OPA policy:

```
package testpolicy

allow {
  input.subject
  data.reports[_].owner = input.subject.name
}
```

And the following AspNetCore policy:

```
opt.AddPolicy("read", x => x.RequireOpaPolicy("testpolicy", "reports", "GET"));
```

Then this is an example on where the current user can only see reports they are owner of:

```
public class ValuesController : ControllerBase
{
  private readonly IAuthorizationService _authorizationService;
  private readonly TestContext _dbContext;

  public ValuesController(IAuthorizationService authorizationService, TestContext dbContext)
  {
      _authorizationService = authorizationService;
      _dbContext = dbContext;
  }

  [HttpGet]
  public async Task<IActionResult> Get()
  {
    var (authResult, filteredData) = await _authorizationService.AuthorizeQueryable(
      HttpContext.User, 
      _dbContext.Reports, 
      "read");

    if (!authResult.Succeeded)
    {
        return Unauthorized();
    }

    return Ok(filteredData);
  }
}
```

### Authorize endpoint access

To authorize that a user can access an endpoint, one only has to add:

```
[Authorize(Policy = "read")]
public class ValuesController : ControllerBase
{
  ...
}
```

Important to note when authorizing an endpoint is that it does not check resources. What is happening behind the scenes is that a partial query is ran with the specified data unknown.

This means that the user perhaps does not have access to all the data, but is allowed to call the endpoint.

If one does not have any dependency on dynamic data and all the data is stored in OPA, this will validate the data that is stored in OPA.

### Authorize a resource

This use case can be used when one wants to authorize if a user can for instance add a new resource, or get a specific one.

```
public class ValuesController : ControllerBase
{
  private readonly IAuthorizationService _authorizationService;

  public ValuesController(IAuthorizationService authorizationService)
  {
      _authorizationService = authorizationService;
  }

  [HttpPost]
  public async Task<IActionResult> Post([FromBody]TestModel data)
  {
      var authResult = await _authorizationService.AuthorizeAsync(HttpContext.User, data, "write");

      if (!authResult.Succeeded)
      {
          return Unauthorized();
      }

      return Ok();
  }
}
```
