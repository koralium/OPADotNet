package mvcexample

allow {
  input.operation = "POST"
  input.resource.Owner = input.subject.name
}

allow {
  input.operation = "GET"
  data.securedata[_].owner = input.subject.name
}

allow {
  input.operation = "PUT"

  # Check that the owner has not changed
  input.resource.Owner = data.securedata.owner

  # Check that the user is the owner of the previous object
  input.subject.name = data.securedata.owner
}

allow {
  input.operation = "DELETE"

  # Current user must be the owner
  input.resource.Owner = input.subject.name
}