az containerapp up `
  --name public `
  --resource-group sample-rg `
  --location westeurope `
  --environment sample-env `
  --target-port 80 `
  --ingress external `
  --source .