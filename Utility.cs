using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_Prospecto
{
    public class Utility
    {
        public static void UpdateCounter(ref Entity entity, string attribute, int value)
        {
            int currentCounter = int.Parse(entity.Attributes[attribute].ToString());
            entity.Attributes[attribute] = currentCounter + value;
        }
    }
}
