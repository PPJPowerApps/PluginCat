using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Plugin_Prospecto
{
    public class Services
    {
        // Variables
        private ITracingService tracingService;
        private IPluginExecutionContext context;
        private IOrganizationServiceFactory serviceFactory;
        private IOrganizationService service;

        public ITracingService TracingService { get => tracingService; set => tracingService = value; }
        public IPluginExecutionContext Context { get => context; set => context = value; }
        public IOrganizationServiceFactory ServiceFactory { get => serviceFactory; set => serviceFactory = value; }
        public IOrganizationService Service { get => service; set => service = value; }

        // Asignar los valores desde el contexto de ejecución
        public void SetServices(IServiceProvider serviceProvider)
        {
            TracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            Context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            Service = ServiceFactory.CreateOrganizationService(Context.UserId);
        }
    }
}
