using System;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonObject : MonoBehaviour
{
    public bool Open;
    [SerializeField]private GameObject objectToMove;
    [SerializeField]private Move_Object_To function;
    [SerializeField]private SphereCollider collider;
    [SerializeField]private float TimeToMove;
    [SerializeField]private int UnitsToMove;
    private Vector3 direction;
    
    private void OnMouseDown()
    {
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
        Debug.Log((direction * UnitsToMove));
        //objectToMove.transform.DOMove((direction * UnitsToMove), TimeToMove);
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
