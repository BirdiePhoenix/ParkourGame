using System.Collections;
using UnityEngine;

public class PlayerVault : MonoBehaviour
{
    

    private Rigidbody rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb =GetComponent<Rigidbody>();
    }

    
}
