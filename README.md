# Identity-API

This repository contains basic implementation of IdentityApi service (IdentityApi.Host project) and and example of web client (IdentityApi.MvcClient) which corresponds with host service by OpenID Connect and Oauth 2.0 protocols.    
NB: Both host and client project were built with ASP.NET Core MVC over .NET Core 1.1 and can be run using VS 2015 with the latest .NET Core Tools (Preview 2).


## Prerequisites
Here we will describe how to setup and run the examples


### 1. .NET Core 1.1
Download and run appropriate (for your system) installer from [.NET web-site](https://www.microsoft.com/net/download/core#/current) 


### 2.  Azure SDK 
IdentityApi.Host application uses Azure Table Storage to store information about user profiles, API resources, clients (other services), etc.
So you need to install [Azure SDK](https://azure.microsoft.com/en-us/downloads/) for VS 2015 and launch  storage emulator before running the projects locally.   

Here are the commands you need to start storage emulator:

```
cd "c:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator" 
AzureStorageEmulator.exe start
```

### 3.  Startup projects
Both host and client projects should be run in necessary order to make it work properly. To setup that - right click on solution's node (IdentityApi) in Solution Explorer and select "Set StartUp projects..." item there. After that select "Multiple startup projects" option, set "Start" as "Action" for all projects and make "IdentityApi.Host" project to be first in list.
 
NB1: We run IdentityApi.Host app on port `localhost:5000` and IdentityApi.MvcClient on `localhost:5002`. These addresses are hard-coded currently and should be moved to configuration for proper deploying.


NB2: We also included into repository the signing certificate with private key (file IdentityApi.pfx) and the password for this certificate (CERT_PWD envritonment variable in Identity.Host launch settings).  Both these items should be removed from Git repository further by security reasons.


## How to test samples
So, we guess all steps listed in Prerequisites are performed and all projects in IdenityApi solution are build without errors. 

 1. Start the projects in debug mode (F5). Two web-pages should be opened in your default browser: http://localhost:5000 - for host application and http://localhost:5002 - for web client

 2. On web client's page (localhost:5002) click on "User Profile" link in page's header. You will be redirected to login page on host's application.
 
 3. Use the following credentials to sign in:
 
  * email: admin@zumo.org
  * password: admin01
 
 4. After login you will be redirected back to "User Profile" page of client's application. 
 Check all claims associated with current user. You can also check the content of issued access token via [JWT.io](https://jwt.io) web-service.
 

 
## Deployment
On deployment you need to set the following environment variables:
 
### IdentityApi.Host

 * Swagger__XmlDocPath = {path to XMLDoc file} //By default it's `bin\Release\netcoreapp1.1\IdentityApi.Host.xml`
 * CERT_PWD = {password for signing certificate}
 * ConnectionStrings__AzureStorage = {connection string to Azure storage account}
 
### IdentityApi.MvcClient

 * Endpoints__IdentityApi = {path to IdentityApi host}   //it's "http://localhost:5000" by default 
 

**NB**: When publishing to Azure you need can set connection strings separately from other environment variables. In this case you don't need  "ConnectionStrings__" prefix. Use the connection string name only (like "AzureStorage").
 
 
 
**Enjoy!**
 