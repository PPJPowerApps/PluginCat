using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionProspecto
{
    public class ReasignarEjecutivo : IPlugin
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

            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                try
                {
                    // Evitar un bucle infinito limitando a uno la ejecución
                    if (context.Depth > 1)
                    {
                        return;
                    }
                    // Obtener la entidad prospecto que fue creada
                    Entity prospecto = (Entity)context.InputParameters["Target"];

                    ColumnSet cs = new ColumnSet("cr8e5_productoaofrecer", "cr8e5_ejecutivo");
                    Entity auxProspecto = service.Retrieve("cr8e5_prospecto", prospecto.Id, cs);
                    
                    // Obtener el producto nuevo que se asignó
                    EntityReference productoNuevo = (EntityReference)prospecto.Attributes["cr8e5_productoaofrecer"];
                    EntityReference ejecutivoNuevo;

                    // Obtener el producto actual
                    EntityReference productoActual = (EntityReference)auxProspecto.Attributes["cr8e5_productoaofrecer"];
                    // Obtener el ejecutivo actual
                    EntityReference ejecutivoActual = (EntityReference)auxProspecto.Attributes["cr8e5_ejecutivo"];


                    // Configurador productos
                    QueryExpression query = new QueryExpression("cr8e5_configuradordeproducto")
                    {
                        ColumnSet = new ColumnSet("cr8e5_ejecutivo", "cr8e5_contador")
                    };
                    // Agregar filtro para buscar solo el producto que fue asignado al prospecto en configurador producto
                    query.Criteria.AddCondition("cr8e5_productoaofrecer", ConditionOperator.Equal, productoNuevo.Id);
                    query.AddOrder("cr8e5_contador", OrderType.Ascending);
                    EntityCollection entityConfigurador = service.RetrieveMultiple(query);


                    // Comprobar que los datos obtenidos no estén vacíos
                    if (entityConfigurador.Entities.Count > 0)
                    {
                        // Obtener el primer configurador prducto
                        Entity aux = entityConfigurador.Entities[0];
                        // Aumentar el contador de los prospectos que se encarga el ejecutivo
                        aux.Attributes["cr8e5_contador"] = int.Parse(aux.Attributes["cr8e5_contador"].ToString()) + 1;

                        // Obtener la referencia de ejecutivo desde configurador producto para asignarlo al prospecto
                        ejecutivoNuevo = (EntityReference)aux.Attributes["cr8e5_ejecutivo"];
                        auxProspecto.Attributes["cr8e5_ejecutivo"] = ejecutivoNuevo;

                        // Actualizar el configurador producto y el prospecto
                        service.Update(aux);
                        service.Update(auxProspecto);

                        // Iniciar registro de los productos ofrecidos al mismo prospecto
                        Entity auxProductos = new Entity("cr8e5_auxproductosofrecidos");
                        auxProductos.Attributes["cr8e5_prospecto"] = new EntityReference("cr8e5_prospecto", prospecto.Id);
                        auxProductos.Attributes["cr8e5_productoaofrecer"] = (EntityReference)prospecto.Attributes["cr8e5_productoaofrecer"];

                        service.Create(auxProductos);
                    }

                    // Buscar configurador producto del ejecutivo actual

                    QueryExpression queryEjecutivoActual = new QueryExpression("cr8e5_configuradordeproducto")
                    {
                        ColumnSet = new ColumnSet("cr8e5_contador")
                    };
                    // Buscar el configurador producto del ejetuvico actual
                    queryEjecutivoActual.Criteria.AddCondition("cr8e5_ejecutivo", ConditionOperator.Equal, ejecutivoActual.Id);
                    queryEjecutivoActual.Criteria.AddCondition("cr8e5_productoaofrecer", ConditionOperator.Equal, productoActual.Id);
                    EntityCollection entityConfiguradorEjecutivo = service.RetrieveMultiple(queryEjecutivoActual);

                    // Reducir el contador en 1
                    Entity auxEjec = entityConfiguradorEjecutivo.Entities[0];
                    auxEjec.Attributes["cr8e5_contador"] = int.Parse(auxEjec.Attributes["cr8e5_contador"].ToString()) - 1;

                    service.Update(auxEjec);
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
