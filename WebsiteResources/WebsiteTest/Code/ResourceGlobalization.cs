using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebsiteTest.Code
{
    public class ResourceGlobalization
    {
        public string ResourceKey { get; set; }
        public string ResourceType { get; set; }
        public string ResourceValue { get; set; }
        public string CultureCode { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as ResourceGlobalization;
            if (other == null)
                return false;

            if (ResourceKey == other.ResourceKey
                && ResourceType == other.ResourceType
                && ResourceValue == other.ResourceValue
                && CultureCode == other.CultureCode)
                return true;
            else
                return false;
        }
    }
}
