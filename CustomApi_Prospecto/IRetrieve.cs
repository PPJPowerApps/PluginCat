using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomApi_Prospecto
{
    internal interface IRetrieve
    {
        EntityCollection MultipleQuery(string entityName, ColumnSet columnSet, IOrganizationService Service, FilterExpression filterExpression = null, List<OrderExpression> orderExpressions = null);
    }
}
