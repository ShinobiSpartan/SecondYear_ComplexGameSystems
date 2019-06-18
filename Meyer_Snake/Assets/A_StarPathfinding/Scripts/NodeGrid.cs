using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid : MonoBehaviour
{
    // How big the grid will be in Unity units
    public Vector2 gridWorldSize;
    // The radius of each node
    public float nodeRadius;
    // The array that will make up the grid
    Node[,] grid;
    // Determines if a node will be walkable or not
    public LayerMask unwalkableMask;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    // List of nodes which make up the path
    public List<Node> path;

    private void Start()
    {
        nodeDiameter = nodeRadius * 2;

        // Calculates the size of the actual grid
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        // Create the initial grid
        CreateGrid();
    }

    private void Update()
    {
        // Recreates the grid every frame to accomodate for changes in the unwalkable nodes
        CreateGrid();
    }

    // Generates grid for Pathfinding to work off of
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        // Find the bottom left of the grid to base points off of
        Vector2 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

        // For loops to check and create each node in the grid
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // Calculates the current world point in which to place a node
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);

                // Checks if the node is to be walkable or not
                bool walkable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask));

                // Creates new node at point
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    // Find the 4 nodes surrounding the selected node
    public List<Node> GetNeighbours(Node node)
    {
        // Create new list for the neighbours
        List<Node> neighbours = new List<Node>();

         for (int x = -1; x <= 1; x++)
         {
            for (int y = -1; y <= 1; y++)
                // Skip the check if it is any of the folllowing squares in a 3x3 square arround the selected node
            {     // Centre                Bottom Left             Top Left               Top Right             Bottom Right
                if ((x == 0 && y == 0) || (x == -1 && y == -1) || (x == -1 && y == 1) || (x == 1 && y == 1) || (x == 1 && y == -1))
                    continue;

                // Check if the axis is near the edge of the grid
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                // If the neighbours check out, add them to the list
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    neighbours.Add(grid[checkX, checkY]);
            }
         }

         // Returns the list of current neighbours
        return neighbours;
    }

    // Allows you to get a specific node from the world coordinates
    public Node NodeFromWorldPoint(Vector2 worldPosition)
    {
        // Generates a percentage of where the postion is on the grid
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;

        // Clamps the percent so it can only be between 0 and 1
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        // Calculates X and Y from percentages
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        // Returns the node at the position
        return grid[x, y];

    }

    // *ONLY VISIBLE IN SCENE VIEW* Draws the grid along with the pathfinding
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector2(gridWorldSize.x, gridWorldSize.y));

        if (grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.isWalkable) ? Color.white : Color.red;
                if (path != null)
                    if (path.Contains(n))
                        Gizmos.color = Color.black;
                Gizmos.DrawCube(n.worldPosition, Vector2.one * (nodeDiameter - .1f));
            }
        }
    }
}
