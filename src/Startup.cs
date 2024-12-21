using Check_Inn.Areas.Dashboard.Middleware;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Data.Entity.Migrations;
using WebGrease.Css.Ast.Selectors;

[assembly: OwinStartupAttribute(typeof(Check_Inn.Startup))]
namespace Check_Inn
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            ApplyMigrations();

            Dashboard.RegisterMiddleware(app);
        }

        private void ApplyMigrations()
        {
            var configuration = new Check_Inn.Migrations.Configuration
            {
                AutomaticMigrationsEnabled = true, // Set as needed
                AutomaticMigrationDataLossAllowed = false
            };

            var migrator = new DbMigrator(configuration);
            migrator.Update();
        }
    }
}
