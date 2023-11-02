using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Plugin_Prospecto.Ejecutivo;
using Plugin_Prospecto.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_Prospecto
{
    public class PReasignarEjecutivo : Services, IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            SetServices(serviceProvider);
            if (Context.InputParameters.Contains("Target") &&
                Context.InputParameters["Target"] is Entity)
            {
                try
                {
                    // Obtener la entidad target post operación
                    var postProspecto = (Entity)Context.InputParameters["Target"];
                    var postProducto = (EntityReference)postProspecto.Attributes[NombreEntidades.PRODUCTO];

                    // Obtener la enteidad target pre opración
                    var preProspecto = Context.PreEntityImages["PreProspecto"];
                    var preProducto = (EntityReference)preProspecto.Attributes[NombreEntidades.PRODUCTO];
                    var preEjecutivo = (EntityReference)preProspecto.Attributes[NombreEntidades.EJECUTIVO];

                    // Obtener el ejecutivo con menos productos asignados
                    var strategy = new AssignStrategy(new AssignEjecutiveFewestProducts());
                    var configuradorProducto = strategy.Execute(postProspecto, Service);

                    // Actualizar el controlador producto y asignar el ejecutivo
                    if (configuradorProducto != null)
                    {
                        Service.Update(configuradorProducto);
                        Service.Update(CreateAuxProspecto(postProspecto, configuradorProducto));
                        Service.Create(Utility.CreateAuxProducto(postProspecto));
                    }

                    // Actualizar el controlador producto antiguo y recudir el contador de ejecutivo en 1
                    strategy.ChangeStrategy(new UnassignEjecutive());
                    var auxConfiguradorProducto = strategy.Execute(preProspecto, Service);
                    Service.Update(auxConfiguradorProducto);

                }
                catch (Exception ex)
                {
                    TracingService.Trace("Error of Plugin: {0}", ex.ToString());
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
