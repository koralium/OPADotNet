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
  some i
  input.operation = "PUT"

  data.securedata[i].name = input.resource.Name

  # Check that the owner has not changed
  input.resource.Owner = data.securedata[i].owner

  # Check that the user is the owner of the previous object
  input.subject.name = data.securedata[i].owner
}

allow {
  input.operation = "CAN_DELETE"

  # Current user must be the owner of the object to delete
  data.securedata[_].owner = input.subject.name
}

allow {
  input.operation = "DELETE"

  # Current user must be the owner
  input.resource.Owner = input.subject.name
}