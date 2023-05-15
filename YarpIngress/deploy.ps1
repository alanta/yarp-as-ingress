$insightsConnection=$(az resource show -g sample-rg -n insights-for-sample --resource-type "microsoft.insights/components" --query properties.ConnectionString --output tsv)

az containerapp up `
  --name public `
  --resource-group sample-rg `
  --location westeurope `
  --environment sample-env `
  --target-port 80 `
  --ingress external `
  --env-vars WEBSITE_HOSTNAME=yarpingress APPLICATIONINSIGHTS_CONNECTION_STRING="$insightsConnection" `
  --source .