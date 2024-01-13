using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersController : MonoBehaviour
{
    // Characters
    [SerializeField] private List<Character> characters;
    public int guideCharacterID;

    // Pathfinding
    [SerializeField] private Pathfinding pathFinder;
    public Transform target;
    bool movingToTarget = false;

    // Update is called once per frame
    void Update()
    {
        if (movingToTarget)
        {
            CheckIsNextNodeVisited();
        } 
    }

    private void CheckIsNextNodeVisited()
    {
        if (pathFinder.IsTargetNode(characters[guideCharacterID].transform.position))
        {
            movingToTarget = false;
            characters[guideCharacterID].ChangeToIdleState();
            return;
        }
        else if (pathFinder.IsNextNodeVisited(characters[guideCharacterID].transform.position))
        {
            characters[guideCharacterID].SetMovementDirection(pathFinder.GetActualMoveDirection());
        }
    }

    private void FindNewPath()
    {
        if (transform.position != target.position)
        {
            pathFinder.FindPath(characters[guideCharacterID].transform.position, target.position);
            characters[guideCharacterID].SetCharacterRotation(pathFinder.GetActualMoveDirection());
            characters[guideCharacterID].ChangeToMoveState();
        }
    }

    public void MoveToTarget()
    {
        if (!movingToTarget)
        {
            FindNewPath();
            movingToTarget = true;
        }
    }

    
}
