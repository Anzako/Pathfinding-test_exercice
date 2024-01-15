using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class CharactersController : MonoBehaviour
{
    // Characters
    [SerializeField] private List<Character> characters;
    public int guideID;

    // Pathfinding
    [SerializeField] private Pathfinding pathFinder;
    public Transform target;
    bool movingToTarget = false;
    public LayerMask groundLayer;

    // Update is called once per frame
    void Update()
    {
        if (movingToTarget)
        {
            MoveCharacters();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            ChangeTargetPosition();
        }
        
    }

    private void MoveCharacters()
    {
        bool allCharactersInTarget = true;
        CheckIsNextNodeVisited(characters[guideID]);
        for (int i = 0; i < characters.Count; i++) 
        { 
            if (!characters[i].isInTarget)
            {
                allCharactersInTarget = false;
                if (i != guideID)
                {
                    FollowGuideCharacter(characters[i]);
                }
            }
        }
        if (allCharactersInTarget)
        {
            movingToTarget = false;
        }
    }

    private void CheckIsNextNodeVisited(Character guideCharacter)
    {
        if (pathFinder.IsTargetNode(guideCharacter.position))
        {
            guideCharacter.isInTarget = true;
            guideCharacter.ChangeToIdleState();
            return;
        }
        else if (pathFinder.IsNextNodeVisited(guideCharacter.position))
        {
            guideCharacter.SetVelocity(pathFinder.GetActualMoveDirection() * guideCharacter.actualSpeed);
        }
    }

    private void FindNewPath()
    {
        if (transform.position != target.position)
        {
            pathFinder.FindPath(characters[guideID].transform.position, target.position);
            characters[guideID].SetVelocity(pathFinder.GetActualMoveDirection() * characters[guideID].actualSpeed);
            characters[guideID].SetCharacterRotation(pathFinder.GetActualMoveDirection());

            StartMovingCharacters();
        }
    }

    private void StartMovingCharacters()
    {
        for(int i = 0; i < characters.Count; i++)
        {
            characters[i].ChangeToMoveState();
            characters[i].isInTarget = false;
        }
    }

    private void FollowGuideCharacter(Character character)
    {
        float distanceBehind = 1.2f;
        Vector3 v = characters[guideID].velocity * -1;
        v = v.normalized * distanceBehind;
        Vector3 behind = characters[guideID].position + v;

        Vector3 distanceToGuide = characters[guideID].position - character.position;
        if (characters[guideID].isInTarget)
        {
            if (distanceToGuide.magnitude <= 1.5f)
            {
                character.ChangeToIdleState();
                character.isInTarget = true;
            }
        }

        Vector3 vel = computeArrive(character, behind);
        vel += computeSeparate(character);

        character.SetVelocity(vel);
    }

    private Vector3 computeSeparate(Character myCharacter)
    {
        Vector3 v = new Vector3();
        int neighbourCount = 0;
        float maxSeparation = 1.2f;

        foreach (Character character in characters)
        {
            if (character != myCharacter)
            {
                float distance = (myCharacter.position - character.position).magnitude;
                if (distance < maxSeparation)
                {
                    v.x += character.position.x - myCharacter.position.x;
                    v.z += character.position.z - myCharacter.position.z;
                    neighbourCount++;
                }
            }
        }

        if (neighbourCount == 0)
            return v;

        v.x /= neighbourCount;
        v.z /= neighbourCount;
        v = v * -1;
        v = v.normalized;
        v = v * maxSeparation;

        return v;
    }

    private Vector3 computeArrive(Character character, Vector3 behind)
    {
        Vector3 desiredVelocity = behind - character.position;
        float distance = desiredVelocity.magnitude;
        float slowingRadius = 2f;

        if (distance < slowingRadius)
        {
            desiredVelocity = desiredVelocity.normalized * character.actualSpeed * (distance / slowingRadius);
        }
        else
        {
            desiredVelocity = desiredVelocity.normalized * character.actualSpeed;
        }

        Vector3 steering = desiredVelocity - character.velocity;
        Vector3 vel = truncate(character.velocity + steering, character.actualSpeed);
        return vel;
    }

    private Vector3 truncate(Vector3 vector, float max) 
    {
        float i;
        i = max / vector.magnitude;
        i = i < 1.0f ? i : 1.0f;
        return vector * i;
    }

    public void MoveToTarget()
    {
        if (!movingToTarget)
        {
            FindNewPath();
            movingToTarget = true;
        }
    }

    public void ChangeTargetPosition()
    {
        // Rzutowanie promienia z kamery do punktu klikniêcia myszy
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            pathFinder.ChangeTargetPosition(hit.point);
        }
    }

    
}
