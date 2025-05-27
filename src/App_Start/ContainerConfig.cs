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
                   .AsSelf()
                   .SingleInstance();

            // Register services and repositories
            builder.RegisterType<AccomodationPackagesService>()
                    .AsSelf()
                    .SingleInstance();

            builder.RegisterType<AccomodationsService>()
                    .AsSelf()
                    .SingleInstance();

            builder.RegisterType<AccomodationTypesService>()
                    .AsSelf()
                    .SingleInstance();

            builder.RegisterType<BookingsService>()
                    .AsSelf()
                    .SingleInstance();

            builder.RegisterType<CheckInnRoleManager>()
                    .AsSelf()
                    .SingleInstance();

            builder.RegisterType<CheckInnSignInManager>()
                    .AsSelf()
                    .SingleInstance();
            
            builder.RegisterType<EmailService>()
                    .AsSelf()
                    .SingleInstance();

            builder.RegisterType<PaymentService>()
                    .AsSelf()
                    .SingleInstance();

            builder.RegisterType<StripeService>()
                    .AsSelf()
                    .SingleInstance();

            return builder.Build();
        }
    }
}
