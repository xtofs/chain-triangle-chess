using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Models;
using ctc.Rendering;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new VertexModelBinderProvider());
});

const int SIZE = 9;
// Register shared services
builder.Services.AddSingleton<TriangleGeometry>(_ => new TriangleGeometry(SIZE, 50, 20, 20));
builder.Services.AddSingleton<TriangleGrid>(_ => new TriangleGrid(SIZE));
builder.Services.AddSingleton<SvgRenderer>();
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

// get the static part of the svg, the board "background"
app.MapGet("/api/board", GetBoard);

// get the dynamic part of the svg, the game state (bands, pegs)
app.MapGet("/api/game", GetGame);

// get reachable vertices from a given vertex, for band placement
app.MapGet("/api/reachable/{vertex}", GetReachableVertices);

// place a band between two vertices
app.MapPost("/api/place-band/{vertex1}/{vertex2}", PlaceBand);

app.Run();

IResult GetReachableVertices(Vertex vertex, TriangleBoard board)
{
    var reachable = board.GetReachableVertices(vertex);
    return Results.Json(reachable.Select(v => new { v.Row, v.Col }));
}

IResult PlaceBand(Vertex vertex1, Vertex vertex2, TriangleBoard board)
{
    Console.WriteLine($"Placing band from {vertex1} to {vertex2}");
    // TODO: validate that this is a valid band and return result accordingly
    board.AddBand(vertex1, vertex2);
    return Results.Ok();
}

IResult GetBoard(SvgRenderer renderer)
{
    return Results.Stream(renderer.RenderBoard, contentType: "image/svg+xml");
}

IResult GetGame(SvgRenderer renderer, TriangleBoard board)
{
    async Task Action(Stream stream)
    {
        await renderer.RenderGame(stream, board);
    }

    return Results.Stream(Action, contentType: "image/svg+xml");
}
