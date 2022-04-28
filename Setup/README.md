# General
This folder contains the needed files and instructions to set up the resources needed for benchmarking.
The following resource types need to be created:
- Virtual Machine (Data Generation)
- IoT Hub
- Databases 
- Azure Functions
Each folder contains instructions on how to set these up.

The Azure Functions, that insert the messages into the databases and the ones, that query the database also need to be created. Please follow the respective READMEs for the setup:
[Ingress](../IngressFunctions/README.md) & [Egress](../EgressFunctions/README.md)


# Deploy Resources
In order to deploy the resources, the Bicep files are necessary. 
Recommended Deployment is via the [Azure CLI](https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/deploy-cli). The specific commands used are described in the folders README files.

# Create Resource Group
For creating a Resource Group use the following command
```bash
az group create --location <Location> --name <ResourceGroupName> [--managed-by] [--subscription] [--tags]
```