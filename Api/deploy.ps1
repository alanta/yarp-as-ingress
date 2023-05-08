az containerapp up `
  --name api `
  --resource-group sample-rg `
  --location westeurope `
  --environment sample-env `
  --target-port 80 `
  --ingress internal `
  --source .