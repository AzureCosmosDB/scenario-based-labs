$apiVersion = "2020-03-01"
$resourceGroupName = "iot-lab-2020-test-eus2"
$accountName = "iot-lab-cdb-test-eus2"
$containerResourceType = "Microsoft.DocumentDb/databaseAccounts/sqlDatabases/containers"
$databaseName = "ContosoAuto"

Function Get-ResourceBlock {
	Param
	(
		[String]$containerName,
		[string]$partitionKey,
		[Int]$throughput
	)

	$result = @{
		"resource" = @{
			"id" = $containerName; 
			"partitionKey" = @{
				"paths" = @($partitionKey); 
				"kind" = "Hash"
			}; 
			"indexingPolicy" = @{
				"indexingMode" = "Consistent"; 
			};
			"analyticalStorageTtl" = -1;
		};
		"options"=@{ "Throughput" = $throughput }
	} 

	return $result
}

Function Create-Container {
	Param
	(
		[String]$containerName,
		[string]$partitionKey,
		[Int]$throughput
	)

	$containerResourceName = $accountName + "/" + $databaseName + "/" + $containerName

	Write-Host "Creating container: $containerName"

	$containerProperties = Get-ResourceBlock -containerName $containerName -partitionKey $partitionKey -throughput $throughput

	New-AzResource -ResourceType $containerResourceType `
		-ApiVersion $apiVersion -ResourceGroupName $resourceGroupName `
		-Name $containerResourceName -PropertyObject $containerProperties `
		-Force
}


# metadata
$containername = "metadata"
$partitionkey = "/partitionKey"
$throughput = 50000
create-container -containername $containername -partitionkey $partitionkey -throughput $throughput

# telemetry
$containerName = "telemetry"
$partitionKey = "/partitionKey"
$throughput = 15000
Create-Container -containerName $containerName -partitionKey $partitionKey -throughput $throughput

# maintenance
$containerName = "maintenance"
$partitionKey = "/vin"
$throughput = 400
Create-Container -containerName $containerName -partitionKey $partitionKey -throughput $throughput

