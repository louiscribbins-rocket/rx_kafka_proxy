This code base is meant to simplify and help you to connect to our non-prod kafka cluster.

If you run into connection issues, then run this solution and follow the steps it outlines.

PLEASE NOTE! You **must** have the regular ccloud proxy working.

Run this to get the proxy running:
```
.\ccloud_proxy.ps1
```

To learn more about setting up, managing, and **issues** with proxies to our confluent servers look here: https://git.rockfin.com/Rocket-Exchange/ccloud-proxy

Look at this repo for a simple example of consuming/publishing from kafka using a minimal amount of code: https://git.rockfin.com/lcribbins/kafka-clients
