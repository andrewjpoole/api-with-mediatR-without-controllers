using System;

namespace AJP.MediatrEndpoints.PropertyAttributes
{
    public class SwaggerExampleAttribute: Attribute
    {
        public string Example { get; }

        public SwaggerExampleAttribute(string example)
        {
            Example = example;
        }
    }
}