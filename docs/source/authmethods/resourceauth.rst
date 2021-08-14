Authorize a resource
=====================

This use case can be used when one wants to authorize if a user can for instance add a new resource, or get a specific one.
It allows the caller to check an in-memory resource against a policy.

There are three different ways to authorize a resource, using *input.resource*, *data.{dataName}*, or a combination of the two to compare two resources (can be useful in update scenarios).

Using *input.resource*
----------------------


If you want to write your policy using *input.resource* then the following configuration in Startup.cs is enough:

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
        opt.AddPolicy("create", x => x.RequireOpaPolicy("example.create"));
      });
    }
  }

Example policy:

.. code-block:: 

  package example.create

  allow {
    # Property names under your resource class will be usuable under resource
    input.resouce.owner = input.subject.name
  }

Resource model class:

.. code-block:: csharp

  public class Model {
    public string Name { get; set; }
    public string Owner { get; set; }
  }

.. note::
  All resource based authorization uses the *IAuthorizationService* and should be collected using dependency injection (it is added during *AddAuthorization*).

Example on how to authorize an object:

.. code-block:: csharp

  public class ValuesController : ControllerBase
  {
    private readonly IAuthorizationService _authorizationService;

    public ValuesController(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody]Model data)
    {
        var authResult = await _authorizationService.AuthorizeAsync(HttpContext.User, data, "create");

        if (!authResult.Succeeded)
        {
            return Unauthorized();
        }

        return Ok();
    }
  }

Example usage can be found in `MvcExample <https://github.com/koralium/OPADotNet/blob/8f4cbebad743bc27a884c05067dae3eb9affbd2e/samples/MvcExample/Controllers/DataController.cs#L91>`_.

Using an unknown data name
---------------------------

Authorizating a resource using an unknown data name is similar to :ref:`querydataauth` regarding setup. 

Example usage is if you want to check if a user can see a specific resource or not.

Startup.cs:

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

Policy:

.. code-block::

  package example.read

  allow {
    #dataname here matches the configuration in startup.cs
    data.dataname[_].owner = input.subject.name
  }

Authorization check:

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

    public async Task<ActionResult> Details(string id)
    {
      var resource = _dataDbContext.Data.FirstOrDefault(x => x.Name == id);

      if (resource == null)
      {
        return NotFound();
      }

      var authResult = await _authorizationService.AuthorizeAsync(User, resource, "read");

      if (!authResult.Succeeded)
      {
        return Forbid();
      }

      return View(resource);
    }
  }

Compare two resources
----------------------

Comparing two resources can be useful in scenarios such as updating a resource, this allows the OPA policy to have access to both the updated version and the old version.
For instance consider the following policy:

.. code-block::

  package example.update

  allow {
    some i
    # User is owner of the existing object
    data.dataname[i].owner = input.subject.name

    # User is not allowed to change the owner field. 
    data.dataname[i].owner = input.resource.owner
  }

In this policy the user must be the owner of the current object, but the new object must also have the same owner. So this user is not allowed to change ownership.

Startup.cs:

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
        // We use the policy defined above
        opt.AddPolicy("read", x => x.RequireOpaPolicy("example.update", "dataname"));
      });
    }
  }

Authorization check:

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(string id, [FromForm] Model dataModel)
    {
      try
      {
        var old = _dataDbContext.Data.FirstOrDefault(x => x.Name == id);

        if (old == null)
        {
            return NotFound();
        }

        //Authorize with different input and data object
        var authResult = await _authorizationService.AuthorizeAsync(User, dataModel, old, "update");

        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        // Update the database object here with new values
        ...

        // Save changes
        await _dataDbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
      }
      catch
      {
        return View();
      }
    }
  }
