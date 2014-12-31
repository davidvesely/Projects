using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteTest.Code
{
    public class FakeDB
    {
        private static List<ResourceGlobalization> table;

        public List<ResourceGlobalization> ResourceGlobalizations { get { return table; } }

        static FakeDB()
        {
            table = new List<ResourceGlobalization>();
            InitDb();
        }

        public static void InitDb()
        {
            table.Add(new ResourceGlobalization()
            {
                CultureCode = string.Empty,
                ResourceKey = "CustomText",
                ResourceValue = "This is the english version of the resx1.",
                ResourceType = "Resource",
            });

            table.Add(new ResourceGlobalization()
            {
                CultureCode = "bg-BG",
                ResourceKey = "CustomText",
                ResourceValue = "Български текст исахдяуъ лсадхсиуя ъеасх кйдгя8ув хдакйс хда.",
                ResourceType = "Resource",
            });

            table.Add(new ResourceGlobalization()
            {
                CultureCode = string.Empty,
                ResourceType = "About.aspx",
                ResourceKey = "ASPxButton1Resource1.Text",
                ResourceValue = "A Button",
            });

            table.Add(new ResourceGlobalization()
            {
                CultureCode = string.Empty,
                ResourceType = "About.aspx",
                ResourceKey = "ASPxLabel1Resource1.Text",
                ResourceValue = "English label",
            });

            table.Add(new ResourceGlobalization()
            {
                CultureCode = string.Empty,
                ResourceType = "About.aspx",
                ResourceKey = "PageResource1.Title",
                ResourceValue = "About",
            });

            table.Add(new ResourceGlobalization()
            {
                CultureCode = "bg-BG",
                ResourceType = "About.aspx",
                ResourceKey = "ASPxButton1Resource1.Text",
                ResourceValue = "Бутон",
            });

            table.Add(new ResourceGlobalization()
            {
                CultureCode = "bg-BG",
                ResourceType = "About.aspx",
                ResourceKey = "ASPxLabel1Resource1.Text",
                ResourceValue = "Български лейбъл",
            });

            table.Add(new ResourceGlobalization()
            {
                CultureCode = "bg-BG",
                ResourceType = "About.aspx",
                ResourceKey = "PageResource1.Title",
                ResourceValue = "Относно...",
            });
        }
    }
}