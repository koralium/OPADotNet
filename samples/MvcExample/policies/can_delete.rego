package mvcexample.can_delete

# Checks if a user can delete a resource or not. Used to show or hide the delete button.

allow {
  # Current user must be the owner of the object to delete
  data.securedata[_].owner = input.subject.name
}