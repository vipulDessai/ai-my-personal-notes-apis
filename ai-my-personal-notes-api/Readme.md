# TODO
- enable cors
    - [serverless httpapi config](https://www.serverless.com/framework/docs/providers/aws/events/http-api)
    - [stack overflow issue cors](https://stackoverflow.com/questions/66000642/httpapi-serverless-framework-api-gateway-cors-not-working)
    - [AWS cors note](https://docs.aws.amazon.com/apigateway/latest/developerguide/http-api-cors.html)
- serverless.yml
    - configs
        - [multiple httpApi route configs](https://forum.serverless.com/t/multiple-request-methods-for-a-single-httpapi-route/15721/5)
- lambda authorizer
    - use password hash (something like bcrypt js)
- Setup git action pipeline for building and deploying the project
    - using node and dotnet - [link](https://docs.github.com/en/actions/automating-builds-and-tests/building-and-testing-net)
- graphql for lambda
    - [Link 1](https://dev.to/memark/running-a-graphql-api-in-net-6-on-aws-lambda-17oc)
- Graph QL setup
    - [Article 1](https://medium.com/@TimHolzherr/creating-a-graphql-backend-in-c-how-to-get-started-with-hot-chocolate-12-in-net-6-30f0fb177c5c) - implementation in progress
    - [Article 2](https://www.c-sharpcorner.com/article/building-api-in-net-core-with-graphql2/) - for reference only

# Open dotnet aws project

## installation
```properties
dotnet new tool-manifest
dotnet tool install amazon.lambda.tools
```

### General installation of packages
```c#
dotnet add package HotChocolate.AspNetCore.Authorization
```

## Deploy
```properties
dotnet lambda package -o dist/ai-my-personal-notes-api.zip

npx serverless deploy
```

## test
```properties
curl "https://<YourUrl>/graphql?query=%7B+sysInfo+%7D"
```


# GraphQL server
```
https://localhost:62926/graphql/
```

## sample query
```
# mutation
mutation addAuthor {
  addAuthor(input: {name: "Schiller"}) {
    record {
      id
      name
    }
  }
}

mutation addBook {
  addBook(input: {
    author: "fadc4809-00d7-48dc-9b1b-51be8d768a6c",
    title: "An die freude"
  }) {
    record {
      id
      title
      author {
        name
      }
    }
  }
}

# query
query books {
  books {
    id
    author {
      name
    }
  } 
}

query author {
  author(input: {
    authorId: "5944c03d-0f4d-4be7-8f6c-35cc7103c12f"
  }) {
    id
    name
  }
}
```

# ASP.NET Core Web API Serverless Application
- [Documentation](https://docs.aws.amazon.com/lambda/latest/dg/csharp-package-asp.html)

This project shows how to run an ASP.NET Core Web API project as an AWS Lambda exposed through Amazon API Gateway. The NuGet package [Amazon.Lambda.AspNetCoreServer](https://www.nuget.org/packages/Amazon.Lambda.AspNetCoreServer) contains a Lambda function that is used to translate requests from API Gateway into the ASP.NET Core framework and then the responses from ASP.NET Core back to API Gateway.


For more information about how the Amazon.Lambda.AspNetCoreServer package works and how to extend its behavior view its [README](https://github.com/aws/aws-lambda-dotnet/blob/master/Libraries/src/Amazon.Lambda.AspNetCoreServer/README.md) file in GitHub.


### Configuring for API Gateway HTTP API ###

API Gateway supports the original REST API and the new HTTP API. In addition HTTP API supports 2 different
payload formats. When using the 2.0 format the base class of `LambdaEntryPoint` must be `Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunction`.
For the 1.0 payload format the base class is the same as REST API which is `Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction`.
**Note:** when using the `AWS::Serverless::Function` CloudFormation resource with an event type of `HttpApi` the default payload
format is 2.0 so the base class of `LambdaEntryPoint` must be `Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunction`.


### Configuring for Application Load Balancer ###

To configure this project to handle requests from an Application Load Balancer instead of API Gateway change
the base class of `LambdaEntryPoint` from `Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction` to 
`Amazon.Lambda.AspNetCoreServer.ApplicationLoadBalancerFunction`.

### Project Files ###

* serverless.template - an AWS CloudFormation Serverless Application Model template file for declaring your Serverless functions and other AWS resources
* aws-lambda-tools-defaults.json - default argument settings for use with Visual Studio and command line deployment tools for AWS
* LambdaEntryPoint.cs - class that derives from **Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction**. The code in 
this file bootstraps the ASP.NET Core hosting framework. The Lambda function is defined in the base class.
Change the base class to **Amazon.Lambda.AspNetCoreServer.ApplicationLoadBalancerFunction** when using an 
Application Load Balancer.
* LocalEntryPoint.cs - for local development this contains the executable Main function which bootstraps the ASP.NET Core hosting framework with Kestrel, as for typical ASP.NET Core applications.
* Startup.cs - usual ASP.NET Core Startup class used to configure the services ASP.NET Core will use.
* appsettings.json - used for local development.
* Controllers\ValuesController - example Web API controller

You may also have a test project depending on the options selected.

## Here are some steps to follow from Visual Studio:

To deploy your Serverless application, right click the project in Solution Explorer and select *Publish to AWS Lambda*.

To view your deployed application open the Stack View window by double-clicking the stack name shown beneath the AWS CloudFormation node in the AWS Explorer tree. The Stack View also displays the root URL to your published application.

## Here are some steps to follow to get started from the command line:

Once you have edited your template and code you can deploy your application using the [Amazon.Lambda.Tools Global Tool](https://github.com/aws/aws-extensions-for-dotnet-cli#aws-lambda-amazonlambdatools) from the command line.

Install Amazon.Lambda.Tools Global Tools if not already installed.
```
    dotnet tool install -g Amazon.Lambda.Tools
```

If already installed check if new version is available.
```
    dotnet tool update -g Amazon.Lambda.Tools
```

Execute unit tests
```
    cd "ai-my-personal-notes-api/test/ai-my-personal-notes-api.Tests"
    dotnet test
```

Deploy application
```
    cd "ai-my-personal-notes-api/src/ai-my-personal-notes-api"
    dotnet lambda deploy-serverless
```
