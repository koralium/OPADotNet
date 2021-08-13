Http Tar.Gz sync
=================

The *Http tar.gz sync* allows an application to fetch policies and data from a tar.gz file located on a http server. 
The implementation tries to stay close to `OPA Server implementation <https://www.openpolicyagent.org/docs/latest/management-bundles/>`__.

To add the tar.gz sync to your application, add the following:

.. code-block:: csharp

  services.AddOpa(opt =>
    .AddSync(s =>
      s.UseHttpTarGz(opt =>
      {
        opt.Url = new Uri("https://yourserver");
      })
    )
  );

Adding headers
---------------

If you need to add custom headers to the request you can do that by adding them to the *Headers* property under the options:

.. code-block:: csharp

  services.AddOpa(opt =>
    .AddSync(s =>
      s.UseHttpTarGz(opt =>
      {
        opt.Url = new Uri("https://yourserver");
        opt.Headers.Add("custom", "value");
      })
    )
  );

OAuth2 Support
---------------

If the service requires an OAuth2 access token, it is possible to configure the sync to authorize against an OAuth2 server.
The only supported grant type at this point is *client_credential*.

Example:

.. code-block:: csharp

  services.AddOpa(opt =>
    .AddSync(s =>
      s.UseHttpTarGz(opt =>
      {
        opt.Url = new Uri("https://yourserver");
        opt.UseOAuth2(auth =>
        {
          auth.ClientId = "clientid";
          auth.ClientSecret = "client secret";
          auth.TokenUrl = "https://tokenurl";
          auth.Scopes = new List<string>() { "scope" };
        });
      })
    )
  );
