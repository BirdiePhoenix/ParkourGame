using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDie : MonoBehaviour
{
    public List<GameObject> newMap = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        newMap.ForEach(x => x.SetActive(false));
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

        if (other.CompareTag("Crystal1"))
        {
           newMap.ForEach(x => x.SetActive(true));
        }
    }
}
