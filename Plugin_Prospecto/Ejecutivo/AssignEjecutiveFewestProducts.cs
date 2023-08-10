using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Plugin_Prospecto.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_Prospecto.Ejecutivo
{
    // Estategia para asignar un ejecutivo a un prospecto, donde se selecciona el ejecutivo que menos productos está gestionando
    public class AssignEjecutiveFewestProducts : IAssign
    {
        public Entity AssignEjecutivo(Entity prospecto, IOrganizationService service)
        {
            // Extraer el producto nuevo
            var producto = (EntityReference)prospecto.Attributes[NombreEntidades.PRODUCTO];
            
            // Preparar la query, en base al producto
            var columnSet = new ColumnSet(NombreEntidades.EJECUTIVO, NombreEntidades.CONTADOR);
            var filterExpression = new FilterExpression();
            filterExpression.AddCondition(NombreEntidades.PRODUCTO, ConditionOperator.Equal, producto.Id);
            
            // Ordenaro por contador 
            var orderExpressions = new List<OrderExpression>
                    {
                        new OrderExpression(NombreEntidades.CONTADOR, OrderType.Ascending)
                    };

            // Ejecutar la query
            var entityCollection = RetrieveMultiple.MultipleQuery(NombreEntidades.CONFIGURADORPRODUCTO, service, columnSet, filterExpression, orderExpressions);

            var configuradorProducto = entityCollection.Entities?.First();                  
            
            // Retornar el configurador de producto con el contador aumentado en 1
            return Utility.CreateAuxConfigurador(configuradorProducto, 1);

        }

    }
}
