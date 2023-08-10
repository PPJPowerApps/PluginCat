using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Plugin_Prospecto.Ejecutivo;
using Plugin_Prospecto.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Services;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_Prospecto
{
    public class PAsignarEjecutivo : Services, IPlugin
    {
        // Asignar el ejecutivo que se hará cargo del prospecto en base al producto ofrecido
        public void Execute(IServiceProvider serviceProvider)
        {
            // Asignar el contexto de ejecución del plugin
            SetServices(serviceProvider);
            if (Context.InputParameters.Contains("Target") &&
                Context.InputParameters["Target"] is Entity)
            {
                try
                {
                    // Obtener la entidad target 
                    var prospecto = (Entity)Context.InputParameters["Target"];
                    var producto = (EntityReference)prospecto.Attributes[NombreEntidades.PRODUCTO];

                    // Obtener el configurador del producto relacionado al prospecto
                    var strategy = new AssignStrategy(new AssignEjecutiveFewestProducts());
                    var configuradorProducto = strategy.Execute(prospecto, Service);

                    // Actualizar el controlador producto y asignar el ejecutivo
                    if (configuradorProducto != null)
                    {
                        // Entidades de referencia para actualizar
                        Service.Update(configuradorProducto);
                        Service.Update(CreateAuxProspecto(prospecto, configuradorProducto));
                        Service.Create(Utility.CreateAuxProducto(prospecto));
                    }

                }
                catch (Exception ex)
                {
                    TracingService.Trace("Error of Plugin: {0}", ex.ToString());
                    throw;
                }
            }
        }
        // Crear un prospecto auxiliar con un solo valor, ejecutivo
        private Entity CreateAuxProspecto(Entity prospecto, Entity configuradorProducto)
        {
            Entity auxProspecto = new Entity(prospecto.LogicalName)
            {
                Id = prospecto.Id
            };
            // Asginar el ejecutivo desde el configurador de producto
            auxProspecto.Attributes[NombreEntidades.EJECUTIVO] = (EntityReference)configuradorProducto.Attributes[NombreEntidades.EJECUTIVO];
            return auxProspecto;
        }
    }
}
