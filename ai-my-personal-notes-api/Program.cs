// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddSingleton<Repository>()
    .AddAuthentication()
    .Services.AddAuthorization(o => o.AddPolicy("Librarian", p => p.RequireAssertion(_ => true)))
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddAuthorization();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:3000",
                "https://ai-my-personal-notes-ui.vercel.app"
            );
            policy.AllowAnyHeader();
        }
    );
});

builder.Services.AddControllers();

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

builder.WebHost.UseUrls("https://localhost:8081;http://localhost:8080");

var app = builder.Build();

app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);
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
