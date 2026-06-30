using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

public class SecurityRequirementsTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var components = document.Components ?? new OpenApiComponents();

        components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Name         = "Authorization",
            Type         = SecuritySchemeType.Http,
            Scheme       = "bearer",
            BearerFormat = "JWT",
            In           = ParameterLocation.Header,
            Description  = "Informe o token JWT no formato: Bearer {token}"
        };

        document.Components = components;

        var schemeReference = new OpenApiSecuritySchemeReference("Bearer", document);

        document.Security ??= new List<OpenApiSecurityRequirement>();
        document.Security.Add(new OpenApiSecurityRequirement
        {
            { schemeReference, new List<string>() }
        });

        return Task.CompletedTask;
    }
}
