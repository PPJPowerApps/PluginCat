using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GestionProspecto
{
    public class ApiProductos : IPlugin
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

            // Texto aquí
            bool response = false;

            string prospecto = (string)context.InputParameters["entity"];
            string producto = (string)context.InputParameters["proAttr"];

            if (!string.IsNullOrEmpty(prospecto) && !string.IsNullOrEmpty(producto))
            {

                // Auxiliar productos
                QueryExpression queryAuxProducto = new QueryExpression("cr8e5_auxproductosofrecidos")
                {
                    ColumnSet = new ColumnSet("cr8e5_productoaofrecer")
                };

                // Agregar filtro para buscar solo el producto que fue asignado al prospecto
                queryAuxProducto.Criteria.AddCondition("cr8e5_prospecto", ConditionOperator.Equal, prospecto);
                queryAuxProducto.Criteria.AddCondition("cr8e5_productoaofrecer", ConditionOperator.Equal, producto);

                EntityCollection cantidadRegistros = service.RetrieveMultiple(queryAuxProducto);

                if (cantidadRegistros.Entities.Count == 0)
                {
                    response = true;
                }
            }
            
            context.OutputParameters["respuesta"] = response;
        }
    }
}
