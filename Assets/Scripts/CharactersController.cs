using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
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
        CheckIsNextNodeVisited();
        for (int i = 0; i < characters.Count; i++) 
        { 
            if (!characters[i].isInTarget)
            {
                allCharactersInTarget = false;
                if (i != guideID)
                {
                    FollowGuideCharacter(i);
                }
            }
        }
        if (allCharactersInTarget)
        {
            movingToTarget = false;
        }
    }

    private void CheckIsNextNodeVisited()
    {
        if (pathFinder.IsTargetNode(characters[guideID].transform.position))
        {
            characters[guideID].isInTarget = true;
            characters[guideID].ChangeToIdleState();
            return;
        }
        else if (pathFinder.IsNextNodeVisited(characters[guideID].transform.position))
        {
            characters[guideID].velocity = pathFinder.GetActualMoveDirection() * characters[guideID].speed;
            characters[guideID].SetMovementDirection(pathFinder.GetActualMoveDirection());
        }
    }

    private void FindNewPath()
    {
        if (transform.position != target.position)
        {
            pathFinder.FindPath(characters[guideID].transform.position, target.position);
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

    private void FollowGuideCharacter(int characterID)
    {
        Vector3 direction = characters[guideID].transform.position - characters[characterID].transform.position;

        if (direction.magnitude > 1.5f)
        {
            characters[characterID].SetMovementDirection(direction.normalized); 
            characters[characterID].ChangeToMoveState();
        } else
        {
            characters[characterID].ChangeToIdleState();
            if (characters[guideID].isInTarget)
            {
                characters[characterID].isInTarget = true;
            }
        }
       /* Vector3 desiredVelocity = characters[guideID].transform.position - characters[characterID].transform.position;
        float distance = desiredVelocity.magnitude;
        float slowingRadius = 1.5f;

        if (distance < slowingRadius)
        {
            desiredVelocity = desiredVelocity.normalized * characters[characterID].speed * (distance / slowingRadius);
        } else
        {
            desiredVelocity = desiredVelocity.normalized * characters[characterID].speed;
        }

        Vector3 steering = desiredVelocity - */

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
