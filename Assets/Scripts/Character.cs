using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Material stateMaterial;
    [SerializeField] public Material playerMaterial;

    // Movement
    private enum State
    {
        Idle,
        Move,
        Rest
    }

    private State state;
    private bool isRotated = true;

    public Vector3 velocity;

    public float speed;
    public float agility;
    public float endurance;
    public float actualSpeed;
    private float walkingTime = 0;
    private float restingTime = 0;
    public float timeToRest;

    public Vector3 position
    {
        get
        {
            return transform.position;
        }
    }

    private void Start()
    {
        speed = Random.Range(1, 6);
        agility = Random.Range(1, 6);
        endurance = Random.Range(1, 6);

        actualSpeed = speed;
        state = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Idle:
                stateMaterial.color = Color.white;
                break;

            case State.Move:
                stateMaterial.color = Color.green;
                walkingTime += Time.deltaTime;
                if (walkingTime >= endurance)
                {
                    walkingTime = 0;
                    state = State.Rest;
                }
                Move();

                break;

            case State.Rest:
                stateMaterial.color = Color.yellow;
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

        transform.position += velocity * Time.deltaTime;
    }

    private void RotateToDirection()
    {
        if (!isRotated)
        {
            Quaternion toRotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);
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

    public void SetVelocity(Vector3 velocity)
    {
        if (transform.eulerAngles != velocity.normalized)
        {
            this.velocity = velocity;
            isRotated = false;
        }
    }

    public void SetCharacterRotation(Vector3 rotation)
    {
        transform.rotation = Quaternion.LookRotation(rotation, Vector3.up);
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

    
}
