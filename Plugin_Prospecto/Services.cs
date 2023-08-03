﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Plugin_Prospecto
{
    public abstract class Services : IPlugin, IRetrieve
    {
        private ITracingService tracingService;
        private IPluginExecutionContext context;
        private IOrganizationServiceFactory serviceFactory;
        private IOrganizationService service;

        public ITracingService TracingService { get => tracingService; set => tracingService = value; }
        public IPluginExecutionContext Context { get => context; set => context = value; }
        public IOrganizationServiceFactory ServiceFactory { get => serviceFactory; set => serviceFactory = value; }
        public IOrganizationService Service { get => service; set => service = value; }

        public abstract void Execute(IServiceProvider serviceProvider);
        public void SetServices(IServiceProvider serviceProvider)
        {
            TracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            Context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            Service = ServiceFactory.CreateOrganizationService(Context.UserId);
        }

        public EntityCollection MultipleQuery(string entityName, ColumnSet columnSet, IOrganizationService Service, FilterExpression filterExpression = null, List<OrderExpression> orderExpressions = null)
        {
            QueryExpression query = new QueryExpression(entityName)
            {
                ColumnSet = columnSet,
            };                
            if (filterExpression != null) { query.Criteria = filterExpression; }
            if (orderExpressions != null)
            {
                foreach (var item in orderExpressions)
                {
                    query.AddOrder(item.AttributeName, item.OrderType);
                    query.AddOrder(item.AttributeName, item.OrderType);
                }

            }
            EntityCollection entityCollection = Service.RetrieveMultiple(query);
            return entityCollection;
        }
        public void UpdateControladorProspecto(Entity controlador, Entity prospecto, EntityReference producto, int value)
        {
            Utility.UpdateCounter(ref controlador, "cr8e5_contador", value);

            EntityReference ejecutivo = (EntityReference)controlador.Attributes["cr8e5_ejecutivo"];

            Entity auxProspecto = new Entity("cr8e5_prospecto")
            {
                Id = prospecto.Id,
            };
            auxProspecto.Attributes["cr8e5_ejecutivo"] = ejecutivo;

            Service.Update(auxProspecto);
            Service.Update(controlador);

        }
    }
}
