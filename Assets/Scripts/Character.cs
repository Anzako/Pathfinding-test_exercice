using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Transform target;
    bool findNewPath = true;

    public Vector2 movementDirection;
    [SerializeField] private float speed;
    [SerializeField] private float agility;
    [SerializeField] private float endurance;
    [SerializeField] private Pathfinding pathFinder;

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (findNewPath)
        {
            pathFinder.FindPath(transform.position, target.position);
            movementDirection = pathFinder.GetActualMoveDirection();

            findNewPath = false;
        }

        if (pathFinder.IsNextNodeVisited(transform.position))
        {
            if (pathFinder.IsTargetNode(transform.position))
            {
                movementDirection = Vector3.zero;
                return;
            }
            movementDirection = pathFinder.GetActualMoveDirection();
            
        }
        
        Move();
    }

    private void Move()
    {
        float velocity = speed * Time.deltaTime;

        transform.position += new Vector3(movementDirection.x * velocity, 0, movementDirection.y * velocity);
    }

}
