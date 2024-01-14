using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Material material;

    // Movement
    private enum State
    {
        Idle,
        Move,
        Rest
    }

    private State state;
    private bool isRotated = true;
    public bool isInTarget = false;

    private Vector3 movementDirection;
    public Vector3 velocity;

    [SerializeField] public float speed;
    [SerializeField] public float agility;
    [SerializeField] public float endurance;
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
                material.color = Color.white;
                break;

            case State.Move:
                material.color = Color.green;
                walkingTime += Time.deltaTime;
                if (walkingTime >= endurance)
                {
                    walkingTime = 0;
                    state = State.Rest;
                }
                Move();

                break;

            case State.Rest:
                material.color = Color.yellow;
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
        //transform.position += new Vector3(movementDirection.x * velocity, 0, movementDirection.z * velocity);
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

    public void ChangeToIdleState()
    {
        state = State.Idle;
    }

    public void ChangeToMoveState()
    {
        if (state != State.Rest)
        {
            state = State.Move;
        }
    }

    public void ChangeToRestState()
    {
        state = State.Rest;
    }

    public void SetCharacterRotation(Vector3 rotation)
    {
        transform.rotation = Quaternion.LookRotation(rotation, Vector3.up);
    }
}
