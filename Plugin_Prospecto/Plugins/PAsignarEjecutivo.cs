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
    public class PAsignarEjecutivo : Services
    {
        public override void Execute(IServiceProvider serviceProvider)
        {

            SetServices(serviceProvider);
            if (Context.InputParameters.Contains("Target") &&
                Context.InputParameters["Target"] is Entity)
            {
                try
                {
                    // Obtener la entidad target 
                    Entity prospecto = (Entity)Context.InputParameters["Target"];
                    EntityReference producto = (EntityReference)prospecto.Attributes["cr8e5_productoaofrecer"];

                    // Obtener el configurador del producto
                    ColumnSet columnSet = new ColumnSet("cr8e5_ejecutivo", "cr8e5_contador");
                    FilterExpression filterExpression = new FilterExpression();
                    filterExpression.AddCondition("cr8e5_productoaofrecer", ConditionOperator.Equal, producto.Id);
                    List<OrderExpression> orderExpressions = new List<OrderExpression>
                        {
                            new OrderExpression("cr8e5_contador", OrderType.Ascending)
                        };
                    EntityCollection entityCollection = MultipleQuery("cr8e5_configuradordeproducto", columnSet, Service,filterExpression, orderExpressions);


                    if (entityCollection.Entities.Count > 0)
                    {
                        UpdateControladorProspecto(entityCollection.Entities[0], prospecto, producto, 1);
                    }

                }
                catch (Exception ex)
                {
                    TracingService.Trace("Error of Plugin: {0}", ex.ToString());
                    throw;
                }
            }
        }
    }
}
