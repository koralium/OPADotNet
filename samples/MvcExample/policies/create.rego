package mvcexample.create

# This policy handles if a user can create a certain resource

allow {
  input.resource.owner = input.subject.name
}