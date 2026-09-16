using UnityEngine;

public class test_move_interakt_pressure_plate : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("yes");
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("no");
    }
}
