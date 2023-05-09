$insightsConnection=$(az resource show -g sample-rg -n insights-for-sample --resource-type "microsoft.insights/components" --query properties.InstrumentationKey --output tsv)

az containerapp up `
  --name frontend `
  --resource-group sample-rg `
  --location westeurope `
  --environment sample-env `
  --target-port 80 `
  --ingress internal `
  --env-vars WEBSITE_HOSTNAME=frontend APPLICATIONINSIGHTS_CONNECTION_STRING="$insightsConnection" `
  --source .