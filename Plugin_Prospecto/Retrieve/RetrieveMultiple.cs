using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_Prospecto
{
    public class RetrieveMultiple
    {
        // Obtener multiples valores o un valor si se desconoce el id de la entidad consultada
        public static EntityCollection MultipleQuery(string entityName, IOrganizationService service, ColumnSet columnSet, FilterExpression filterExpression = null, List<OrderExpression> orderExpressions = null)
        {
            // Entidad a consultar y columnas a obtener
            var query = new QueryExpression(entityName)
            {
                ColumnSet = columnSet,
            };

            // Añadir filtros si existen 
            if (filterExpression != null) { query.Criteria = filterExpression; }

            // Añadir orden si existen 
            orderExpressions?.OrderBy(o => o.AttributeName).ThenBy(o => o.OrderType).ToList().ForEach(item => query.AddOrder(item.AttributeName, item.OrderType));
            
            // Eejecutar y retornar los valores
            EntityCollection entityCollection = service.RetrieveMultiple(query);
            return entityCollection;
        }
    }
}
