using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_Prospecto.Prospecto
{
    // Interfaz encargada de actualizar entidades
    internal interface IUpdate
    {
        void Update(Entity prospecto, IOrganizationService service, EntityReferenceCollection entityReference = null);
    }
}
