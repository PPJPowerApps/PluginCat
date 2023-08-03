using CustomApi_Cliente;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace CustomApi_Cliente
{
    public class ApiListarProductos : Services
    {
        public override void Execute(IServiceProvider serviceProvider)
        {
            SetServices(serviceProvider);

            // Parametros de entrada de la API
            Guid clienteGuid = (Guid)Context.InputParameters["id"];

            if (clienteGuid != Guid.Empty)
            {
                try
                {
                    // Obtener todos los productos
                    ColumnSet cs = new ColumnSet("cr8e5_productoaofrecerid", "cr8e5_name", "cr8e5_fechavigencia");
                    EntityCollection productos = MultipleQuery("cr8e5_productoaofrecer", cs, Service);

                    // Obtener los productos que se le han ofrecido al cliente
                    cs = new ColumnSet("cr8e5_productoaofrecer", "cr8e5_estadooferta");
                    FilterExpression filterExpression = new FilterExpression();
                    filterExpression.AddCondition("cr8e5_contact", ConditionOperator.Equal, clienteGuid);

                    filterExpression.AddCondition("cr8e5_estadooferta", ConditionOperator.Between, 0, 1);
                    EntityCollection clienteProductoAux = MultipleQuery("cr8e5_clienteproducto", cs, Service, filterExpression);

                    var productosOfrecidos = clienteProductoAux.Entities.Select(x => (EntityReference)x.Attributes["cr8e5_productoaofrecer"]);

                    EntityCollection ofertasCliente = new EntityCollection();

                    foreach (var item in productos.Entities)
                    {
                        bool existe = productosOfrecidos.Any(x => x.Id.Equals(item.Id));

                        if (!existe)
                        {
                            ofertasCliente.Entities.Add(item);
                        }
                    }

                    Context.OutputParameters["productos"] = ofertasCliente;
                    Context.OutputParameters["idcliente"] = clienteGuid;

                }
                catch (Exception)
                {

                    throw;
                }
            }

        }
    }
}
