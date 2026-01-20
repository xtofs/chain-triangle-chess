using Microsoft.AspNetCore.Mvc.ModelBinding;
using Models;

public class VertexModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(Vertex))
        {
            return new VertexModelBinder();
        }

        return null;
    }
}
