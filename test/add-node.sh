#!/usr/bin/env bash
set -eu

curl -v --header "Content-Type: application/json" \
  --request POST \
  --data '{"Id":0,"Name":"test","Description":"Test node"}' \
  http://localhost:5000/api/nodes
