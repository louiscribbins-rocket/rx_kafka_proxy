$proxy_namespace = 'nonprod-ccloud-proxy'

Write-Output "Getting pod names for namespace: $proxy_namespace"

# get just the list of names for the pods in our namespace
$pods = kubectl get pods --no-headers -o custom-columns=":metadata.name" -n $proxy_namespace

$target_pod = $pods
if ($pods -is [object[]] -and $pods.Length -gt 1)
{
	$target_pod = $pods[0]
}

if ($target_pod -is [string])
{
	Write-Output ""
	Write-Output "Starting proxy with the following command"
	Write-Output "    kubectl port-forward -n $proxy_namespace $target_pod 443:8443 9092:9092"
	Write-Output ""
	Write-Output "Be sure you see FOUR proxy mapping lines in your output. If not check the README.md file."
	Write-Output ""
	Write-Output ""
	kubectl port-forward -n $proxy_namespace $target_pod 443:8443 9092:9092
}
else
{
	Write-Output "Failed to get pod names!"
	Write-Output $pods
}
