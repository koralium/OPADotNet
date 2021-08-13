package mvcexample.update

# Check if a user is allowed to complete an update operation of a resource.

allow {
  r = data.securedata[_]

  r.name = input.resource.Name

  # Check that the owner has not changed
  input.resource.owner = r.owner

  # Check that the user is the owner of the previous object
  input.subject.name = r.owner
}