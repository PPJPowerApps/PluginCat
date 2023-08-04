using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Plugin_Prospecto.Entities;
using Plugin_Prospecto.Prospecto;
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
    public class PAsignarEjecutivo : Services, IPlugin
    {
        // Asignar el ejecutivo que se hará cargo del prospecto en base al producto ofrecido
        public void Execute(IServiceProvider serviceProvider)
        {
            // Asignar el contexto de ejecución del plugin
            SetServices(serviceProvider);
            if (Context.InputParameters.Contains("Target") &&
                Context.InputParameters["Target"] is Entity)
            {
                try
                {
                    // Obtener la entidad target 
                    var prospecto = (Entity)Context.InputParameters["Target"];
                    var producto = (EntityReference)prospecto.Attributes[NombreEntidades.PRODUCTO];

                    // Obtener el configurador del producto relacionado al prospecto
                    var columnSet = new ColumnSet(NombreEntidades.EJECUTIVO, NombreEntidades.CONTADOR);
                    var filterExpression = new FilterExpression();
                    filterExpression.AddCondition(NombreEntidades.PRODUCTO, ConditionOperator.Equal, producto.Id);
                    var orderExpressions = new List<OrderExpression>
                        {
                            new OrderExpression(NombreEntidades.CONTADOR, OrderType.Ascending)
                        };
                    var entityCollection = MultipleQuery(NombreEntidades.CONFIGURADORPRODUCTO, columnSet, filterExpression, orderExpressions);

                    // Actualizar el controlador producto y asignar el ejecutivo
                    if (entityCollection.Entities.Count > 0)
                    {
                        // Entidades de referencia para actualizar
                        var entityRefCollection = new EntityReferenceCollection();
                        
                        // Inicializar IUpdater
                        var updater = new UpdaterController(new UpdateEntity());

                        // Actualizar contador de la entidad configurador producto
                        var controladorProducto = entityCollection.Entities[0];
                        Utility.UpdateCounter(ref controladorProducto, NombreEntidades.CONTADOR, 1);
                        
                        // Actualizar entidades
                        updater.Execute(controladorProducto, Service);
                        entityRefCollection.Add((EntityReference)controladorProducto[NombreEntidades.EJECUTIVO]);
                        updater.Execute(prospecto, Service, entityRefCollection);

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
