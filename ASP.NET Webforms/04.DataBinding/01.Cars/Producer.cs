using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;

namespace _01.Cars
{
    public class Producer
    {
        public string Name { get; set; }

        public List<Model> Models { get; set; }
    }
}