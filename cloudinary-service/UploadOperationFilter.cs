using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class UploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.RequestBody == null)
            operation.RequestBody = new OpenApiRequestBody();

        operation.RequestBody.Content.Add("multipart/form-data", new OpenApiMediaType
        {
            Schema = new OpenApiSchema
            {
                Type = "object",
                Properties =
                {
                    ["image"] = new OpenApiSchema
                    {
                        Type = "string",
                        Format = "binary"
                    }
                },
                Required = new HashSet<string> { "image" }
            }
        });
    }
}
