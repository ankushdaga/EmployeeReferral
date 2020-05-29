using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using ReferralSystem.Repository;
using ReferralSystem.Service;
using ReferralSystem.Models;
using Microsoft.Identity.Core.UI;
using Microsoft.IdentityModel.Tokens;
using ReferralSystem.Extensions;


namespace ReferralSystem
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.UseCookieAuthentication(new CookieAuthenticationOptions
            //{
               
            //    LoginPath = new PathString("/Account/Login")
            //});


            // requires using Microsoft.Extensions.Options
            services.Configure<MongoDbSettings>(
                Configuration.GetSection(nameof(MongoDbSettings)));

            services.AddSingleton<IMongoDbSettings>(sp =>
                sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

         

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            
           
          .AddCookie()
          .AddOpenIdConnect("Auth0", options => {
                // Set the authority to your Auth0 domain
                options.Authority = $"https://{Configuration["Auth0:Domain"]}";

                // Configure the Auth0 Client ID and Client Secret
                options.ClientId = Configuration["Auth0:ClientId"];
              options.ClientSecret = Configuration["Auth0:ClientSecret"];

                // Set response type to code
                options.ResponseType = "code";

                // Configure the scope
                options.Scope.Clear();
              options.Scope.Add("openid");
              options.Scope.Add("profile");
              options.Scope.Add("email");
              options.TokenValidationParameters = new TokenValidationParameters
              {
                  NameClaimType = "name",

              };

                // Set the callback path, so Auth0 will call back to http://localhost:3000/callback
                // Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard
                options.CallbackPath = new PathString("/callback");

                // Configure the Claims Issuer to be Auth0
                options.ClaimsIssuer = "Auth0";

                // Saves tokens to the AuthenticationProperties
                options.SaveTokens = true;

              options.Events = new OpenIdConnectEvents
              {
                  OnAuthorizationCodeReceived = (context) =>
                  {

                      return Task.CompletedTask;
                  },
                  
                    // handle the logout redirection 
                    OnRedirectToIdentityProviderForSignOut = (context) =>
                  {
                      var logoutUri = $"https://{Configuration["Auth0:Domain"]}/v2/logout?client_id={Configuration["Auth0:ClientId"]}";

                      var postLogoutUri = context.Properties.RedirectUri;
                      if (!string.IsNullOrEmpty(postLogoutUri))
                      {
                          if (postLogoutUri.StartsWith("/"))
                          {
                                // transform to absolute
                                var request = context.Request;
                              postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                          }
                          logoutUri += $"&returnTo={ Uri.EscapeDataString(postLogoutUri)}";
                      }

                      context.Response.Redirect(logoutUri);
                      context.HandleResponse();

                      return Task.CompletedTask;
                  }
              };
          });

            services.AddAuthorization(options =>
            {
               // options.AddPolicy("RequireAdministratorRole",
                //    policy => policy.RequireRole("HR"));
            });

            //services.addsign(Configuration);

            //services.AddAuthentication(AzureADB2CDefaults.AuthenticationScheme)
            //    .AddAzureADB2C(options => Configuration.Bind("AzureAdB2C", options));

            //services.Configure<JwtBearerOptions>(AzureADB2CDefaults.JwtBearerAuthenticationScheme, options =>
            //{
            //    // This is an Azure AD v2.0 Web API
            //    options.Authority += "/v2.0";

            //    // The valid audiences are both the Client ID (options.Audience) and api://{ClientID}
            //    options.TokenValidationParameters.ValidAudiences = new string[] { options.Audience, $"api://{options.Audience}" };

            //    // Instead of using the default validation (validating against a single tenant, as we do in line of business apps),
            //    // we inject our own multitenant validation logic (which even accepts both V1 and V2 tokens)
            //    options.TokenValidationParameters.IssuerValidator = AadIssuerValidator.ValidateAadIssuer;
            //});
            //services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
            //{
            //    microsoftOptions.ClientId = "76b04c1f-2db1-4c60-b7a5-4022369018ab";
            //    microsoftOptions.ClientSecret = "ykUxQNDCLWBsk41vv_09-N36o1i~Hpl3F-";

            //});

            //services.AddAuthentication(AzureADB2CDefaults.AuthenticationScheme)
            //    .AddAzureADB2C(options => Configuration.Bind("AzureAdB2C", options));


            //services.Configure<OpenIdConnectOptions>(
            //    AzureADB2CDefaults.OpenIdScheme, options =>
            //    {

            //        // Omitted for brevity
            //    });

            //services.Configure<CookieAuthenticationOptions>(
            //    AzureADB2CDefaults.CookieScheme, options =>
            //    {
            //        // Omitted for brevity
            //    });

            //services.Configure<JwtBearerOptions>(
            //    AzureADB2CDefaults.JwtBearerAuthenticationScheme, options =>
            //    {
            //        // Omitted for brevity
            //    });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddMvc().AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());


            services.AddKendo();



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

          

          //  app.UseMiddleware<CustomMiddleware>();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
