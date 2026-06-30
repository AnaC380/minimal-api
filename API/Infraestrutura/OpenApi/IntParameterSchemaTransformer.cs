using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

public class IntParameterSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (schema.Type == (JsonSchemaType.Integer | JsonSchemaType.String))
        {
            schema.Type = JsonSchemaType.Integer;
            schema.Pattern = null;
        }

        return Task.CompletedTask;
    }
}
