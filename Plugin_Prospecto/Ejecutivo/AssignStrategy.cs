using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_Prospecto.Ejecutivo
{
    public class AssignStrategy
    {
        // Variables
        private IAssign _assign;
        public IAssign Assign { get => _assign; set => _assign = value; }

        // Constructores
        public AssignStrategy() { }
        public AssignStrategy(IAssign assign) => Assign = assign;

        // Ejecutar estrategia
        public Entity Execute(Entity guid, IOrganizationService service)
        {
            return Assign.AssignEjecutivo(guid, service);
        }

        // Cambiar estrategia
        public void ChangeStrategy(IAssign assign) => Assign = assign;
        
    }
}
