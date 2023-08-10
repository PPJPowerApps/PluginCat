using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_Prospecto.Ejecutivo
{
    public interface IAssign
    {
        // Metodo de asginación
        Entity AssignEjecutivo(Entity prospecto, IOrganizationService service);
    }
}
