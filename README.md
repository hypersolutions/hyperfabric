# HyperFabric

## Getting Started

#### Installing the Tool

The tool is installed as a dotnet global line tool via NuGet:

```bash
dotnet tool install HyperFabric.Client --global
```

**Note** that this installs the latest. Add _--version X.Y.Z_ for a specific version.

#### Run the Tool

You run the tool at the command line:

```bash
hyperfabric --json "c:\temp\manifest1.json" --loggers "Console"
```

**Note** that you can also add a _File_ logger as well. Example:

```bash
hyperfabric --json "c:\temp\manifest1.json" --loggers "Console,File"
```

gives you both console and file output. The file is written to the current directory as _out.json_.

There are only two options - the json and logging option. For more information, type:

```bash
hyperfabric --help
```

#### Uninstall the Tool

Uninstall the tool from the command line:

```bash
dotnet tool uninstall HyperFabric.Client --global
```

### Overview

### Deployment Manifest

The deployment of applications into service fabric is achieved via a _Deployment Manifest_. This is a _json_ structure that contains a list of packages to be deployed, and in what order.

Ordering is handled via _groups_. Any package within a group is deemed suitable for parallel deployment. The order you place the _groups_ in, determines the deployment order.

There is a section for the cluster details. The _connection_ is required. If you are connecting to a cluster over https then you will need to provide certificate details such as a 
thumbprint or common name and the appropriate search option.

They are also common deployment _options_ such as _NumberOfParallelDeployments_ or _CheckClusterHealthWaitTime_. These have defaults which are detailed below.

#### Manifest Information

##### Options

* **NumberOfParallelDeployments** determines how many actual parallel deployments can take place at any one time. If a group contains more packages than this value then the tool 
will start new deployments as threads become available. This is optional and has a default of 5 but is bounded with a range of 1 to 10.
* **CheckClusterHealthWaitTime** determines the amount of time in seconds to wait for the cluster to become healthy during health checks. Optional with a default of 30 seconds.
* **WorkingDirectory** is the local directory where packages are copied to before being uploaded. Optional with a default of the current working directory where the tool runs.

##### ClusterDetails

* **Connection** is the connection to the cluster such as _http://localhost:19080_. A required value.
* **FindByValue** is used to search for a certificate in the store when connecting to a secured cluster. Required if connecting over https.
* **Location** is the store where the certificate resides. These match the standard .NET enum _StoreLocation_ with values like _LocalMachine_. Defaults to _CurrentUser_.  
* **FindBy** is the lookup to use in conjunction with the _FindByValue_. The values match the .NET _X509FindType_ enum with values like _FindByThumbprint_ or _FindBySubjectName_. 
Defaults to _FindByThumbprint_. **Note** that you can omit the _FindBy_ text e.g. _FindByThumbprint_ becomes _Thumbprint_.

##### Groups

These are containers for packages that can be deployed in parallel. Once one group is complete, it will move onto the next group. The packages themselves are defined in the _Items_ section.

##### Items

List of packages for the group to be deployed.

* **PackagePath** is the path to the package. A required field.
* **ParameterFile** is the path to the cloud parameter file for the package with its settings for deployment. A required field.
* **RemoveApplicationFirst** sets a flag indicating if the existing application type should be removed before deployment. Optional and has a default of false.
* **CompressPackage** sets a flag indicating if the package should be compressed before copying to the image store. Optional and has a default of false.
* **MaxApplicationReadyWaitTime** determines the amount of time (in seconds) to wait for an application to be ready once deployed. This is an optional, 
bounded value of between 5 and 300 seconds, default of 10.
* **UpgradeMode** determines how the cluster monitors an upgrade. This is optional but if upgrading, it will use UnmonitoredAuto unless set with one of 
the UnmonitoredAuto, UnmonitoredManual and Monitored values.

### Examples

#### Single Package

```json
{
  "clusterDetails": {
    "connection": "http://localhost:19080"
  },
  "groups": [
    {
      "items": [
        {
          "packagePath": "c:\\temp\\app1",
          "parameterFile": "C:\\temp\\app1\\cloud.xml",
          "removeApplicationFirst": true
        }
      ]
    }
  ]
}
```

The above example will deploy a single package to an unsecured cluster. If the application type exists then it will be removed first - along with its applications.

#### Parallel Package Deployment

