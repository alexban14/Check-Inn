namespace Check_Inn.Migrations
{
    using Check_Inn.Entities;
    using Check_Inn.Services;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Threading.Tasks;

    internal sealed class Configuration : DbMigrationsConfiguration<Check_Inn.DAL.CheckInnMySqlContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Check_Inn.DAL.CheckInnMySqlContext context)
        {
            var userManager = new CheckInnUserManager(new UserStore<User>(context));
            var roleManager = new CheckInnRoleManager(new RoleStore<IdentityRole>(context));

            var seeder = new DatabaseSeeder(context, userManager, roleManager);

            // Seed the database asynchronously
            SeedDatabase(seeder).GetAwaiter().GetResult();
        }
        private async Task SeedDatabase(DatabaseSeeder seeder)
        {
            await seeder.SeedAsync();
        }
    }
}
