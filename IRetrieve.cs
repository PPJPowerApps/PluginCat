using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_Prospecto
{
    internal interface IRetrieve
    {
        EntityCollection MultipleQuery(string entityName, ColumnSet columnSet, IOrganizationService Service, FilterExpression filterExpression = null, List<OrderExpression> orderExpressions = null);
    }
}
