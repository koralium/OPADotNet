Input values
================

When any Authorize command is ran, the following values are added to the input:

.. csv-table:: Input values
  :file: inputvalues.csv
  :delim: ;
  :header: "Value", "Description"

Important to note is that all objects are camelCase serialized, so a property under resource for instance will be in camelCase.

Request Route Values
---------------------

*input.request.routeValues* can contain useful information that does not exist in the *path* variable. An example structure can look as following:

.. code-block:: json

  {
    "controller": "Data",
    "action": "Index",
    "id": 123
  }

In the example above one could do the following check: :code:`input.request.routeValues.id == 123`

Request Query
-------------

*input.request.query* is implemented as a dictionary with a list of values. Given the following url: :code:`http://example.com?test=1&test=2` the value would be:

.. code-block:: json

  {
    "test": [
      "1",
      "2"
    ]
  }