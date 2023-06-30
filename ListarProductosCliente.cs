using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionProspecto
{
    public class ListarProductosCliente : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtener los servicios de rastreo
            ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtener el contexto de ejecución  
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Servicios de referencia
            IOrganizationServiceFactory serviceFactory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            Guid clienteGuid = (Guid)context.InputParameters["id"];

            if (clienteGuid != Guid.Empty)
            {
                try
                {
                    QueryExpression queryProductos = new QueryExpression("cr8e5_productoaofrecer")
                    {
                        ColumnSet = new ColumnSet("cr8e5_productoaofrecerid", "cr8e5_name", "cr8e5_fechavigencia")
                    };

                    EntityCollection productos = service.RetrieveMultiple(queryProductos);

                    QueryExpression queryClienteProductosAux = new QueryExpression("cr8e5_clienteproducto")
                    {
                        ColumnSet = new ColumnSet("cr8e5_productoaofrecer", "cr8e5_estadooferta")
                    };

                    queryClienteProductosAux.Criteria.AddCondition("cr8e5_contact", ConditionOperator.Equal, clienteGuid);
                    queryClienteProductosAux.Criteria.AddCondition("cr8e5_estadooferta", ConditionOperator.Between, 0, 1);

                    EntityCollection clienteProductosAux = service.RetrieveMultiple(queryClienteProductosAux);

                    EntityCollection ofertasCliente = new EntityCollection();

                    var productosOfrecidos = clienteProductosAux.Entities.Select(x => (EntityReference)x.Attributes["cr8e5_productoaofrecer"]);

                    tracingService.Trace(clienteGuid.ToString());


                    foreach (var item in productos.Entities)
                    {
                        tracingService.Trace(item.Id.ToString());
                        var existe = productosOfrecidos.Any(x => x.Id.Equals(item.Id));

                        if (!existe )
                        {
                            ofertasCliente.Entities.Add(item);
                            tracingService.Trace(existe.ToString());
                        }
                    }      

                    if (ofertasCliente.Entities.Count > 0)
                    {
                        context.OutputParameters["productos"] = ofertasCliente;
                        context.OutputParameters["idcliente"] = clienteGuid;
                    }
                }
                catch (Exception ex)
                {
                    tracingService.Trace("Error of Plugin: {0}", ex.ToString());
                    throw;
                }             

            }
        }
    }
}
