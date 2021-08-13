package mvcexample.can_edit

# Check if a user can start to edit a resource

allow {
  # Check that the user is the owner
  input.subject.name = data.securedata[_].owner
}