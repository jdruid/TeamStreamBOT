using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using Autofac;
using Microsoft.Azure.Search.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;

using TeamStreamApp.BotComponents.Search.Contracts.Models;
using TeamStreamApp.Dialogs;
using TeamStreamApp.BotComponents.Search.Azure.Services;
using TeamStreamApp.BotComponents.Search.Contracts.Services;

namespace TeamStreamApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterType<IntroDialog>()
              .As<IDialog<object>>()
              .InstancePerDependency();

            builder.RegisterType<TeamStreamMapper>()
                .Keyed<IMapper<DocumentSearchResult, GenericSearchResults>>(FiberModule.Key_DoNotSerialize)
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<AzureSearchClient>()
               .Keyed<ISearchClient>(FiberModule.Key_DoNotSerialize)
               .AsImplementedInterfaces()
               .SingleInstance();

            builder.Update(Conversation.Container);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            DocumentDBRepository<TeamStreamApp.Models.Video>.Initialize();

            
        }
    }
}
