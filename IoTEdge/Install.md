# Functions at the Edge

* `Install-WindowsFeature "Containers","Hyper-V"`
* Install DotNet Core SDK: https://download.microsoft.com/download/3/7/1/37189942-C91D-46E9-907B-CF2B2DE584C7/dotnet-sdk-2.1.200-win-x64.exe
* Install VSCode: https://go.microsoft.com/fwlink/?Linkid=852157
* Install Docker for Windows: https://download.docker.com/win/stable/Docker%20for%20Windows%20Installer.exe
* Install Python: https://www.python.org/ftp/python/2.7.15/python-2.7.15.amd64.msi
  * Remeber to add Python to Path!  
* `pip install -U azure-iot-edge-runtime-ctl`   

## Edge Setup
   
* Configure IoTEdge: 
```
iotedgectl setup --connection-string "{device connection string}" --nopass
iotedgectl start
```

## Edge Demo Modules

* Temp sensor Image URI:  
`microsoft/azureiotedge-simulated-temperature-sensor:1.0-preview`

* Nginx Image URI:  
`nginx:latest`
  * Create options:  
  `{"HostConfig":{"PortBindings":{"80/tcp":[{"HostPort":"8080"}]}}}`

## Edge Functions Setup

* Install Azure Funcitons Edge Project:  
`dotnet new -i Microsoft.Azure.IoT.Edge.Function`

* Create New Functions Edge Project:  
`dotnet new aziotedgefunction -n YourNameHere -r registryaddress/yournamehere`

function.json:
```
{
    "disabled": false,
    "bindings": [
        {
            "authLevel": "anonymous",
            "name": "req",
            "type": "httpTrigger",
            "direction": "in",
            "methods": [
                "get"
            ]
        },
        {
            "name": "$return",
            "type": "http",
            "direction": "out"
        }
    ]
}
```

run.csx:
```
using System.Net;

public static string Run(HttpRequestMessage req, TraceWriter log)
{
  log.Info("C# HTTP trigger function processed a request.");

  return "Hello World!";
}
```

Dockerfile:
```
FROM microsoft/azureiotedge-functions-binding:1.0-preview

EXPOSE 80

ENV AzureWebJobsScriptRoot=/app

COPY . /app
```

* Login to our ACR:  
`docker login -u <ACR username> -p <ACR password> <ACR login server>`

* Build and push
```
docker build -t yournamehere .
docker tag yournamehere:latest registryaddress/yournamehere
docker push registryaddress/yournamehere
```

* Setup IoT edge with access to our ACR  
`iotedgectl login --address <your container registry address> --username <username> --password <password>`

* Add the module to our edge with Image URI:  
`registryaddress/yournamehere:latest`
  * Create options:  
  `{"HostConfig":{"PortBindings":{"80/tcp":[{"HostPort":"7007"}]}}}`

* Test your function:
`http://localhost:7007/api/HttpExample`