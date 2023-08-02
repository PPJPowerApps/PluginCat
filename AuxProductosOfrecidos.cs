using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_Prospecto
{
    internal class AuxProductosOfrecidos
    {
       public static void CreateAuxProductos(Guid prospecto, EntityReference cr8e5_productoaofrecer, IOrganizationService service)
       {
            // Iniciar registro de los productos ofrecidos al mismo prospecto
            Entity auxProductos = new Entity("cr8e5_auxproductosofrecidos");
            auxProductos.Attributes["cr8e5_prospecto"] = new EntityReference("cr8e5_prospecto", prospecto);
            auxProductos.Attributes["cr8e5_productoaofrecer"] = cr8e5_productoaofrecer;

            service.Create(auxProductos);
        }
    }
}
