.. _querydataauth:

Queryable data authorization
=============================

This use case is when one wants to get a list of resources that a user has access to from a database or similar.
It builds on creating an expression from an OPA partial query that is then applied to the queryable that the user provides.

This means that it works well with `EntityFrameworkCore <https://docs.microsoft.com/en-us/ef/core/>`_, and similar frameworks that use *IQueryable* to create their final data queries.

Configuration
--------------

To use the queryable resource authorization, the policy must be configured with an unknown data name which is used when writing the rego policy for OPA.

Example on how to configure a policy with an unknown data name:

.. code-block:: csharp

  public class Startup
  {
    ...

    public void ConfigureServices(IServiceCollection services)
    {
      ...

      // Configure OPADotNet
      services.AddOpa(opt => ...);

      services.AddAuthorization(opt =>
      {
        opt.AddPolicy("read", x => x.RequireOpaPolicy("example.read", "dataname"));
      });
    }
  }

In the example above, the *AspNetCore* policy *read* was configured to use an OPA policy named *example.read* and there is an unknown data source called *dataname*.

This then allows us to write the following rego policy:

.. code-block::

  package example.read

  allow {
    #dataname here matches the configuration in startup.cs
    data.dataname[_].owner = input.subject.name
  }

The *data.dataname* does not have to exist in the OPA store, and will be used to translate the OPA partial query into a C# expression tree.

Usage
------

Now when the policy has been configured, it is possible to use it to authorize queryable resources with it.

All resource based authorization uses the *IAuthorizationService* and should be collected using dependency injection (it is added during *AddAuthorization*).

Example:

.. code-block:: csharp

  public class ValuesController : ControllerBase
  {
    private readonly IAuthorizationService _authorizationService;
    private readonly TestContext _dbContext;

    // Get auth service and an EntityFrameworkCore DbContext
    public ValuesController(IAuthorizationService authorizationService, TestContext dbContext)
    {
        _authorizationService = authorizationService;
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      //Call AuthorizeQueryable to append the query required to only see resources the user can see.
      var authResult = await _authorizationService.AuthorizeQueryable(
        HttpContext.User, 
        // The queryable you want to authorize
        _dbContext.Reports,
        // The policy name configured in Startup.cs
        "read");

      // Check if the user is not authorized at all to see any data
      if (!authResult.Succeeded)
      {
          return Unauthorized();
      }

      // authResult.Queryable contains the filtered queryable.
      return Ok(authResult.Queryable);
    }
  }
