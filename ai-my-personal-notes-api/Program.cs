// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddSingleton<Repository>()
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();

builder.Services.AddControllers();

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

var app = builder.Build();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapGet(
        "/",
        async context =>
        {
            await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
        }
    );

    endpoints.MapGraphQL();
});

await app.RunAsync();
