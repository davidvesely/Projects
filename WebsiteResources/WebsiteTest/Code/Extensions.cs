namespace WebsiteTest.Code
{
    using System.Collections.Generic;

    public static class Extensions
    {
        public static bool ContainsResourceKey(this HashSet<ResourceGlobalization> resources, string key, string className)
        {
            foreach (var res in resources)
            {
                if (res.ResourceKey == key && res.ResourceType == className)
                {
                    return true;
                }
            }

            return false;
        }
    }
}