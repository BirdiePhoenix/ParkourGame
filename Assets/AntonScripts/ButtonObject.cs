using System;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonObject : MonoBehaviour
{
    public bool Open;
    [SerializeField]private GameObject objectToMove;
    [SerializeField]private Move_Object_To function;
    private SphereCollider buttonCollider;
    [SerializeField]private float moveSpeed;
    [SerializeField]private int UnitsToMove;
    [SerializeField]private bool Reverses;
    private bool active = false;
    private Vector3 direction;
    private bool playTween = false;
    private float unitsMoved = 0;

    private void Start()
    {
        buttonCollider = objectToMove.GetComponent<SphereCollider>();
    }
    private void OnMouseDown()
    {
        if (active == true)
        {
            return;
        }
        active = true;
        Open = !Open;
        if (Open)
        {
            
            switch (function)
            {
                case Move_Object_To.move_Right:
                {
                    direction = Vector3.right;
                    break;
                }
                case Move_Object_To.move_Left:
                {
                    direction = Vector3.left;
                    break;
                }
                case Move_Object_To.move_Down:
                {
                    direction = Vector3.down;
                    break;
                }
                case Move_Object_To.move_Up:
                {
                    direction = Vector3.up;
                    break;
                }
                case Move_Object_To.move_Backward:
                    direction = Vector3.back;
                    break;
                default:
                {
                    direction = Vector3.forward;
                    break;
                }
                
            }
        }
        direction.Normalize();
        playTween = true;
        
    }

    
    void Update()
    {
        if (playTween == true && objectToMove && unitsMoved < UnitsToMove)
        {
            objectToMove.transform.position += direction * (moveSpeed * Time.deltaTime);
            unitsMoved += 0.01f;
            
        }

        if (unitsMoved >= UnitsToMove)
        {
            active = false;
            if (Reverses)
            {
                unitsMoved = 0;
                playTween = false;
                direction = direction * -1;
            }
        }

        
    }

    enum Move_Object_To
    {
        move_Right,
        move_Left,
        move_Up,
        move_Down,
        move_Forward,
        move_Backward,
    }
}
