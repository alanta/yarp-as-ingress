Azure Container Apps (ACA) needs an external service to do URL routing. In kubernetes this is usually handled by an Ingress.
ACA uses [Envoy](https://www.envoyproxy.io) under the hood to handle web traffic, which is great, but it's URL routing functionality is not exposed.

This repo demonstrates how to implement URL routing with YARP, so you get all the flexibility you normally have with ingresses in K8S.

This demo is loosly based on this [Azure sample](https://github.com/Azure-Samples/container-apps-connect-multiple-apps/tree/main/with-fqdn). 

### Disclaimer

This is not a full-fledged production ready sample, it's focussed on demonstrating what _could_ be done with Yarp in Azure Container Apps.
For a high traffic site you'll probably want to host your front-end on a service that supports edge caching.

For less demanding applications though, the setup presented here could save quite a bit of money and reduce complexity of the required infrastructure.

## Architecture

The application has 3 deployments, with 1 container each. The _public_ deployment has the Yarp container in it, which delegates traffic by url to the API or the front-end.

<img src="./Docs/Network architecture.svg"/>

## Goals

The goal of this project is to have a play with Azure Container Apps and get some hands-on experience with:

* Configuring traffic into the Container App Environment
* Connecting apps within an environment
* Auto-scaling with network traffic
* Routing traffic from external to internal apps

ACA already does a bunch of things that most apps need:
* CORS
* IP filtering
* HTTPS termination

Just like K8S and AKS, you can reach other containers directly using their deployment name. So, within the Conainer Environment, `http://frontend` resolves to the `frontend` deployment.

### What's missing

The main missing feature for me is path based routing and URL rewriting. This is where YARP comes in. It's dotnet based reverse proxy.
For dotnet devs it's really easy to use and configure, and it's pretty fast too.

### Bonus : API authorization
To make it even more interesting, I've implemented HMAC request signing in YARP. Here, it's implemented as an authentication method.
The idea is that if you can implement this kind of functionality within the cluster, there's less need for pricy external resources like API Management.
This type of authorization is best suited for machine-to-machine connections, for example to validate incoming webhooks.

## Running locally

You'll need:
* [Dotnet 7](https://dotnet.microsoft.com/en-us/download)

A dev environment is handy too:
* [Visual Studio Code](https://code.visualstudio.com/download) 
* [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/) 

Start each of the projects, either from the development environment or by running `dotnet run`.
Navigate to the YARP endpoint at [https://localhost:7193](https://localhost:7193).

There's a [LoadTester](./LoadTester) tool that will hit the API and the frontend.

In the [node](./node) folder there's a small test script showing how to invoke the API from another tech stack.

## Deploying to Azure

You'll need:
* [PowerShell Core](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows?#msi) 
* [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli)

### Deploying the infrastructure

Before rolling out the apps, we need to have some infrastructure in place. We'll roll this out using Azure CLI.

First, make sure you're logged in and have the correct subscription selected:

```pwsh
# Login to the CLI
az login
az extension add -n containerapp

# Ensure you have the right subscription selected
az account show
```

Now deploy the Container Apps environment and supporting services.

```pwsh
# Setup preferences
$location="westeurope"

# Ensure the required provider is registered in the subscription
az provider register --namespace Microsoft.Web

# Create a resource group
az group create `
  --name 'sample-rg' `
  --location $location

az monitor log-analytics workspace create  `
  --resource-group 'sample-rg' `
  --workspace-name 'logs-for-sample'

az monitor app-insights component create ` 
  --app insights-for-sample ` 
  --location $location `
  --resource-group 'sample-rg'

$LOG_ANALYTICS_WORKSPACE_CLIENT_ID=$(az monitor log-analytics workspace show --query customerId -g 'sample-rg' -n 'logs-for-sample' --out tsv)
$LOG_ANALYTICS_WORKSPACE_CLIENT_SECRET=$(az monitor log-analytics workspace get-shared-keys --query primarySharedKey -g 'sample-rg' -n 'logs-for-sample' --out tsv)

# Create a container app environment
az containerapp env create `
  --name 'sample-env' ` 
  --resource-group 'sample-rg' `
  --logs-workspace-id $LOG_ANALYTICS_WORKSPACE_CLIENT_ID `
  --logs-workspace-key $LOG_ANALYTICS_WORKSPACE_CLIENT_SECRET `
  --location $location
```

### Deploying the container apps

Each individual project can be deployed using the `deploy.ps1` script in the respective apps's folder.
On the first deployment, the container app tooling will create a container registry. This will happen only once.