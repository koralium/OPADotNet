Input Constants
================

When any Authorize command is ran, the following constants are added to the input:

* **input.subject** - The current user
* **input.subject.name** - The current user name, translated from claimsPrincipal.Identity.Name
* **input.subject.claims[]** - An array of the current user claims
* **input.subject.claims[_].type** - The type that the claim is, for example "role".
* **input.subject.claims[_].value** - The value of the claim.
* **input.operation** - The operation name set when configuring the policy.

When doing a single resource authorization (not queryable), the following input is also added:

* **input.resource** - This contains the resource that is being authorized.
