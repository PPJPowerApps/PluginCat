using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Plugin_Prospecto.Entities;

namespace Plugin_Prospecto
{
    internal class UpdateCounter
    {
        public static CalculateRollupFieldResponse RollUpCounter(Guid guid, IOrganizationService service)
        {
            var calculateRollUp = new CalculateRollupFieldRequest
            {
                Target = new EntityReference(NombreEntidades.CONFIGURADORPRODUCTO, guid),
                FieldName = NombreEntidades.CONTADOR
            };

            var response = (CalculateRollupFieldResponse)service.Execute(calculateRollUp);

            return response;
        }
    }
}