```json
{
  "options": {
    "numberOfParallelDeployments": 2,
    "CheckClusterHealthWaitTime": 45
  },
  "clusterDetails": {
    "connection": "http://localhost:19080"
  },
  "groups": [
    {
      "items": [
        {
          "packagePath": "c:\\temp\\app1",
          "parameterFile": "c:\\temp\\app1\\cloud.xml",
          "removeApplicationFirst": true
        },
        {
          "packagePath": "c:\\temp\\app2",
          "parameterFile": "c:\\temp\\app2\\cloud.xml",
          "removeApplicationFirst": true
        },
        {
          "packagePath": "c:\\temp\\app3",
          "parameterFile": "c:\\temp\\app3\\cloud.xml",
          "removeApplicationFirst": true
        },
        {
          "packagePath": "c:\\temp\\app4",
          "parameterFile": "c:\\temp\\app4\\cloud.xml",
          "removeApplicationFirst": true
        },
        {
          "packagePath": "c:\\temp\\app5",
          "parameterFile": "c:\\temp\\app5\\cloud.xml",
          "removeApplicationFirst": true
        }
      ]
    }
  ]
}
```

The above will deploy the packages in parallel, albeit honouring the overriden _numberOfParallelDeployments_ value of 2. This is useful so avoid using too much resource on your deployment cluster.

#### Secured Cluster Deployment

```json
{
  "clusterDetails": {
    "connection": "https://localhost:19080",
    "findBy": "Thumbprint",
    "findByValue": "69DDF57D6CC2795A6FCAAA26EE8EDC11866E275F",
    "location": "LocalMachine"
  },
  "groups": [
    {
      "items": [
        {
          "packagePath": "c:\\temp\\app1",
          "parameterFile": "C:\\temp\\app1\\cloud.xml"
        }
      ]
    }
  ]
}
```

This shows how to setup the secured cluster connection details.

#### Upgrading an Application

Given an existing application has been deployed:

```json
{
  "clusterDetails": {
    "connection": "http://localhost:19080"
  },
  "groups": [
    {
      "items": [
        {
          "packagePath": "c:\\temp\\app1",
          "parameterFile": "C:\\temp\\app1\\cloud.xml"
        }
      ]
    }
  ]
}
```

The following will upgrade the application to a new version:

```json
{
  "clusterDetails": {
    "connection": "http://localhost:19080"
  },
  "groups": [
    {
      "items": [
        {
          "packagePath": "c:\\temp\\app1",
          "parameterFile": "C:\\temp\\app1\\cloud.xml",
          "upgradeMode": "Monitored",
          "maxApplicationReadyWaitTime": 180
        }
      ]
    }
  ]
}
```

**Note** that you may have to increase the _MaxApplicationReadyWaitTime_ to a higher value (default is 10sec) as the time to stabilize will depend on the _UpgradeMode_ used.

The available upgrade modes match the UpgradeMode enum (UnmonitoredAuto, UnmonitoredManual and Monitored). In the above example, the upgrade will stop after completing each upgrade domain and automatically monitor health before proceeding.

## Developer Notes

### Building and Publishing

From the root, to build, run:

```bash
dotnet build --configuration Release 
```

To run all the unit and integration tests, run:

```bash
dotnet test --no-build --configuration Release
```

To create a package for the tool, run:
 
```bash
cd src/HyperFabric.Client
dotnet pack --no-build --configuration Release 
```

To publish the package to the nuget feed on nuget.org:

```bash
dotnet nuget push ./HyperFabric.Client/bin/Release/HyperFabric.Client.1.0.0.nupkg -k [THE API KEY] -s "https://api.nuget.org/v3/packages" 
```

#### Installing the Tool

```bash
dotnet tool install HyperFabric.Client --version 1.0.0 --add-source "c:\work\nuget\packages" --global
```

#### Run the Tool

```bash
hyperfabric --json "c:\temp\manifest1.json" --loggers "Console"
```

#### Uninstall the Tool

```bash
dotnet tool uninstall HyperFabric.Client --global
```

#### Local Secure Cluster

To setup your local cluster (windows) as a secured cluster:

```bash
cd "C:\Program Files\Microsoft SDKs\Service Fabric\ClusterSetup"
.\DevClusterSetup.ps1 -PathToClusterDataRoot "c:\SFDevCluster\Data" -PathToClusterLogRoot "c:\SFDevCluster\Log" -AsSecureCluster -CreateOneNodeCluster
```

**Note** the service fabric explorer will not work. Not sure how to get this working!
