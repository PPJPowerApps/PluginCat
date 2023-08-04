using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Plugin_Prospecto.Entities;
using Plugin_Prospecto.Prospecto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_Prospecto
{
    public class PReasignarEjecutivo : Services, IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            SetServices(serviceProvider);
            if (Context.InputParameters.Contains("Target") &&
                Context.InputParameters["Target"] is Entity)
            {
                try
                {
                    // Obtener la entidad target post operación
                    var postProspecto = (Entity)Context.InputParameters["Target"];
                    var postProducto = (EntityReference)postProspecto.Attributes[NombreEntidades.PRODUCTO];

                    // Obtener la enteidad target pre opración
                    var preProspecto = Context.PreEntityImages["PreProspecto"];
                    var preProducto = (EntityReference)preProspecto.Attributes[NombreEntidades.PRODUCTO];
                    var preEjecutivo = (EntityReference)preProspecto.Attributes[NombreEntidades.EJECUTIVO];

                    // Obtener el configurador del producto nuevo
                    var columnSet = new ColumnSet(NombreEntidades.EJECUTIVO, NombreEntidades.CONTADOR);
                    var filterExpression = new FilterExpression();
                    filterExpression.AddCondition(NombreEntidades.PRODUCTO, ConditionOperator.Equal, postProducto.Id);
                    var orderExpressions = new List<OrderExpression>
                    {
                        new OrderExpression(NombreEntidades.CONTADOR, OrderType.Ascending)
                    };
                    var entityCollection = MultipleQuery(NombreEntidades.CONFIGURADORPRODUCTO, columnSet, filterExpression, orderExpressions);

                    var updater = new UpdaterController(new UpdateEntity());
                    // Actualizar el controlador producto y asignar el ejecutivo
                    if (entityCollection.Entities.Count > 0)
                    {
                        var entityRefCollection = new EntityReferenceCollection();

                        var controladorProducto = entityCollection.Entities[0];
                        Utility.UpdateCounter(ref controladorProducto, NombreEntidades.CONTADOR, 1);
                        updater.Execute(controladorProducto, Service);

                        entityRefCollection.Add((EntityReference)controladorProducto[NombreEntidades.EJECUTIVO]);
                        updater.Execute(postProspecto, Service, entityRefCollection);

                    }

                    // Obtener el configurador del producto antiguo
                    filterExpression = new FilterExpression();
                    filterExpression.AddCondition(NombreEntidades.EJECUTIVO, ConditionOperator.Equal, preEjecutivo.Id);
                    filterExpression.AddCondition(NombreEntidades.PRODUCTO, ConditionOperator.Equal, preProducto.Id);
                    entityCollection = MultipleQuery(NombreEntidades.CONFIGURADORPRODUCTO, columnSet, filterExpression);

                    // Actualizar el controlador producto antiguo y recudir el contador de ejecutivo en 1
                    var auxConfiguradorProducto = entityCollection.Entities[0];
                    Utility.UpdateCounter(ref auxConfiguradorProducto, NombreEntidades.CONTADOR, -1);
                    updater.Execute(auxConfiguradorProducto, Service);

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
