using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Services;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_Prospecto
{
    public class AsignarEjecutivo : IPlugin, IRetrieve
    {
        private Services services;

        public void Execute(IServiceProvider serviceProvider)
        {

            services = new Services(serviceProvider);
            if (services.Context.InputParameters.Contains("Target") &&
                services.Context.InputParameters["Target"] is Entity)
            {
                try
                {
                    // Obtener la entidad target 
                    Entity prospecto = (Entity)services.Context.InputParameters["Target"];
                    EntityReference producto = (EntityReference)prospecto.Attributes["cr8e5_productoaofrecer"];

                    // Obtener el configurador del producto
                    ColumnSet columnSet = new ColumnSet("cr8e5_ejecutivo", "cr8e5_contador");
                    FilterExpression filterExpression = new FilterExpression();
                    filterExpression.AddCondition("cr8e5_productoaofrecer", ConditionOperator.Equal, producto.Id);
                    List<OrderExpression> orderExpressions = new List<OrderExpression>
                        {
                            new OrderExpression("cr8e5_contador", OrderType.Ascending)
                        };
                    EntityCollection entityCollection = MultipleQuery("cr8e5_configuradordeproducto", columnSet, services.Service,filterExpression, orderExpressions);


                    if (entityCollection.Entities.Count > 0)
                    {
                        Entity firstControlador = entityCollection.Entities[0];
                        Utility.UpdateCounter(ref firstControlador, "cr8e5_contador", 1);

                        EntityReference ejecutivo = (EntityReference)firstControlador.Attributes["cr8e5_ejecutivo"];

                        Entity auxProspecto = new Entity("cr8e5_prospecto")
                        {
                            Id = prospecto.Id,
                        };
                        auxProspecto.Attributes["cr8e5_ejecutivo"] = ejecutivo;

                        services.Service.Update(auxProspecto);
                        services.Service.Update(firstControlador);

                        AuxProductosOfrecidos.CreateAuxProductos(prospecto.Id, producto, services.Service);
                    }

                }
                catch (Exception ex)
                {
                    services.TracingService.Trace("Error of Plugin: {0}", ex.ToString());
                    throw;
                }
            }
        }

        public EntityCollection MultipleQuery(string entityName, ColumnSet columnSet, IOrganizationService Service, FilterExpression filterExpression = null, List<OrderExpression> orderExpressions = null)
        {
            QueryExpression query = new QueryExpression(entityName)
            {
                ColumnSet = columnSet,
                Criteria = filterExpression
            };
            foreach (var item in orderExpressions)
            {
                query.AddOrder(item.AttributeName, item.OrderType);
                query.AddOrder(item.AttributeName, item.OrderType);
            }
            EntityCollection entityCollection = Service.RetrieveMultiple(query);
            return entityCollection;
        }
    }
}
