using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace CustomApi_Prospecto.CustomAPI
{
    public class ApiProductoOfrecido : Services
    {
        public override void Execute(IServiceProvider serviceProvider)
        {
            SetServices(serviceProvider);

            // Parametros de entrada

            EntityReference prospecto = (EntityReference)Context.InputParameters["entity"];
            EntityReference producto = (EntityReference)Context.InputParameters["proAttr"];


            // Parametro de respuesta
            bool response = false;

            if (!producto.Equals(null) && !prospecto.Equals(null))
            {
                try
                {
                    ColumnSet cs = new ColumnSet(producto.LogicalName);

                    FilterExpression fe = new FilterExpression();
                    fe.AddCondition(prospecto.LogicalName, ConditionOperator.Equal, prospecto.Id);
                    fe.AddCondition(producto.LogicalName, ConditionOperator.Equal, producto.Id);

                    EntityCollection cantidadProductosOfrecidos = MultipleQuery("cr8e5_auxproductosofrecidos", cs, Service, fe);

                    if (cantidadProductosOfrecidos.Entities.Count == 0)
                    {
                        response = true;
                    }
                }
                catch (Exception ex)
                {
                    TracingService.Trace("Error of Plugin: {0}", ex.ToString());
                    throw;
                }

            }
            Context.OutputParameters["respuesta"] = response;

        }
    }
}
