﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ITOps.ViewModelComposition.Gateway
{
    public static class ServiceCollectionExtensions
    {
        public static void AddViewModelComposition(this IServiceCollection services, string assemblySearchPattern = "*ViewModelComposition*.dll")
        {
            var fileNames = Directory.GetFiles(AppContext.BaseDirectory, assemblySearchPattern);

            var types = new List<Type>();
            foreach (var fileName in fileNames)
            {
                var temp = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(fileName)))
                    .GetTypes()
                    .Where(t =>
                    {
                        return !t.GetTypeInfo().IsInterface &&
                        (
                            typeof(IRouteInterceptor).IsAssignableFrom(t)
                            //|| typeof(IRegisterRoutes).IsAssignableFrom(t)
                        );
                    });

                types.AddRange(temp);
            }

            foreach (var type in types)
            {
                //if (typeof(IRegisterRoutes).IsAssignableFrom(type))
                //{
                //    services.AddSingleton(typeof(IRegisterRoutes), type);
                //}

                if (typeof(IRouteInterceptor).IsAssignableFrom(type))
                {
                    services.AddSingleton(typeof(IRouteInterceptor), type);
                }
            }
        }
    }
}