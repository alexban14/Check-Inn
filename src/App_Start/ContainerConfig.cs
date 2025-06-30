using Autofac;
using Autofac.Integration.Mvc;
using Check_Inn.DAL;
using Check_Inn.Services;
using System.Reflection;
using System.Web.Mvc;

namespace Check_Inn.Infrastructure
{
    public static class ContainerConfig
    {
        public static IContainer RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            // Register controllers
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            // Register DbContext for DI
            builder.RegisterType<CheckInnMySqlContext>()
                   .As<ICheckInnContext>()
                   .AsSelf()
                   .InstancePerRequest();

            // Register services and repositories
            builder.RegisterType<AccomodationPackagesService>()
                   .AsSelf()
                   .InstancePerRequest();

            builder.RegisterType<AccomodationsService>()
                    .AsSelf()
                    .InstancePerRequest();

            builder.RegisterType<AccomodationTypesService>()
                    .AsSelf()
                    .InstancePerRequest();

            builder.RegisterType<BookingsService>()
                    .AsSelf()
                    .InstancePerRequest();

            builder.RegisterType<CheckInnRoleManager>()
                    .AsSelf()
                    .InstancePerRequest();

            builder.RegisterType<CheckInnSignInManager>()
                    .AsSelf()
                    .InstancePerRequest();
            
            builder.RegisterType<EmailService>()
                    .AsSelf()
                    .InstancePerRequest();

            builder.RegisterType<PaymentService>()
                    .AsSelf()
                    .InstancePerRequest();

            builder.RegisterType<StripeService>()
                    .AsSelf()
                    .InstancePerRequest();

            return builder.Build();
        }
    }
}
