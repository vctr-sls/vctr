#!/bin/bash

echo "Running Gateway..."
dotnet run --project Gateway -- --iscirun=true > /dev/null 2>&1 &
GATEWAY_PID=$!

for i in {1..5}; do
    echo "Timeout for 30s..."
    sleep 30
    echo "Fetch swagger.json - Attempt: $i/5"
    DATA=$(curl http://localhost:5000/swagger/v1/swagger.json) \
        && echo $DATA > swagger.json \
        && break
    [ $i == 5 ] \
        && echo "ERROR: Failed fetching swagger.json" \
        && exit 1
done

echo "Killing Gateway with PID $GATEWAY_PID..."
kill $GATEWAY_PID