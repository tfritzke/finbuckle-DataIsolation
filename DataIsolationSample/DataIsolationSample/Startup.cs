
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Finbuckle.MultiTenant;
using DataIsolationSample.Data;
using DataIsolationSample.Models;

namespace DataIsolationSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddMultiTenant<TenantInfo>()
                .WithInMemoryStore(options =>
                {
                    options.IsCaseSensitive = false;
                })
                .WithRouteStrategy();

            // Register the db context, but do not specify connection details
            // since these vary by tenant.
            // The context can be extended with EF code migrations.
            services.AddDbContext<ToDoDbContext>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseMultiTenant();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{__tenant__=}/{controller=Home}/{action=Index}");
            });

            SetupDb(app.ApplicationServices);
        }

        private void SetupDb(IServiceProvider serviceProvider)
        {
            var store = serviceProvider.GetService<IMultiTenantStore<TenantInfo>>();

            // tenant 1
            var ti = new TenantInfo { 
                Id = "finbuckle",
                Identifier = "finbuckle",
                Name = "Finbuckle",
                ConnectionString = @"Data Source=App_Data/Finbuckle_ToDoList.db"
            };
            store.TryAddAsync(ti).Wait();

            using (var db = new ToDoDbContext(ti))
            {
                db.Database.EnsureDeleted();
                db.Database.MigrateAsync().Wait();
                db.ToDoItems.Add(new ToDoItem { Title = "Call Lawyer", Completed = false });
                db.ToDoItems.Add(new ToDoItem { Title = "File Papers", Completed = false });
                db.ToDoItems.Add(new ToDoItem { Title = "Send Invoices", Completed = true });
                db.SaveChanges();
            }

            // tenant 2
            ti = new TenantInfo {
                Id = "megacorp",
                Identifier = "megacorp",
                Name = "MegaCorp",
                ConnectionString = @"Data Source=App_Data/MegaCorp_ToDoList.db"
            };
            store.TryAddAsync(ti).Wait();

            using (var db = new ToDoDbContext(ti))
            {
                db.Database.EnsureDeleted();
                db.Database.MigrateAsync().Wait();
                db.ToDoItems.Add(new ToDoItem { Title = "Send Invoices", Completed = true });
                db.ToDoItems.Add(new ToDoItem { Title = "Construct Additional Pylons", Completed = true });
                db.ToDoItems.Add(new ToDoItem { Title = "Call Insurance Company", Completed = false });
                db.SaveChanges();
            }

            // tenant 3
            ti = new TenantInfo {
                Id = "initech",
                Identifier = "initech",
                Name = "Initech LLC",
                ConnectionString = @"Data Source=App_Data/Initech_ToDoList.db"
            };
            store.TryAddAsync(ti).Wait();

            using (var db = new ToDoDbContext(ti))
            {
                db.Database.EnsureDeleted();
                db.Database.MigrateAsync().Wait();
                db.ToDoItems.Add(new ToDoItem { Title = "Send Invoices", Completed = false });
                db.ToDoItems.Add(new ToDoItem { Title = "Pay Salaries", Completed = true });
                db.ToDoItems.Add(new ToDoItem { Title = "Write Memo", Completed = false });
                db.SaveChanges();
            }
        }
    }
}
