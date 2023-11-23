using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShopingCart.Contracts.Repositories;
using ShopingCart.Repositories;
using Minio;
using Minio.DataModel.Args;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Okta.AspNetCore;
using ShopingCart.Contracts.Workers;
using ShopingCart.Wokers;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using NuGet.Common;
using static System.Net.WebRequestMethods;
using System.Net;
using System.Security.Policy;
using Microsoft.IdentityModel.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var context = builder.Configuration;

context.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.
services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; ;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
    .AddJwtBearer()
    .AddOpenIdConnect(options =>
    {
        options.ClientId = context["Okta:ClientId"];
        options.ClientSecret = context["Okta:ClientSecret"];
        options.Authority = context["Okta:Authority"];
        options.CallbackPath = context["Okta:CallbackPath"];
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.RequireHttpsMetadata = false;
    });

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "ShopingCart", Version = "v1" });
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        In = ParameterLocation.Header,
        Name = "oauth2",
        Description = "OAuth2 Client Credentials Grant",
        OpenIdConnectUrl = new Uri($"{context["Okta:Domain"]}/.well-known/openid-configuration"),
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {

                AuthorizationUrl = new Uri($"{context["Okta:Domain"]}/protocol/openid-connect/auth"),
                TokenUrl = new Uri($"{context["Okta:Domain"]}/protocol/openid-connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenID" },
                    { "profile", "Profile" },
                    { "email", "Email" }
                }
            }
        }
    }); ;
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
{
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                },
                Scheme = "oauth2",
                Name = "oauth2",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.OAuth2
            },
            new List <string> ()
        }
});
});


services.AddScoped<ICarts, Carts>();
services.AddScoped<IProducts, Products>();
services.AddScoped<IImages, Images>();


services.AddDbContext<ShopingCartContext>(options =>
    options.UseSqlServer(context["ConnectionStrings:DefaultConnection"],
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));

services.AddMinio(options => options
                .WithSSL(false)
                .WithEndpoint(context["Minio:Endpoint"])
                .WithCredentials(context["Minio:AccessKey"], context["Minio:SecretKey"]));

services.AddScoped<IProductLogic, ProductLogic>();
services.AddScoped<ICartLogic, CartLogic>();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ShopingCartContext>();
    db.Database.Migrate();
}


IdentityModelEventSource.ShowPII = true;

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());


app.UseAuthorization();
app.UseAuthentication();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ShopingCart V1");
    options.OAuthClientId(context["Okta:ClientId"]);
    options.OAuthClientSecret(context["Okta:ClientSecret"]);
    options.OAuthAppName("ShopingCart");
    options.OAuthScopes("openid", "profile", "email");
    //options.OAuth2RedirectUrl(context["SWAGGER_REDIRECT_URL"]);
    options.DisplayOperationId();
});


app.MapControllers();

app.Run();