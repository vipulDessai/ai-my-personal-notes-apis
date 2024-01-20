// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddSingleton<Repository>()
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

var app = builder.Build();

app.UseRouting();

app.UseEndpoints(endpoints => endpoints.MapGraphQL());

await app.RunAsync();
