// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServerAspNetIdentity.Data;
using IdentityServerAspNetIdentity.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.HttpOverrides;

namespace IdentityServerAspNetIdentity
{
    public class Startup
    {
        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.Clients)
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.IdentityResources)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.ApiResources)
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var resource in Config.ApiScopes)
                    {
                        context.ApiScopes.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            string connectionString = Configuration.GetConnectionString("ConnectionStore");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;
            })
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                        //CAMBIO NOMBRE DE TABLAS PARA QUE FUNCIONE CON ADMINUI
                        //options.DefaultSchema = "IdentityServer4";
                        options.ApiResourceClaim.Name = "ApiClaims";
                        options.ApiResourceProperty.Name = "ApiProperties";
                        options.ApiResource.Name = "ApiResources";
                        options.ApiScopeClaim.Name = "ApiScopeClaims";
                        options.ApiScope.Name = "ApiScopes";
                        options.ApiResourceSecret.Name = "ApiSecrets";
                        options.ClientClaim.Name = "ClientClaims";
                        options.ClientCorsOrigin.Name = "ClientCorsOrigins";
                        options.ClientGrantType.Name = "ClientGrantTypes";
                        options.ClientIdPRestriction.Name = "ClientIdPRestrictions";
                        options.ClientPostLogoutRedirectUri.Name = "ClientPostLogoutRedirectUris";
                        options.ClientProperty.Name = "ClientProperties";
                        options.ClientRedirectUri.Name = "ClientRedirectUris";
                        options.Client.Name = "Clients";
                        options.ClientScopes.Name = "ClientScopes";
                        options.ClientSecret.Name = "ClientSecrets";
                        options.IdentityResourceClaim.Name = "IdentityClaims";
                        options.IdentityResourceProperty.Name = "IdentityProperties";
                        options.IdentityResource.Name = "IdentityResources";

                        /////FIN
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                        
                })
                ////DATOS EN MEMORIA
                ////.AddInMemoryIdentityResources(Config.IdentityResources)
                ////.AddInMemoryApiScopes(Config.ApiScopes)
                ////.AddInMemoryClients(Config.Clients)
                .AddAspNetIdentity<ApplicationUser>();
            
            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    
                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to https://localhost:5001/signin-google
                    options.ClientId = "copy client ID from Google here";
                    options.ClientSecret = "copy client secret from Google here";
                });

           /* services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });*/
        }

        public void Configure(IApplicationBuilder app)
        {   ///Descomentar para inicializar database
            //InitializeDatabase(app);

            //Config para Apache como RP
            // using Microsoft.AspNetCore.HttpOverrides;

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            //Fin config RP 

           // app.UseCors("CorsPolicy");

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }


            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}