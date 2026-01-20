using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.FileProviders;
using Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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

app.MapGet("/api/board", GenerateStaticSvg);

app.MapGet("/api/game", GenerateDynamicSvg);

app.Run();

IResult GenerateStaticSvg()
{
    var geom = new TriangleGeometry(9, 50, 20, 20);
    var renderer = new SvgRenderer(geom);

    return Results.Stream(async (stream) =>
    {
        await renderer.RenderStatic(stream);
    }, contentType: "image/svg+xml");
}

IResult GenerateDynamicSvg()
{
    var geom = new TriangleGeometry(9, 50, 20, 20);
    var renderer = new SvgRenderer(geom);

    var bands = new (Vertex, Vertex)[]
    {
        ((1,1), (4,4)),
        ((4,2), (7,2)),
        ((6,1), (6,4)),
        ((5,0), (8,3)),
    };

    var pegs = new Piece[]
    {
        (6, 3, "red"),
    };
    var game = new TriangleBoard(bands, pegs);

    return Results.Stream(async (stream) =>
    {
        await renderer.RenderDynamic(stream, game);
    }, contentType: "image/svg+xml");
}
