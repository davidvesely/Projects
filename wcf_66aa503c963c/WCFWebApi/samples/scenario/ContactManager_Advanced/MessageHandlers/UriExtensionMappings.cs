// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace ContactManager_Advanced.MessageHandlers
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    [Export(typeof(IEnumerable<UriExtensionMapping>))]
    public class UriExtensionMappings : List<UriExtensionMapping>
    {
        public UriExtensionMappings()
        {
            this.AddMapping("xml", "application/xml");
            this.AddMapping("json", "application/json");
            this.AddMapping("png", "image/png");
            this.AddMapping("odata", "application/atom+xml");
            this.AddMapping("vcf", "text/directory");
            this.AddMapping("ics", "text/calendar");
        }
    }
}