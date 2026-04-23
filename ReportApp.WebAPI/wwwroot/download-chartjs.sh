#!/bin/bash
# Run this script once to download the real Chart.js
curl -o "$(dirname "$0")/chart.umd.min.js" \
  "https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"
echo "Downloaded chart.umd.min.js"
