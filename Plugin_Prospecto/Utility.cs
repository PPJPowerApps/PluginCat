using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Plugin_Prospecto.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_Prospecto
{
    public class Utility
    {
        // Actualizar contador de configurador producto
        public static void UpdateCounter(ref Entity entity, string attribute, int value)
        {
            int currentCounter = int.Parse(entity.Attributes[attribute].ToString());
            entity.Attributes[attribute] = currentCounter + value;
        }
        // Crear una entidad auxiliar de la entidad AuxProductosOfrecidos para crear un nuevo registro
        public static Entity CreateAuxProducto(Entity prospecto)
        {
            var producto = (EntityReference)prospecto.Attributes[NombreEntidades.PRODUCTO];

            var auxProductos = new Entity(NombreEntidades.AUXPRODUCTOSOFRECIDOS);
            auxProductos.Attributes[NombreEntidades.PROSPECTO] = new EntityReference(NombreEntidades.PROSPECTO, prospecto.Id);
            auxProductos.Attributes["cr8e5_productoaofrecer"] = producto;

            return auxProductos;
        }
        // Crear una entidad auxiliar de Configurador de producto para actualizar un registro existente
        public static Entity CreateAuxConfigurador(Entity configuradorProducto, int counter)
        {
            var auxEntity = new Entity(configuradorProducto.LogicalName)
            {
                Id = configuradorProducto.Id,
            };
            auxEntity.Attributes[NombreEntidades.EJECUTIVO] = (EntityReference)configuradorProducto.Attributes[NombreEntidades.EJECUTIVO];
            auxEntity.Attributes[NombreEntidades.CONTADOR] = configuradorProducto.Attributes[NombreEntidades.CONTADOR];
            Utility.UpdateCounter(ref auxEntity, NombreEntidades.CONTADOR, counter);
            return auxEntity;
        }
    }
}
