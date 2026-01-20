using Microsoft.AspNetCore.Mvc.ModelBinding;
using Models;

public class VertexModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var routeData = bindingContext.ActionContext.RouteData;
        var vertexParam = routeData.Values.TryGetValue("vertex", out var value) ? value?.ToString() : null;

        if (string.IsNullOrEmpty(vertexParam))
        {
            bindingContext.ModelState.AddModelError("vertex", "Vertex parameter is required");
            return Task.CompletedTask;
        }

        var parts = vertexParam.Split(',');
        if (parts.Length != 2 || !int.TryParse(parts[0], out int row) || !int.TryParse(parts[1], out int col))
        {
            bindingContext.ModelState.AddModelError("vertex", "Vertex must be in format 'row,col'");
            return Task.CompletedTask;
        }

        bindingContext.Result = ModelBindingResult.Success(new Vertex(row, col));
        return Task.CompletedTask;
    }
}
