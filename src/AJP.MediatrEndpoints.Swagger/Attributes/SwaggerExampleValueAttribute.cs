using System;

namespace AJP.MediatrEndpoints.Swagger.Attributes
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