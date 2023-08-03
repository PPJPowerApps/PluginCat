using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_Prospecto
{
    public class PReasignarEjecutivo : Services
    {
        public override void Execute(IServiceProvider serviceProvider)
        {
            SetServices(serviceProvider);
            if (Context.InputParameters.Contains("Target") &&
                Context.InputParameters["Target"] is Entity)
            {
                // Obtener la entidad target post operación
                Entity postProspecto = (Entity)Context.InputParameters["Target"];
                EntityReference postProducto = (EntityReference)postProspecto.Attributes["cr8e5_productoaofrecer"];

                // Obtener la enteidad target pre opración
                Entity preProspecto = Context.PreEntityImages["PreProspecto"];
                EntityReference preProducto = (EntityReference)preProspecto.Attributes["cr8e5_productoaofrecer"];
                EntityReference preEjecutivo = (EntityReference)preProspecto.Attributes["cr8e5_ejecutivo"];

                // Obtener el configurador del producto
                ColumnSet columnSet = new ColumnSet("cr8e5_ejecutivo", "cr8e5_contador");
                FilterExpression filterExpression = new FilterExpression();
                filterExpression.AddCondition("cr8e5_productoaofrecer", ConditionOperator.Equal, postProducto.Id);
                List<OrderExpression> orderExpressions = new List<OrderExpression>
                    {
                        new OrderExpression("cr8e5_contador", OrderType.Ascending)
                    };
                EntityCollection entityCollection = MultipleQuery("cr8e5_configuradordeproducto", columnSet, Service, filterExpression, orderExpressions);

                if (entityCollection.Entities.Count > 0)
                {
                    UpdateControladorProspecto(entityCollection.Entities[0], postProspecto, postProducto, 1);
                    AuxProductosOfrecidos.CreateAuxProductos(postProspecto.Id, postProducto, Service);

                }

                filterExpression = new FilterExpression();
                filterExpression.AddCondition("cr8e5_ejecutivo", ConditionOperator.Equal, preEjecutivo.Id);
                filterExpression.AddCondition("cr8e5_productoaofrecer", ConditionOperator.Equal, preProducto.Id);
                entityCollection = MultipleQuery("cr8e5_configuradordeproducto", columnSet, Service, filterExpression);

                Entity auxConfiguradorProducto = entityCollection.Entities[0];
                Utility.UpdateCounter(ref auxConfiguradorProducto, "cr8e5_contador", -1);
                Service.Update(auxConfiguradorProducto);
            }
        }
    }
}
