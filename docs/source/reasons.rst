Authorization Failed Reasons
=============================

.. note::
  Reasons with AspNetCore will only work on .NET 6 or higher.

  Reasons also only work when using the embedded OPA.

It is possible to use OPADotNet to output reasons to the end-user on why they were unable to be authorized.

This is done first by adding reason text inside of the policy:

.. code-block::

  package reason_test

  # REASON: You must be authorized
  allow { 
    has_scope_test
  }

  # REASON: You must have scope test
  has_scope_test {
    input.subject.claims.scopes[_] = "test"
  }


When adding OPA in the startup class, you must also add:

.. code-block:: csharp

  services.AddOpa(opt => {
    ...
    opt.UseReasons();
    ...
  });

The reason is then given in under the failure object for an authorization check:

.. code-block:: csharp

  var authResult = await _authorizationService.AuthorizeAsync(HttpContext.User, "read");

  if (!authResult.Succeeded)
  {
    // Get the first failure reason message if there is only one requirement (OPA requirement).
    return Unauthorized(authResult.Failure?.FailureReasons.FirstOrDefault()?.Message);
  }

The output from the policy above if the user does not have scope "test" would then be:

.. code-block::

  You must be authorized
  * You must have scope test

Accessing parameters
---------------------

Formatting the reason message with parameters is possible as shown in the following example:

.. code-block::

  allow {
    must_have_scope("test")
  }

  # REASON: You must have scope: {0}
  must_have_scope(scope_name) {
    ...
  }

Which would be translated to: "You must have scope: test"

Limitations
------------

* Reasons will not output correctly for blocks that uses "unknown" data variables. Say if input.subject.name == data.unknown.owner would fail, a reason would not be given correctly.
* Reasons only work with the embedded OPA module, this is because it requires the line numbers in the explanations to locate the correct reasons above a block.
* Failure reasons was added in .NET 6 which means that it is only possible to get the output reason in .NET 6.

Performance impacts
--------------------

Using reasons will imply a small performance impact, since for each policy evaluation the explanation must be extracted from the 
embedded OPA and interpretted to create the final reason message.