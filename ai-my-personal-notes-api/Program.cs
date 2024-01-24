// Program.cs
using System.Text;
using GraphQLAuthDemo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("https://localhost:8081;http://localhost:8080");

builder
    .Services.AddSingleton<Repository>()
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddAuthorization()
    .AddHttpRequestInterceptor<HttpRequestInterceptor>();

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

builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddHttpContextAccessor();

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
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
                Encoding.UTF8.GetBytes("secretsecretsecret")
            )
        };
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
    });

//builder.Services.AddAuthorization(x =>
//{
//    x.AddPolicy("hr-department", builder => builder.RequireAuthenticatedUser().RequireRole("hr"));
//    x.AddPolicy("DevDepartment", builder => builder.RequireRole("dev"));
//});

var app = builder.Build();

app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);

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

app.UseAuthentication();

await app.RunAsync();
