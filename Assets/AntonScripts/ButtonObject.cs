using System;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class ButtonObject : MonoBehaviour
{
    private bool Open;
    [SerializeField]private Transform objectToMove;
    private Vector3 moveDirection = Vector3.up;
    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private float moveSpeed = 2f;
    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isOpen;
    private bool isMoving;
    private Vector3 destination;
    [SerializeField] private bool ReverseAble = false;

    [SerializeField] move_direction  Direction;

    
    private void Start()
    {
        // sets start and target pos
        startPos = objectToMove.position;
        switch (Direction)
        {
            case move_direction.up:
            {
                moveDirection = Vector3.up;
                break;
            }
            case move_direction.down:
            {
                moveDirection = Vector3.down;
                break;
            }
            case move_direction.left:
            {
                moveDirection = Vector3.left;
                break;
            }
            case move_direction.right:
            {
                moveDirection = Vector3.right;
                break;
            }
            case move_direction.forward:
            {
                moveDirection = Vector3.forward;
                break;
            }
            case move_direction.backward:
            {
                moveDirection = Vector3.back;
                break;
            }
        }
        
        targetPos = startPos + moveDirection.normalized * moveDistance;
        
    }

    private void OnMouseDown()
    {
        //checks if not moving
        if (isMoving) return;
        
        Toggle();
        if (!ReverseAble)
        {
            isOpen = !isOpen;
        }
    }

    public void Toggle()
    {
        //toggles 
        isOpen = !isOpen;
        if (isOpen)
        {
            destination = targetPos;
        }
        else
        {
            destination = startPos;
        }
        //starts moving the object
        StartCoroutine(MoveObject(destination));
    }

    private IEnumerator MoveObject(Vector3 newDestination)
    {
        isMoving = true;
        // while it has not reached its final destination
        while (Vector3.Distance(objectToMove.position, newDestination) > 0.01f)
        {
            //move it to towards the final destination
            objectToMove.position = Vector3.MoveTowards(
                objectToMove.position,
                destination,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
        //sets it transform to the destination so it cant overshoot
        objectToMove.position = destination;
        isMoving = false;
    }

    private enum move_direction
    {
        up,
        down,
        left,
        right,
        forward,
        backward
    }
    
}
