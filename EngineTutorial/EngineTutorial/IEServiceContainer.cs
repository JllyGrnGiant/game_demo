using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Innovation
{
    public class IEServiceContainer : IServiceProvider
    {
        // Contains the service types and services
        Dictionary<Type, object> services = new Dictionary<Type, object>();

        // Add a new service
        public void AddService(Type Service, object Provider)
        {
            // If we already have this type of service provider, throw exception
            if (services.ContainsKey(Service))
            {
                throw new Exception("The service container already has a "
                    + "service provider of type " + Service.Name);
            }

            // Add service
            this.services.Add(Service, Provider);
        }

        public void Clear()
        {
            services.Clear();
        }

        // Get a service from the service container
        public object GetService(Type Service)
        {
            // If we have this type of service, return it
            if (services.ContainsKey(Service))
                return services[Service];

            throw new Exception("The service container does not contain "
                + "a service provider of type " + Service.Name);
        }

        // A shortcut way to get a service. The benefit here is that we
        // can specify the type in the brackets and also return the
        // service of that type. For example, instead of
        // "Camera cam = (Camera)Services.GetService(typeof(Camera));",
        // we can use "Camera cam = Services.GetService()"
        public T GetService<T>()
        {
            object result = GetService(typeof(T));

            if (result != null)
                return (T)result;

            return default(T);
        }

        // Removes a service provider from the container
        public void RemoveService(Type Service)
        {
            if (services.ContainsKey(Service))
                services.Remove(Service);
        }

        // Gets whether container has a provider of given type
        public bool ContainsService(Type Service)
        {
            return services.ContainsKey(Service);
        }

        public bool IsService(object Object, out Type ServiceType)
        {
            if (services.ContainsValue(Object))
            {
                foreach (KeyValuePair<Type, object> service in services)
                    if (service.Value == Object)
                    {
                        ServiceType = service.Key;
                        return true;
                    }
            }

            ServiceType = null;
            return false;
        }
    }
}