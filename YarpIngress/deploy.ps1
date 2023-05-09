az containerapp up `
  --name public `
  --resource-group sample-rg `
  --location westeurope `
  --environment sample-env `
  --target-port 80 `
  --ingress external `
  --env-vars WEBSITE_HOSTNAME=yarpingress APPLICATIONINSIGHTS_CONNECTION_STRING="InstrumentationKey=753abc0d-3283-4a74-b653-c7a1c5efc31e;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/" `
  --source .