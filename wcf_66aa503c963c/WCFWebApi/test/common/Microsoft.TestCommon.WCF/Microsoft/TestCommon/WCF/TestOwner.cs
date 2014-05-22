// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF
{
    /// <summary>
    /// The list of the owners name mapping from feature names
    /// </summary>
    public static class TestOwner
    {
        public const string WebAPITestLead = "janezhou";
        public const string Unassign = WebAPITestLead;

        public const string HttpBinding = "kichalla";
        public const string HttpMemoryChannel = "trdai";
        public const string HttpMessageHandler = "dravva";
        public const string HttpMimeMultipart = "trdai";        
        public const string HttpOperationHandler = "maying";
        public const string HttpPrimitiveServiceModel = "trdai";
        public const string OData = "maying";
        public const string OperationSelector  = "vinelap";
        public const string ProgrammingModel = "vinelap";
        public const string QueryComposition = "dravva";
        
        /// Need update
        public const string WindowsCredentials      = Unassign;
        public const string ScenarioTests           = Unassign;
        public const string Service                 = Unassign;
        public const string WebService              = Unassign;
        public const string HttpMessageEncoder      = Unassign;
        public const string HttpMessageExtension    = Unassign;
        public const string HttpMessage             = Unassign; // Http Primitive? Troy?
        public const string HttpTransportSecurity   = Unassign;
        public const string HttpParameterCollection = Unassign;
        public const string HttpBindingSecurity     = Unassign;
        public const string DataMember              = Unassign;
        public const string DataContract            = Unassign;
        public const string ClassDataContract       = Unassign;
    }
}
