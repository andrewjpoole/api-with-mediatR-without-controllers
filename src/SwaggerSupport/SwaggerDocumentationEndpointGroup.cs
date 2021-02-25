namespace AJP.MediatrEndpoints.SwaggerSupport
{
    public class SwaggerDocumentationEndpointGroup 
    {
        public string Name { get; }
        public string Path { get; }
        public string Description { get; }

        public SwaggerDocumentationEndpointGroup(string name, string path, string description)
        {
            Name = name;
            Path = path.StartsWith("/") ? path : $"/{path}";
            Description = description;
        }
    }
}
