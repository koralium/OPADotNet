package mvcexample.read

# This policy regards which resources a user can read

allow {
  allowed[_]
}

allowed[secure] {
  secure := data.securedata[_]
  secure.owner == input.subject.name
}