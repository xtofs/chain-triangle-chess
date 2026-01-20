using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new VertexModelBinderProvider());
});

// Register shared services
builder.Services.AddSingleton(new TriangleGeometry(9, 50, 20, 20));
builder.Services.AddSingleton<TriangleBoard>();

// Allow synchronous IO for SVG generation
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

var app = builder.Build();

var env = app.Services.GetRequiredService<IWebHostEnvironment>();

app.UseDefaultFiles(new DefaultFilesOptions
{
    FileProvider = new PhysicalFileProvider(env.WebRootPath),
    DefaultFileNames = new[] { "index.html" }
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(env.WebRootPath),
    RequestPath = ""
});

app.MapGet("/api/board", GetBoard);

app.MapGet("/api/game", GetGame);

app.MapGet("/api/reachable/{vertex}", GetReachableVertices);

app.MapPost("/api/place-band/{vertex1}/{vertex2}", PlaceBand);

app.Run();

IResult GetReachableVertices(Vertex vertex, ITriangleGrid grid)
{
    var reachable = grid.GetReachableVertices(vertex);
    return Results.Json(reachable.Select(v => new { v.Row, v.Col }));
}

IResult PlaceBand(Vertex vertex1, Vertex vertex2, TriangleBoard board)
{
    Console.WriteLine($"Placing band from {vertex1} to {vertex2}");
    board.AddBand(vertex1, vertex2);
    return Results.Ok();
}

IResult GetBoard(TriangleGeometry geom)
{
    var renderer = new SvgRenderer(geom);

    return Results.Stream(renderer.RenderBoard, contentType: "image/svg+xml");
}

IResult GetGame(TriangleGeometry geom, TriangleBoard board)
{
    var renderer = new SvgRenderer(geom);

    async Task Action(Stream stream)
    {
        await renderer.RenderGame(stream, board);
    }

    return Results.Stream(Action, contentType: "image/svg+xml");
}
