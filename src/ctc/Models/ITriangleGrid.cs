namespace Models;

/// <summary>
/// Interface for logical triangular grid operations.
/// Separates game logic from rendering concerns.
/// </summary>
public interface ITriangleGrid
{
    /// <summary>
    /// Size of the triangular grid.
    /// </summary>
    int Size { get; }

    /// <summary>
    /// All triangle positions in the grid.
    /// </summary>
    IEnumerable<Position> TrianglePositions { get; }

    /// <summary>
    /// Get the three vertices of a triangle.
    /// </summary>
    Vertex[] GetTriangleVertices(Position triangle);

    /// <summary>
    /// Check if a triangle has an edge between two vertices.
    /// </summary>
    bool HasEdge(Vertex[] vertices, Vertex v1, Vertex v2);

    /// <summary>
    /// Get all vertices that are adjacent (form an edge) with the given vertex.
    /// </summary>
    IEnumerable<Vertex> GetAdjacentVertices(Vertex v);

    /// <summary>
    /// Get all vertices reachable in exactly 3 hops from the given vertex.
    /// </summary>
    IEnumerable<Vertex> GetReachableVertices(Vertex from);

    /// <summary>
    /// Check if a vertex is valid (within bounds of the grid).
    /// </summary>
    bool IsValidVertex(Vertex v);
}
