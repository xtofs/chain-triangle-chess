using Models;
using Xunit;

namespace ctc.Tests;

public class BandValidationTests
{
    private readonly TriangleGeometry _geometry;
    private readonly TriangleBoard _board;

    public BandValidationTests()
    {
        _geometry = new TriangleGeometry(9, 50, 20, 20);
        _board = new TriangleBoard(_geometry);
    }

    [Fact]
    public void ReachableVertices_ShouldOnlyIncludeActuallyReachable()
    {
        // Starting from (2,1), what vertices are exactly 3 hops away?
        var from = new Vertex(2, 1);
        var reachable = _geometry.GetReachableVertices(from).ToList();

        // All reachable vertices must form a valid band (be adjacent to another vertex in the path)
        // For now, just verify the list is not empty
        Assert.NotEmpty(reachable);

        // Each reachable vertex should form an edge with at least one other vertex we can reach
        foreach (var target in reachable)
        {
            // Verify there's a valid 3-hop path from from to target
            var path = FindPath(from, target, 3);
            Assert.NotNull(path);
        }
    }

    [Fact]
    public void AddBand_OnlyBetweenAdjacentVertices()
    {
        // Bands should only be placeable between vertices that form a triangle edge
        var v1 = new Vertex(1, 0);
        var v2 = new Vertex(1, 1);

        // This should work (they form an edge)
        _board.AddBand(v1, v2);
        Assert.Single(_board.Bands);
    }

    // Helper: find a path of exactly length hops between two vertices
    private List<Vertex>? FindPath(Vertex start, Vertex end, int hopsRequired)
    {
        var queue = new Queue<(Vertex current, List<Vertex> path)>();
        queue.Enqueue((start, new List<Vertex> { start }));
        var visited = new HashSet<Vertex> { start };

        while (queue.Count > 0)
        {
            var (current, path) = queue.Dequeue();

            if (current == end && path.Count == hopsRequired + 1) // hops = vertices - 1
            {
                return path;
            }

            if (path.Count >= hopsRequired + 1)
                continue;

            // Get adjacent vertices
            var adjacent = _geometry.GetAdjacentVertices(current);
            foreach (var next in adjacent)
            {
                if (!visited.Contains(next))
                {
                    visited.Add(next);
                    var newPath = new List<Vertex>(path) { next };
                    queue.Enqueue((next, newPath));
                }
            }
        }

        return null;
    }
}
