using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionProspecto
{
    public class ControlarProducto : IPlugin
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

                    // Obtener el producto que se asignó
                    EntityReference producto = (EntityReference)prospecto.Attributes["cr8e5_productoaofrecer"];

                    // Configurador productos
                    QueryExpression query = new QueryExpression("cr8e5_auxproductosofrecidos")
                    {
                        ColumnSet = new ColumnSet("cr8e5_productoaofrecer")
                    };
                    // Agregar filtro para buscar solo el producto que fue asignado al prospecto en configurador producto
                    query.Criteria.AddCondition("cr8e5_productoaofrecer", ConditionOperator.Equal, producto.Id);
                    query.Criteria.AddCondition("cr8e5_prospecto", ConditionOperator.Equal, prospecto.Id);

                    EntityCollection cantidadRegistros = service.RetrieveMultiple(query);

                    // Si el valor es mayor a uno, significa que el producto nuevo ya ha sido ofrecido, se levanta la expeción
                    if (cantidadRegistros.Entities.Count > 0)
                    {
                        InvalidPluginExecutionException ex = new InvalidPluginExecutionException("El producto asignado ya ha sido asignado con anterioridad");
                        tracingService.Trace("TareasPlugin: {0}", ex.ToString());
                        throw ex;
                    }
                    // En caso de que el producto sea nuevo, la actualización ocurre normalmente y se crea el nuevo aux en base al producto anterior
                    else
                    {
                        Entity aux = new Entity("cr8e5_auxproductosofrecidos");
                        aux.Attributes["cr8e5_prospecto"] = new EntityReference("cr8e5_prospecto",prospecto.Id); 
                        aux.Attributes["cr8e5_productoaofrecer"] = (EntityReference)prospecto.Attributes["cr8e5_productoaofrecer"];

                        service.Create(aux);
                    }

                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
