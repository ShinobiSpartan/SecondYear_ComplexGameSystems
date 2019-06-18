using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Transform seeker, target;
    NodeGrid nGrid;

    private void Awake()
    {
        // Gets the NodeGrid component from current gameobject
        nGrid = GetComponent<NodeGrid>();
    }

    private void Update()
    {
        // If no target is set..
        if (!target)
            // Find the gameobject with tag of Food
            target = GameObject.FindGameObjectWithTag("Food").transform;

        FindPath(seeker.position, target.position);
    }

    // Finds path between two points
    void FindPath(Vector2 startPos, Vector2 targetPos)
    {
        // Gets the position of the start and end nodes from World Coordinates
        Node startNode = nGrid.NodeFromWorldPoint(startPos);
        Node targetNode = nGrid.NodeFromWorldPoint(targetPos);

        // Create a list which will contain all available nodes
        List<Node> openSet = new List<Node>();
        // Creates a hashset which will contain all of the path nodes
        HashSet<Node> closedSet = new HashSet<Node>();

        // Adds the start node to the Open set
        openSet.Add(startNode);

        // While the number of items in the open set is greater than 0..
        while(openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost ||  openSet[i].FCost == currentNode.FCost)
                {
                    if(openSet[i].hCost < currentNode.hCost)
                        currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbour in nGrid.GetNeighbours(currentNode))
            {
                if (!neighbour.isWalkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    // Retraces path to back to the start so the path can be displayed
    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        // Iterates through each node whilst not at the start node
        while (currentNode != startNode)
        {
            // Adds the current node to the path
            path.Add(currentNode);
            // Makes the current node the parent of the previous 'current node'
            currentNode = currentNode.parent;
        }

        // Flips the path so it goes the right way
        path.Reverse();

        // Sets the path variable in NodeGrid to the path
        nGrid.path = path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
