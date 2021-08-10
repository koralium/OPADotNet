AutoMapper Support
===================

OPADotNet has support for mapping policy results into boolean variables with the help of `AutoMapper <https://automapper.org/>`_ and the nuget package "OPADotNet.Automapper".

This can be used for scenarios where one wants to create an API to get the current users permission on an object/objects.

Setup
----------------------

To start using the AutoMapper support, the nuget package *OPADotNet.Automapper* must be installed. Afterwards it should be added while configure services:

.. code-block:: csharp

  // Configure AutoMapper first since OPADotNet adds a decorator infront of AutoMapper.
  services.AddAutoMapper(...);

  services.AddOpa(opt =>
  {
      // Add auto mapper support
      opt.AddAutomapperSupport();
      ...
  });

It is important that AutoMapper is configured before adding OPA AutoMapper support since it adds a decorator infront of *IMapper*.

Custom user provider
*********************

The default user provider uses the current ClaimsPrincipal from the HttpContext. It is possible to override this behaviour by implementing the interface *IOpaAutoMapperUserProvider*.

Example:

.. code-block:: csharp

  class CustomUserProvider : IOpaAutoMapperUserProvider
  {
      public ClaimsPrincipal GetClaimsPrincipal()
      {
          //Return a custom user provider here
          return new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
          {
              new Claim("name", "test")
          }));
      }
  }

To use the custom provider, OPA is configured in the following way:

.. code-block:: csharp

  services.AddOpa(opt =>
  {
      // Add auto mapper support
      opt.AddAutomapperSupport<CustomUserProvider>();
      ...
  });

The provided class is added as a scoped service.

Configure an AutoMapper mapping
---------------------------------

There are two different methods that can be used when configuring the AutoMapper mappings:

* **MapFromPolicy** - Maps a value from a policy name, uses the source object on the Policy. So if one maps from type A to type B, type A will be used with the policy.
* **MapFromPolicyBasedOnDestination** - Maps a value from a policy name, but instead uses the destination object on the Policy.

Example for MapFromPolicy:

.. code-block:: csharp

  // Policy
  package test
                            
  allow {
      data.testdata[_].dbname = ""test""
  }

  //Classes
  private class DbModel
  {
      public string DbName { get; set; }
  }

  private class Model
  {
      public string Name { get; set; }

      public bool PolicyValue { get; set; }
  }

  // AutoMapper mapping
  CreateMap<DbModel, Model>(AutoMapper.MemberList.Destination)
    .ForMember(x => x.Name, opt => opt.MapFrom(src => src.DbName))
    .ForMember(x => x.PolicyValue, opt => opt.MapFromPolicy("test"))

In the example above, the property names of the DbModel will be used and also the values in the DbModel.

Example for MapFromPolicyBasedOnDestination:

.. code-block:: csharp

  // Policy
  package test
                            
  allow {
      data.testdata[_].name = ""test""
  }

  //Classes
  private class DbModel
  {
      public string DbName { get; set; }
  }

  private class Model
  {
      public string Name { get; set; }

      public bool PolicyValue { get; set; }
  }

  // AutoMapper mapping
  CreateMap<DbModel, Model>(AutoMapper.MemberList.Destination)
    .ForMember(x => x.Name, opt => opt.MapFrom(src => src.DbName))
    .ForMember(x => x.PolicyValue, opt => opt.MapFromPolicyBasedOnDestination("test"))

In the example above the destination model will be used, in ths case *Model*. 

Usage
-----

There are two main ways to use the AutoMapper support:

* **Map in memory** - Run authorization against an object in memory.
* **ProjectTo** - Create an expression tree to evaluate policies with an IQueryable.

Map in memory
**************

Mapping in memory is a useful case if you have few objects that should be evaluated and mapped.
But it runs a partial evaluation against every object which can cause a delay if there are many objects.

Example:

.. code-block:: csharp

  // AutoMapper mapping
  CreateMap<DbModel, Model>(AutoMapper.MemberList.Destination)
    .ForMember(x => x.Name, opt => opt.MapFrom(src => src.DbName))
    .ForMember(x => x.PolicyValue, opt => opt.MapFromPolicyBasedOnDestination("test"))

  // Usage
  var mappedModel = mapper.Map<Model>(databaseModel);

The property PolicyValue will now contain the result if the user passed the policy "test" on that specific object or not.

ProjectTo
**********

ProjectTo is an AutoMapper functionality that allows the mappings to go into a database query or similar to do as much of the work as possible in the database.
This is also possible with the AutoMapper support in OPADotNet. This allows the policies to be evaluated in in the database query.

This usecase is useful if one wants to evaluate alot of objects at once. For instance allowing the user to pass in a filter to only see objects that they can edit.


Example:

.. code-block:: csharp

  // AutoMapper mapping
  CreateMap<DbModel, Model>(AutoMapper.MemberList.Destination)
    .ForMember(x => x.Name, opt => opt.MapFrom(src => src.DbName))
    .ForMember(x => x.PolicyValue, opt => opt.MapFromPolicyBasedOnDestination("test"))

  // Usage
  var mappedQueryable = mapper.ProjectTo<Model>(dbContext.Data);

