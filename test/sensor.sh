#!/usr/bin/env bash
set -eu

ts=$(date "+%s")
temp=$(cat /sys/devices/platform/coretemp.0/hwmon/hwmon3/temp1_input)

curl --header "Content-Type: application/json" \
  --request POST \
  --data "{\"Id\":0,\"SensorId\":1,\"Timestamp\":$ts,\"Value\":\"$temp\"}" \
  http://localhost:5000/api/values
echo "$ts: $temp"
