using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    // Pathfinding
    [SerializeField] private Pathfinding pathFinder;
    public Transform target;
    bool findNewPath = false;
    
    // Movement
    private enum State
    {
        Idle,
        Move,
        Rest
    }

    private State state;
    private bool isRotated = true;

    private Vector3 movementDirection;

    [SerializeField] private float speed;
    [SerializeField] private float agility;
    [SerializeField] private float endurance;
    private float actualSpeed;
    private float walkingTime = 0;
    private float restingTime = 0;
    public float timeToRest;

    private void Start()
    {
        actualSpeed = speed;
        state = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Idle:
                if (findNewPath)
                {
                    FindNewPath();
                }
                break;

            case State.Move:
                walkingTime += Time.deltaTime;
                if (walkingTime >= endurance)
                {
                    walkingTime = 0;
                    state = State.Rest;
                }
                Move();
                CheckIsNextNodeVisited();

                break;

            case State.Rest:
                restingTime += Time.deltaTime;
                if (restingTime >= timeToRest)
                {
                    restingTime = 0;
                    state = State.Move;
                }

                break;
        }
    }

    private void Move()
    {
        RotateToDirection();
        float velocity = actualSpeed * Time.deltaTime;

        transform.position += new Vector3(transform.forward.x * velocity, 0, transform.forward.z * velocity);
    }

    private void RotateToDirection()
    {
        if (!isRotated)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            float angle = AngleBetween(transform.rotation, toRotation);
            if (angle > 5)
            {
                actualSpeed = 1;
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, agility * Time.deltaTime);
            }
            else
            {
                transform.rotation = toRotation;
                actualSpeed = speed;
                isRotated = true;
            }
        }
        
    }

    private void FindNewPath()
    {
        Debug.Log("DUPA");
        if (transform.position != target.position)
        {
            pathFinder.FindPath(transform.position, target.position);
            movementDirection = pathFinder.GetActualMoveDirection();
            transform.rotation = Quaternion.LookRotation(movementDirection, Vector3.up);

            findNewPath = false;
            state = State.Move;
        }
    }

    private void CheckIsNextNodeVisited()
    {
        if (pathFinder.IsTargetNode(transform.position))
        {
            state = State.Idle;
            return;
        } else if (pathFinder.IsNextNodeVisited(transform.position))
        {
            SetMovementDirection(pathFinder.GetActualMoveDirection());
        }
    }

    private float AngleBetween(Quaternion from, Quaternion to)
    {
        float angle = Quaternion.Angle(from, to);
        return Mathf.Abs(angle);
    }

    public void SetMovementDirection(Vector3 direction)
    {
        if (movementDirection != direction)
        {
            movementDirection = direction;
            isRotated = false;
        }
    }

    public void MoveToNewTarget()
    {
        findNewPath = true;
    }

}
