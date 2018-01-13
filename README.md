# Plivo Coding Assignment

## Getting Started

These instructions explains the current infrastructure deployment of the project and steps to test the application. It will also list down the steps to setup and install the application on AWS cloud platform

## Deployed Infrastruture and Testing
### API

The project consist of 2 Web API deployment over AWS. The SMS APIs are hosted at below location
```
http://plivosms-dev.us-west-2.elasticbeanstalk.com
```
```
Endpoints: http://plivosms-dev.us-west-2.elasticbeanstalk.com/swagger
```
Cache APIs are hosted at below location
```
http://smscache-dev.us-west-2.elasticbeanstalk.com
```
```
Endpoints: http://smscache-dev.us-west-2.elasticbeanstalk.com/swagger
```

### Database
Microsoft SQL Server database is hosted on AWS RDS. Below are the database credentials to connect
```
instance: plivo-sms.ckygzynow2v2.us-west-2.rds.amazonaws.com
username: plivoadmin
password: admin123
```

### Testing
#### Prerequisites

```
dotnet test command utility provided by .NET Core 1.x
[Ref: https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-test?tabs=netcore1x]
```
To run the unit/integration test for SMS APIs, follow the instruction below
```
Download the master branch of the project on your machine
```
```
Navigate to SMS.Tests directory in command prompt and run the below command
```
```
$:> dotnet test
```

Test Cache APIs from the swagger link provided below
```
http://smscache-dev.us-west-2.elasticbeanstalk.com/swagger
```

## AWS Deployment Steps
Follow below instructions to deploy on AWS
```
https://docs.aws.amazon.com/toolkit-for-visual-studio/latest/user-guide/deployment-beanstalk-netcore.html
```
## Authors
Akhilesh Singh
