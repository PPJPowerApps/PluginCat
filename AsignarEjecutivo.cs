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
    public class AsignarEjecutivo : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtener los servicios de Dynamics 365
            ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtener el contexto de ejecución en el que se llamó al plugin
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

                    // Obtener el producto que se asignó
                    EntityReference producto = (EntityReference)prospecto.Attributes["cr8e5_productoaofrecer"];

                    // Obtener todos los configurador productos que cumplen con las condiciones posteriormente asignadas
                    QueryExpression query = new QueryExpression("cr8e5_configuradordeproducto")
                    {
                        ColumnSet = new ColumnSet("cr8e5_ejecutivo", "cr8e5_contador")
                    };
                    // Agregar filtro para buscar solo el producto que fue asignado al prospecto en configurador producto
                    query.Criteria.AddCondition("cr8e5_productoaofrecer", ConditionOperator.Equal, producto.Id);
                    // Odenar la consulta en base a la cantidad de prospectos de los que se está encargando cada ejecutivo
                    query.AddOrder("cr8e5_contador", OrderType.Ascending);
                    EntityCollection entityConfigurador = service.RetrieveMultiple(query);

                    // Declarar el ejecutivo que será asignado posteriormente
                    EntityReference ejecutivo;

                    // Comprobar que los datos obtenidos no estén vacíos
                    if (entityConfigurador.Entities.Count > 0)
                    {
                        // Obtener el primer configurador prducto
                        Entity aux = entityConfigurador.Entities[0];
                        // Aumentar el contador de los prospectos que se encarga el ejecutivo
                        aux.Attributes["cr8e5_contador"] = int.Parse(aux.Attributes["cr8e5_contador"].ToString()) + 1;

                        // Obtener la referencia de ejecutivo desde configurador producto para asignarlo al prospecto
                        ejecutivo = (EntityReference)aux.Attributes["cr8e5_ejecutivo"]; 

                        Entity auxProspecto = new Entity("cr8e5_prospecto")
                        {
                            Id = prospecto.Id,
                        };

                        auxProspecto["cr8e5_ejecutivo"] = ejecutivo;

                        // Actualizar el configurador producto y el prospecto
                        service.Update(aux);
                        service.Update(auxProspecto);

                        // Iniciar registro de los productos ofrecidos al mismo prospecto
                        Entity auxProductos = new Entity("cr8e5_auxproductosofrecidos");
                        auxProductos.Attributes["cr8e5_prospecto"] = new EntityReference("cr8e5_prospecto", prospecto.Id);
                        auxProductos.Attributes["cr8e5_productoaofrecer"] = (EntityReference)prospecto.Attributes["cr8e5_productoaofrecer"];

                        service.Create(auxProductos);
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
