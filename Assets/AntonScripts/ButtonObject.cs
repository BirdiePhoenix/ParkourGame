using System;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class ButtonObject : MonoBehaviour
{
    public bool Open;
    [SerializeField]private Transform objectToMove;
    [SerializeField] private Vector3 moveDirection = Vector3.up;
    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private bool canReverse = true;
    [SerializeField] private float moveSpeed = 2f;
    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isOpen;
    private bool isMoving;
    private Vector3 destination;
    
    private void Start()
    {
        // sets start and target pos
        startPos = objectToMove.position;
        targetPos = startPos + moveDirection.normalized * moveDistance;
    }

    private void OnMouseDown()
    {
        //checks if not moving
        if (isMoving) return;
        Toggle();
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
}
