# AI my personal notes APIs solution
- APIs powered by the ChatGPT AI and Elastic Search

## Project template notes
- project is created using AWS SAM CLI - [here](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-getting-started-hello-world.html)

### Setup (once while creating a new project so skip this)
```properties
# only needed once while creating a new project
sam init
```

### Build / Test / Deploy
```properties
# Build the project:
sam build

# Validate SAM template: 
sam validate

# Invoke Function: 
sam local invoke (doesnt work)
sam local start-api --debug

# Test Function in the Cloud: 
sam sync --stack-name {{stack-name}} --watch

# Deploy: 
sam deploy --guided
```