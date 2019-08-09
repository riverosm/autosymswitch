using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace AutoSymSwitch
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AutoSymSwitchRestWCF" in both code and config file together.
    public class AutoSymSwitchRestWCF : IAutoSymSwitchRestWCF
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public string XMLData(string id)
        {
            return Data(id);
        }
        public string JSONData(string id)
        {
            return Data(id);
        }

        private string Data(string id)
        {
            // logic
            return "Data: " + id;
        }
    }
}
