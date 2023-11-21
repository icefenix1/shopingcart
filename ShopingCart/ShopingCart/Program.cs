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

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var context = builder.Configuration;

context.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.


//services.AddAuthentication(options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
//})
//            .AddCookie()
//            .AddOktaMvc(new OktaMvcOptions
//            {
//                OktaDomain = context["Okta:Domain"],
//                AuthorizationServerId = context["Okta:Authority"],
//                ClientId = context["Okta:ClientId"],
//                ClientSecret = context["Okta:ClientSecret"],
//                Scope = new List<string> { "openid", "profile", "email" },
//                //GetClaimsFromUserInfoEndpoint = true,
//                CallbackPath = context["Okta:CallbackPath"]
//            });
//.AddOpenIdConnect(options =>
//{
//    options.ClientId = context["Okta:ClientId"];
//    options.ClientSecret = context["Okta:ClientSecret"];
//    options.Authority = context["Okta:Authority"];
//    options.CallbackPath = context["Okta:CallbackPath"];
//    options.ResponseType = OpenIdConnectResponseType.CodeToken;
//    options.Scope.Add("openid");
//    options.Scope.Add("profile");
//});


services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
//    c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ShopingCart", Version = "v1" });
//    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
//    {
//        Type = SecuritySchemeType.OAuth2,
//        Flows = new OpenApiOAuthFlows
//            {
//            Implicit = new OpenApiOAuthFlow
//            {
//                AuthorizationUrl = new Uri(context["Okta:Authority"]),                
//                Scopes = new Dictionary<string, string>
//                {
//                    { "openid", "openid" },
//                    { "profile", "profile" },
//                }
//            }
//        }
//    });
//    c.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme {
//                Reference = new OpenApiReference {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "oauth2"
//                },
//                Scheme = "oauth2",
//                Name = "oauth2",
//                In = ParameterLocation.Header                
//            },
//            new List <string> ()

//        } 
//    });
//});


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

app.UseHttpsRedirection();

//app.UseAuthorization();
//app.UseAuthentication();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
//    options =>
//{
//    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ShopingCart V1");
//    options.OAuthClientId(context["Okta:ClientId"]);
//    options.OAuthClientSecret(context["Okta:ClientSecret"]);
//    options.OAuthAppName("ShopingCart");
//    options.OAuthUseBasicAuthenticationWithAccessCodeGrant();
//    options.OAuth2RedirectUrl($"https://localhost:51853{context["Okta:CallbackPath"]}");
//});


app.MapControllers();

app.Run();
