// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using IdentityServerAspNetIdentity.Data;
using IdentityServerAspNetIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace IdentityServerAspNetIdentity
{
    public class SeedData
    {
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    context.Database.Migrate();

                    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    



                    var nacho = userMgr.FindByNameAsync("nacho").Result;
                    //Descomentar para configurar Role al Usuario
                    //var r=userMgr.AddToRoleAsync(nacho,"Administrator").Result;

                    if (nacho == null)
                    {
                        nacho = new ApplicationUser
                        {
                            UserName = "nacho",
                            Email = "nacho@manih.tech",
                            EmailConfirmed = true,
                        };
                        var result = userMgr.CreateAsync(nacho, "Clich3$").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(nacho, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Ignacio Ghione"),
                            new Claim(JwtClaimTypes.GivenName, "Ignacio"),
                            new Claim(JwtClaimTypes.FamilyName, "Ghione"),
                            new Claim(JwtClaimTypes.WebSite, "https://manih.tech"),
                        }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        result = userMgr.AddToRoleAsync(nacho,"Administrator").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }


                        Log.Debug("nacho created");
                    }
                    else
                    {
                        Log.Debug("nacho already exists");
                    }

                    var admin = userMgr.FindByNameAsync("admin").Result;
                    var r=userMgr.AddToRoleAsync(admin,"Administrator").Result;
                    if (admin == null)
                    {
                        //var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                        //var role = new IdentityRole();
                        //role.Name = "Administrator";
                        //var result = roleMgr.CreateAsync(role).Result;

                       // if (!result.Succeeded)
                        //{
                            //throw new Exception(result.Errors.First().Description);
                        //}

                        admin = new ApplicationUser
                        {
                            UserName = "admin",
                            Email = "admin@manih.tech",
                            EmailConfirmed = true
                        };
                        var result = userMgr.CreateAsync(admin, "ManiHS3vap4rriba!").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(admin, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "admin"),
                            new Claim(JwtClaimTypes.GivenName, "Administrator"),
                            new Claim(JwtClaimTypes.FamilyName, "Resonance"),
                            new Claim(JwtClaimTypes.WebSite, "https://resonance.com.uy"),
                            new Claim("location", "somewhere")
                        }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Log.Debug("admin created");
                    }
                    else
                    {
                        Log.Debug("admin already exists");
                    }
                }
            }
        }
    }
}
