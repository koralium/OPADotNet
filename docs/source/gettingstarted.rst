Getting Started
================

To get started with OPADotNet there is a main package named *OPADotNet.AspNetCore* which allows an *AspNetCore* application to start using OPA Policies for authorization.

This page describes a simplified way to get started, there are multiple different ways to configure, such as not using an embedded OPA service, adding synchronizations to remote services
to fetch policies etc.

Setup
******

Install the nuget package:

* OPADotNet.AspNetCore

In startup.cs add the following:

.. code-block:: csharp

  public void ConfigureServices(IServiceCollection services)
  {
      ...
      //Add OPA and configure a local policy
      services.AddOpa(x => x.AddSync(sync => sync
          //Add a local policy
          .UseLocal(local =>
          {
              local.AddPolicy(@"
              package test

              allow {
                input.operation = ""GET""
                # Owner of test data must be the current user
                data.testdata[_].owner = input.subject.name
              }
              ");
          })
      ));
      ...
      services.AddAuthorization(opt =>
      {
          opt.AddPolicy("read", x => x.RequireOpaPolicy("test", "testdata", "GET"));
      });
      ...
  }

The example above uses the embedded OPA client and adds a simple OPA policy. It then configures an AspNetCore Authorization policy to use that OPA policy as a requirement. 

The command :code:`RequireOpaPolicy("test", "testdata", "GET")` is an important command here, the parameters mean:

* *First parameter* - The name of the OPA policy to run, will be transalted to "data.{policyName}.allow == true" when evaluating a policy.
* *Second parameter* - Name of the unknown data, this is data that is not stored in OPAs storage, but will instead be evaluated with C# objects.
* *Third parameter* - Custom name that will be added to "input.operation".

Usage
******

When the framework has been configured, its time to use it. There are multiple ways to authorize resources.

Queryable data authorization
-----------------------------

This use case is when one wants to get a list of resources that a user has access to from a database or similar.

It builds on creating an expression from an OPA partial query that is then applied to the queryable that the user provides.

Example:

Given the following OPA policy:

.. code-block:: csharp

  package testpolicy

  allow {
    input.subject
    data.reports[_].owner = input.subject.name
  }

And the following AspNetCore policy:

.. code-block:: csharp

  opt.AddPolicy("read", x => x.RequireOpaPolicy("testpolicy", "reports", "GET"));

Then this is an example on where the current user can only see reports they are owner of:

.. code-block:: csharp

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
      var authResult = await _authorizationService.AuthorizeQueryable(
        HttpContext.User, 
        _dbContext.Reports, 
        "read");

      if (!authResult.Succeeded)
      {
          return Unauthorized();
      }

      return Ok(authResult.Queryable);
    }
  }


Authorize endpoint access
---------------------------

To authorize that a user can access an endpoint, one only has to add:

.. code-block:: csharp

  [Authorize(Policy = "read")]
  public class ValuesController : ControllerBase
  {
    ...
  }

Important to note when authorizing an endpoint is that it does not check resources. What is happening behind the scenes is that a partial query is ran with the specified data unknown.

This means that the user perhaps does not have access to all the data, but is allowed to call the endpoint.

If one does not have any dependency on dynamic data and all the data is stored in OPA, this will validate the data that is stored in OPA.

Authorize a resource
---------------------

This use case can be used when one wants to authorize if a user can for instance add a new resource, or get a specific one.

.. code-block:: csharp

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
