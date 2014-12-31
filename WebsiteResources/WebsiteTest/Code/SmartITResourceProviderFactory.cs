using System;
using System.Web.Compilation;

namespace WebsiteTest.Code
{
    /// <summary>
    /// Summary description for SmartITResourceProviderFactory
    /// </summary>
    public class SmartITResourceProviderFactory : ResourceProviderFactory
    {

        public override IResourceProvider CreateGlobalResourceProvider(string classKey)
        {
            return new SmartITResourceProvider(classKey, classKey, ResxFactoryType.Global);
        }

        public override IResourceProvider CreateLocalResourceProvider(string virtualPath)
        {
            string classKey = virtualPath;
            if (!string.IsNullOrEmpty(virtualPath))
            {
                classKey = virtualPath.Remove(0, 1);
                //classKey = virtualPath.Remove(0, virtualPath.IndexOf('/') + 1);
            }
            return new SmartITResourceProvider(classKey, virtualPath, ResxFactoryType.Local);
        }
    }
}