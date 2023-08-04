using Microsoft.Xrm.Sdk;
using Plugin_Prospecto.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_Prospecto
{
    // Entidad AuxProductosOfrecidos
    internal class AuxProductosOfrecidos
    {
        // Crear nuevo registro en la entidad AuxProductosOfrecidos
        public static void CreateAuxProductos(Guid prospecto, EntityReference cr8e5_productoaofrecer, IOrganizationService service)
        {
            // Preparar valores
            var auxProductos = new Entity(NombreEntidades.AUXPRODUCTOSOFRECIDOS);
            auxProductos.Attributes[NombreEntidades.PROSPECTO] = new EntityReference(NombreEntidades.PROSPECTO, prospecto);
            auxProductos.Attributes["cr8e5_productoaofrecer"] = cr8e5_productoaofrecer;

            // Crear el registro en Dataverse
            service.Create(auxProductos);
        }
    }
}
