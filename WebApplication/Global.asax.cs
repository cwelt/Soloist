using Autofac;
using Autofac.Integration.Mvc;
using CW.Soloist.DataAccess;
using CW.Soloist.DataAccess.DomainModels;
using CW.Soloist.DataAccess.EntityFramework;
using CW.Soloist.DataAccess.Repositories;
using CW.Soloist.WebApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac.Core;
using Evernote.EDAM.Type;

namespace CW.Soloist.WebApplication
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Database.SetInitializer<ApplicationDbContext>(null);

            #region Autofac DI Configurations 

            // create an autofac container builder
            ContainerBuilder builder = new ContainerBuilder();

            // Register MVC controllers (MvcApplication is the name of the class in Global.asax.)
            builder.RegisterControllers(typeof(MvcApplication).Assembly).InstancePerLifetimeScope();

            // manual registration of types and services;
            builder.RegisterType<ApplicationDbContext>().InstancePerLifetimeScope();


            builder.RegisterType<EFUnitOfWork>()
                .As<IUnitOfWork>()
                //.WithParameter(new TypedParameter(typeof(ApplicationDbContext), new ApplicationDbContext()))
                .InstancePerRequest();

            builder.RegisterType<SongRepository>()
                .As<ISongRepostiory>()
                //.WithParameter(new TypedParameter(typeof(ApplicationDbContext), new ApplicationDbContext()))
                .InstancePerRequest();

            /* builder.RegisterAssemblyTypes(typeof(Song).Assembly)
                  .Where(type => type.Name.EndsWith("Repository"))
                  .As(type => type.GetInterfaces()?.FirstOrDefault(
                      interfaceType => interfaceType.Name.Equals("I" + type.Name)))
                  .InstancePerRequest()
                  .WithParameter(new TypedParameter(typeof(ApplicationDbContext), new ApplicationDbContext()));*/


            // OPTIONAL: Register model binders that require DI.
            builder.RegisterModelBinders(typeof(MvcApplication).Assembly);
            builder.RegisterModelBinderProvider();

            // OPTIONAL: Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            // OPTIONAL: Enable property injection in view pages.
            builder.RegisterSource(new ViewRegistrationSource());

            // OPTIONAL: Enable property injection into action filters.
            builder.RegisterFilterProvider();

            // OPTIONAL: Enable action method parameter injection (RARE).
            //builder.InjectActionInvoker();

            // For property injection using Autofac
            // builder.RegisterType<TheRequestedService>().PropertiesAutowired();

            // build the dependencies container 
            IContainer container = builder.Build();

            // Replqde the default ASP.NET MVC dependency resolver to Autofac's dependency resolver
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            #endregion
        }
    }
}
