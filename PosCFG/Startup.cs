using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PosCFG.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using AutoMapper;
using PosCFG.Service;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;

namespace PosCFG
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
            services.AddAuthentication( options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = Configuration.GetSection("UrlEntity").GetSection("url_path").Value;

                options.ClientId = "mvc";
                options.ClientSecret = "secret";
                options.ResponseType = "code";

                var jwtHandler = new JwtSecurityTokenHandler();
                jwtHandler.InboundClaimTypeMap.Clear();

                options.SecurityTokenValidator = jwtHandler;

                options.Scope.Add("roles");
                options.SaveTokens = true;
            })
            .AddCookie("Cookies")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = Configuration.GetSection("UrlEntity").GetSection("url_path").Value;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };
                
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministratorRole", policy => 
                {
                    policy.RequireClaim("role","Administrator");
                });
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireOperadorRole", policy => 
                {
                    policy.RequireClaim("role","Operador");
                });
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "terminalexternal");
                    
                });
            });
            
            //services.AddSingleton<ICSVService>(new CSVService("TDS/Parameters"));
            services.Configure<TDSFileConfig>(Configuration.GetSection("TDSFileConfig"));

            //services.AddOptions();

            services.AddSingleton<ICSVService, CSVService>();

            //fileservice
            services.AddScoped<IFileService, FileService>();

            //automapper
            services.AddAutoMapper(typeof(Startup));

            //path settings in appsettings
            services.Configure<PathSettings>(Configuration.GetSection("PathSettings"));

            //terminalsExternal
            services.AddScoped<ITerminalsExternalService, TerminalsExternalService>();
            
            services.AddScoped<ITerminalService, TerminalService>();
            services.AddScoped<ISystemPOSService, SystemPOSService>();

            //full Terminal System        
            services.AddScoped<IFullTerminalSystemService, FullTerminalSystemService>();
            
            //connection MYSQL
            services.AddDbContext<JPOSDbContext>(options =>
            options.UseMySQL(Configuration.GetConnectionString("JPOSConnection")));
            
            //connection SQL Server        
            services.AddDbContext<PosCFGDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressConsumesConstraintForFormFileParameters = true;
                options.SuppressInferBindingSourcesForParameters = true;
                options.SuppressModelStateInvalidFilter = true;
                options.SuppressMapClientErrors = true;
            });;
  
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "PosCFG API",
                    Description = "PosCFG API",
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("https://example.com/license"),
                    }
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
});

            });
            services.AddRazorPages(options =>
            {
                //options.Conventions.AuthorizePage("/Contact");
                
                //Sin autenticar
               // options.Conventions.AuthorizeFolder("/","RequireOperadorRole");
                
                //options.Conventions.AllowAnonymousToPage("/Private/PublicPage");
                //options.Conventions.AllowAnonymousToFolder("/Private/PublicPages");
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ///Cuando cambia el base path, descomentar
            app.UsePathBase("/poscfg");
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/poscfg/swagger/v1/swagger.json", "PosCFG V1");
            });


            if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
            if (env.IsProduction() || env.IsStaging() || env.IsEnvironment("Staging_2"))
            {
                app.UseExceptionHandler("/Error");
            }


            app.UseHttpsRedirection();

            app.UseRouting();

            //habilitar para token
            app.UseAuthentication();
            app.UseAuthorization();



            //Se habilita para uso de css
            app.UseStaticFiles();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(name: "Logout",
                    pattern: "{controller=Account}/{action=Logout}");
                endpoints.MapRazorPages();
            });
        }
    }
}
