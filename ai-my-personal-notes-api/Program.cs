using System.Text;
using ai_my_personal_notes_api;
using GraphQLAuthDemo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

// Global variables - START
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Global variables - END

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("https://localhost:8081;http://localhost:8080");

// Note that the order of initializing the services is quite important
// refer here - https://stackoverflow.com/questions/72001875/net-6-webapi-with-hotchocolate-graphql-not-authorizing

// 1. initialize the dotnet default authentication to validate the jwt token
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = "audience",
            ValidIssuer = "issuer",
            RequireSignedTokens = false,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    $"{Environment.GetEnvironmentVariable("IDENTITY_SERVER_SECRET")}"
                )
            )
        };
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
    })
    .Services.AddAuthorization();

builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddHttpContextAccessor();

builder
    .Services.AddSingleton<Repository>()
    .AddGraphQLServer()
    .AddHttpRequestInterceptor<HttpRequestInterceptor>()
    .AddAuthorization(options =>
    {
        // 2. check if the bearer token has the required role
        options.AddPolicy("hr", builder => builder.RequireAuthenticatedUser().RequireRole("hr"));
    })
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();

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

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

var app = builder.Build();

app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
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
