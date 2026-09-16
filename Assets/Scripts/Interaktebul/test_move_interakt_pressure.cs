using UnityEngine;

public class test_move_interakt_pressure : MonoBehaviour
{
    [SerializeField] GameObject sphere;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = sphere.transform.position;
    }
}
