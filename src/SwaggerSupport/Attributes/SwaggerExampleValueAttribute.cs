using System;

namespace AJP.MediatrEndpoints.SwaggerSupport.Attributes
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