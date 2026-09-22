using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDie : MonoBehaviour
{
    public List<GameObject> newMap = new List<GameObject>();
    public List<GameObject> newMap2 = new List<GameObject>();
    public List<GameObject> newMap3 = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        newMap.ForEach(x => x.SetActive(false));
        newMap2.ForEach(x => x.SetActive(false));
        newMap3.ForEach(x => x.SetActive(false));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Spike"))
        {
            Respawn();
        }

        if (other.gameObject.CompareTag("Deathbox"))
        {
            Respawn();
        }

        if (other.CompareTag("Crystal1"))
        {
           newMap.ForEach(x => x.SetActive(true));
           CheckpointManager.Instance.SetCheckpoint(1);
        }

        if (other.CompareTag("Crystal2"))
        {
            newMap2.ForEach(x => x.SetActive(true));
            CheckpointManager.Instance.SetCheckpoint(3);
        }

        if (other.CompareTag("Crystal3"))
        {
            newMap3.ForEach((x) => x.SetActive(true));
            CheckpointManager.Instance.SetCheckpoint(5);
        }
    }
    public void Respawn()
    {
        Transform spawnPoint = CheckpointManager.Instance.GetCheckpoint();

        if (spawnPoint == null)
        {
            Debug.LogError("No checkpoint spawn point found!");
            return;
        }

        // Stop the player's movement if you have a Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Move player
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        Debug.Log("Player respawned at checkpoint " +
                  CheckpointManager.Instance.currentCheckpoint);
    }
}

