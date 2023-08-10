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
    // Estategia para desasignar un ejecutivo a un prospecto, para reducir su contador
    public class UnassignEjecutive : IAssign
    {
        public Entity AssignEjecutivo(Entity prospecto, IOrganizationService service)
        {
            // Extraer el producto y ejecutivo antiguo 
            var ejecutivo = (EntityReference)prospecto.Attributes[NombreEntidades.EJECUTIVO];
            var producto = (EntityReference)prospecto.Attributes[NombreEntidades.PRODUCTO];

            // Preparar la query, en base al producto y el ejecutivo
            var columnSet = new ColumnSet(NombreEntidades.EJECUTIVO, NombreEntidades.CONTADOR);
            var filterExpression = new FilterExpression();
            filterExpression.AddCondition(NombreEntidades.EJECUTIVO, ConditionOperator.Equal, ejecutivo.Id);
            filterExpression.AddCondition(NombreEntidades.PRODUCTO, ConditionOperator.Equal, producto.Id);

            // Ejecutar la query
            var entityCollection = RetrieveMultiple.MultipleQuery(NombreEntidades.CONFIGURADORPRODUCTO, service, columnSet, filterExpression);

            var configuradorProducto = entityCollection.Entities?.First();

            // Retornar el configurador de producto con el contador disminuido en 1
            return Utility.CreateAuxConfigurador(configuradorProducto, -1);
        }

    }
}
