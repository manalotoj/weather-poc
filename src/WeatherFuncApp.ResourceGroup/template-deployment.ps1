#
# template_deployment.ps1
#
$rg = "weatherfunctionapp"
$location = "westus"
$TemplateFile = "azuredeploy.json"
$TemplateParametersFile = "azuredeploy.parameters.json"

$deploymentName = $rg + '_' + ((Get-Date).ToUniversalTime()).ToString('MMdd-HHmm')

New-AzureRmResourceGroupDeployment -Name $deploymentName `
                                    -ResourceGroupName $rg `
                                    -TemplateFile $TemplateFile `
                                    -TemplateParameterFile $TemplateParametersFile `
                                    -Force -Verbose `
                                    -ErrorVariable ErrorMessages

az group deployment show -g $rg -n $deploymentName --query properties.outputs.resourceID.value

$accountName = "weatherfuncapp"
$databaseName = "default"
$collectionName = "weather"

# Create a database 
$exists = az cosmosdb database exists `
	--name $accountName `
	--db-name $databaseName `
	--resource-group $rg

if (!$exists) {
    az cosmosdb database create `
	    --name $accountName `
	    --db-name $databaseName `
	    --resource-group $rg
}

# Create a collection
$exists = az cosmosdb collection exists `
	--collection-name $collectionName `
	--name $accountName `
	--db-name $databaseName `
	--resource-group $rg
$exists

if (!$exists) {
    az cosmosdb collection create `
	    --collection-name $collectionName `
	    --name $accountName `
	    --db-name $databaseName `
	    --resource-group $rg
}