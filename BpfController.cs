using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionProspecto
{
    public class BpfController : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtener los servicios de Dynamics 365
            ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtener el contexto de ejecución en el que se llamó al plugin
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Servicios de referencia
            IOrganizationServiceFactory serviceFactory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                Entity entity = (Entity)context.InputParameters["Target"];
                    
                Entity Bpf = GetActiveBPFDetails(entity, service);

                Guid bpfActivo = Bpf.Id;
                Guid faseActual = new Guid(Bpf.Attributes["processstageid"].ToString());
                int posicionActual = -1;
                RetrieveActivePathResponse pathResponse = GetAllStagesOfSelectedBPF(bpfActivo, faseActual, ref posicionActual, service);

                if (pathResponse.ProcessStages != null)
                {
                    Guid faseSiguiente = (Guid)pathResponse.ProcessStages.Entities[posicionActual + 1].Attributes["processstageid"];
                    string nombreFaseSiguiente = pathResponse.ProcessStages.Entities[posicionActual + 1].Attributes["stagename"].ToString();

                    Entity entBpf = new Entity("cr8e5_ofertatest")
                    {
                        Id = bpfActivo
                    };

                    if (nombreFaseSiguiente.ToLower().Equals("rechazado"))
                    {
                        faseSiguiente = (Guid)pathResponse.ProcessStages.Entities[posicionActual + 2].Attributes["processstageid"];
                        nombreFaseSiguiente = pathResponse.ProcessStages.Entities[posicionActual + 2].Attributes["stagename"].ToString();
                    }

                    if (nombreFaseSiguiente.ToLower().Equals("finalizado"))
                    {
                        entBpf["statuscode"] = new OptionSetValue(2);
                    }

                    entBpf["activestageid"] = new EntityReference("processstage", faseSiguiente);

                    service.Update(entBpf);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public Entity GetActiveBPFDetails(Entity entity, IOrganizationService crmService)
        {
            Entity activeProcessInstance = null;
            RetrieveProcessInstancesRequest entityBPFsRequest = new RetrieveProcessInstancesRequest
            {
                EntityId = entity.Id,
                EntityLogicalName = entity.LogicalName
            };
            RetrieveProcessInstancesResponse entityBPFsResponse = (RetrieveProcessInstancesResponse)crmService.Execute(entityBPFsRequest);
            if (entityBPFsResponse.Processes != null && entityBPFsResponse.Processes.Entities != null)
            {
                activeProcessInstance = entityBPFsResponse.Processes.Entities[0];
            }
            return activeProcessInstance;
        }

        public RetrieveActivePathResponse GetAllStagesOfSelectedBPF(Guid activeBPFId, Guid activeStageId, ref int currentStagePosition, IOrganizationService crmService)
        {
            // Retrieve the process stages in the active path of the current process instance
            RetrieveActivePathRequest pathReq = new RetrieveActivePathRequest
            {
                ProcessInstanceId = activeBPFId
            };
            RetrieveActivePathResponse pathResp = (RetrieveActivePathResponse)crmService.Execute(pathReq);
            for (int i = 0; i < pathResp.ProcessStages.Entities.Count; i++)
            {
                // Retrieve the active stage name and active stage position based on the activeStageId for the process instance
                if (pathResp.ProcessStages.Entities[i].Attributes["processstageid"].ToString() == activeStageId.ToString())
                {
                    currentStagePosition = i;
                }
            }
            return pathResp;
        }
    }
}
