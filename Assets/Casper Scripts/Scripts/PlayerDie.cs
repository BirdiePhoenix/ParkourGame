using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDie : MonoBehaviour
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
        if (other.gameObject.CompareTag("Spike"))
        {
           SceneManager.LoadScene(2);
        }

        if (other.gameObject.CompareTag("Deathbox"))
        {
            SceneManager.LoadScene(2);
        }
    }
}
