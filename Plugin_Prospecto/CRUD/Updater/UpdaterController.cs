using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_Prospecto.Prospecto
{
    // Controlador del plugin
    internal class UpdaterController
    {
        // Variables
        private IUpdate prospectoUpdate;
        internal IUpdate ProspectoUpdate { get => prospectoUpdate; set => prospectoUpdate = value; }

        public UpdaterController(IUpdate update) {  ProspectoUpdate = update; }

        // Ejecutar la lógica de actualización
        public void Execute(Entity entity, IOrganizationService service, EntityReferenceCollection entityReferences = null)
        {
            prospectoUpdate.Update(entity, service, entityReferences);
        }
    }
}
