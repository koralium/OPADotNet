package mvcexample.delete

# Checks if a user can delete a resource during the delete operation

allow {
  # Current user must be the owner
  input.resource.owner = input.subject.name
}