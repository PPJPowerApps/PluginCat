using Microsoft.Xrm.Sdk;
using Plugin_Prospecto.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_Prospecto.Prospecto
{   
    // Lógica de actualización
    internal class UpdateEntity : IUpdate
    {
        public void Update(Entity entity, IOrganizationService service, EntityReferenceCollection ejecutivo = null)
        {
            // Definir la entidad a actualizar mediante su nombre lógico e ID
            var auxEntity = new Entity(entity.LogicalName)
            {
                Id = entity.Id,
            };

            // Añadir, si es que hay, las entidades de referencia
            ejecutivo?.OrderBy(o => o.LogicalName).ToList().ForEach(item => auxEntity.Attributes[item.LogicalName] = item);

            // Utiliza el contexto de ejecución del plugin para actualizar
            service.Update(auxEntity);
        }
    }
}
