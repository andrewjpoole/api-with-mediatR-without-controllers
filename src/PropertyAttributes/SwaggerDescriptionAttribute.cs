using System;

namespace AJP.MediatrEndpoints.PropertyAttributes
{
    public class SwaggerDescriptionAttribute: Attribute
    {
        public string Description { get; }
        public bool Deprecated { get; }

        public SwaggerDescriptionAttribute(string description, bool deprecated = false)
        {
            Description = description;
            Deprecated = deprecated;
        }
    }
}