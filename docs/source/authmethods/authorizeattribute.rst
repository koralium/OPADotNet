Authorize Attribute
=====================

.. warning::
  Authorize attribute cannot do resource based authorization, and can only validate that a user has access to make the call.

  This means that filtering resources that the user is allowed to see must be done with another authorization method.

The most straight forward way to authorize a user is to use the *Authorize* attribute.
The authorize attribute does not support authorizing users with unknown data, which means that all the data used must be inside OPA.

Example Usage
--------------



in *Startup.cs* we set up *OPADotNet* and configure an authorization policy.

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
        opt.AddPolicy("read", x => x.RequireOpaPolicy("example.read"));
      });
    }
  }

On the class or method that you want to authorize, add the *Authorize* attribute:

.. code-block:: csharp

  // You can use the attribute on a class level
  [Authorize(Policy = "read")]
  public class DataController : Controller
  {
    ...

    // You can also use the attribute on a method 
    [Authorize(Policy = "read")]
    public async Task<ActionResult> Index()
    {
      ...
    }
  }

