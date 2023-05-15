$insightsConnection=$(az resource show -g sample-rg -n insights-for-sample --resource-type "microsoft.insights/components" --query properties.ConnectionString --output tsv)

az containerapp up `
  --name api `
  --resource-group sample-rg `
  --location westeurope `
  --environment sample-env `
  --target-port 80 `
  --ingress internal `
  --env-vars WEBSITE_HOSTNAME=api APPLICATIONINSIGHTS_CONNECTION_STRING="$insightsConnection" `
  --source .