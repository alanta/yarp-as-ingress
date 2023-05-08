az containerapp up `
  --name frontend `
  --resource-group sample-rg `
  --location westeurope `
  --environment sample-env `
  --target-port 80 `
  --ingress internal `
  --source .