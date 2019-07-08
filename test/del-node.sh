#!/usr/bin/env bash
set -eu

curl -v --request DELETE http://localhost:5000/api/nodes/$1
