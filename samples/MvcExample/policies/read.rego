package mvcexample.read

# This policy regards which resources a user can read

allow {
  data.securedata[_].owner = input.subject.name
}