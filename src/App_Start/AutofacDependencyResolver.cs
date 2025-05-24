using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Autofac;

namespace Check_Inn.Infrastructure
{
    public class AutofacDependencyResolver : IDependencyResolver
    {
        private readonly IContainer _container;

        public AutofacDependencyResolver(IContainer container)
        {
            _container = container;
        }

        public object GetService(Type serviceType)
        {
            return _container.IsRegistered(serviceType) ? _container.Resolve(serviceType) : null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            var enumerableServiceType = typeof(IEnumerable<>).MakeGenericType(serviceType);

            return _container.IsRegistered(serviceType) 
                ? (IEnumerable<object>)_container.Resolve(enumerableServiceType) 
                : Enumerable.Empty<object>();
        }
    }
}
