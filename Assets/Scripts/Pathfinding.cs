using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Pathfinding : MonoBehaviour
{
    private NodeGrid nodeGrid;
    private List<Node> pathNodes;
    public int actualCharacterNode = 0;
    public Transform target;

    private void Awake()
    {
        nodeGrid = GetComponent<NodeGrid>();
    }

    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        actualCharacterNode = 0;
        Node startNode = nodeGrid.NodeFromWorldPoint(startPos);
        Node targetNode = nodeGrid.NodeFromWorldPoint(targetPos);

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

            foreach (Node neighbour in nodeGrid.GetNeighbours(currentNode))
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
        path.Add(startNode);
        path.Reverse();
        pathNodes = path;
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

    public Vector3 GetActualMoveDirection(Vector3 position)
    {
        if (actualCharacterNode != pathNodes.Count)
        {
            return GetMoveDirection(position, pathNodes[actualCharacterNode + 1]);
        }
        return Vector3.zero;
    }

    private Vector3 GetMoveDirection(Vector3 positon, Node node)
    {
        Vector3 direction = node.worldPosition - positon;
        direction.y = 0;

        return direction.normalized;
    }

    public bool IsNextNodeVisited(Vector3 position)
    {
        Node node = nodeGrid.NodeFromWorldPoint(position);

        if (node != pathNodes[actualCharacterNode] && node == pathNodes[actualCharacterNode + 1])
        {
            Vector3 pos = node.worldPosition;
            //float distance = (pos - position).magnitude;
            actualCharacterNode += 1;
            return true;
            
        }
        return false;
    }

    public bool IsTargetNode(Vector3 position)
    {
        Node node = nodeGrid.NodeFromWorldPoint(position);

        if (node == pathNodes[pathNodes.Count - 1])
        {
            return true;
        }
        return false;
    }

    public void ChangeTargetPosition(Vector3 pos)
    {
        Node node = nodeGrid.NodeFromWorldPoint(pos);
        if (node != nodeGrid.NodeFromWorldPoint(target.position) && node.isWalkable)
        {
            target.position = pos;
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(nodeGrid.gridWorldSize.x, 1, nodeGrid.gridWorldSize.y));

            if (nodeGrid.grid != null)
            {
                foreach (Node node in nodeGrid.grid)
                {
                    Gizmos.color = node.isWalkable ? Color.white : Color.red;
                    if (pathNodes != null)
                    {
                        if (pathNodes.Contains(node))
                        {
                            Gizmos.color = Color.green;
                        }
                    }
                    if (node == nodeGrid.NodeFromWorldPoint(target.position))
                    {
                        Gizmos.color = Color.yellow;
                    }
                    Vector3 gizmosSize = new Vector3(nodeGrid.nodeDiameter - 0.1f, 0.05f, nodeGrid.nodeDiameter - 0.1f);
                    Gizmos.DrawCube(node.worldPosition, gizmosSize);
                }
            }
        } 
    }

}
