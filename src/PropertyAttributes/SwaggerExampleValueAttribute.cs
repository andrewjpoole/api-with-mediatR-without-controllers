using System;

namespace AJP.MediatrEndpoints.PropertyAttributes
{
    public class SwaggerExampleValueAttribute: Attribute
    {
        public string Example { get; }

        public SwaggerExampleValueAttribute(string example)
        {
            Example = example;
        }
    }
}