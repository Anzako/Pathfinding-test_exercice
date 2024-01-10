using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    //public Transform seeker, target;
    private NodeGrid grid;
    public int actualCharacterNode = 0;

    private void Awake()
    {
        grid = GetComponent<NodeGrid>();
    }

    private void Update()
    {
        //FindPath(seeker.position, target.position);
    }

    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || 
                    openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost) 
                { 
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

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
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
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode) 
        { 
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        grid.pathNodes = path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }
    
    public Vector2 GetActualMoveDirection()
    {
        return GetMoveDirection(grid.pathNodes[actualCharacterNode], grid.pathNodes[actualCharacterNode + 1]);
    }

    public Vector2 GetMoveDirection(Node nodeA, Node nodeB)
    {
        Vector2 direction = new Vector2(nodeB.worldPosition.x - nodeA.worldPosition.x, nodeB.worldPosition.z - nodeA.worldPosition.z);

        return direction.normalized;
    }

    public bool IsNextNodeVisited(Vector3 position)
    {
        Node node = grid.NodeFromWorldPoint(position);

        if (node != grid.pathNodes[actualCharacterNode] && node == grid.pathNodes[actualCharacterNode + 1])
        {
            actualCharacterNode += 1;
            return true;
        }
        return false;
    }

    public bool IsTargetNode(Vector3 position)
    {
        Node node = grid.NodeFromWorldPoint(position);

        if (node == grid.pathNodes[grid.pathNodes.Count - 1])
        {
            return true;
        }
        return false;
    }
}
