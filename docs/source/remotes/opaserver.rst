Execute on OPA Server
=====================

One of the major strengths of OPADotNet is that it runs all the policies embedded in the application which provides better performance.
But it is possible to have all policy checks to be done on a remote OPA server instead.

This is done by setting the following configuration:

.. code-block:: csharp

  services.AddOpa(opt =>
    opt.UseOpaServer("https://pathtoopa")
  );

When using a remote OPA server, the other configurations such as syncs, discovery etc will not work since they require the embedded OPA.

.. _externalindev:

External OPA Server in development
-----------------------------------

It may be useful to execute on an external *OPA Server* during development since data does not have to be synced on each debug session.
This pairs very well with using the :doc:`opaserversync` to have better performance in production.

To configure this you can add:

.. code-block:: csharp

  public class Startup
  {
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        CurrentEnvironment = env;
    }

    public IConfiguration Configuration { get; }
    private IWebHostEnvironment CurrentEnvironment{ get; }

    public void ConfigureServices(IServiceCollection services)
    {
      ...

      var url = Configuration.GetValue<string>("OpaServerUrl");
      services.AddOpa(opt =>
      {
        if (CurrentEnvironment.IsDevelopment())
        {
            opt.UseOpaServer(url);
        }
        else
        {
            opt.AddSync(sync => sync.UseOpaServer(url));
        }
      });

      ...
    }
  }

  