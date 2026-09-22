using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    [Header("Checkpoint spawn points")]
    public Transform startSpawn;
    public Transform crystal1PickupSpawn;
    public Transform crystal1PortalSpawn;
    public Transform crystal2PickupSpawn;
    public Transform crystal2PortalSpawn;
    public Transform crystal3PickupSpawn;
    public Transform crystal3PortalSpawn;
    public Transform crystal4PickupSpawn;
    public Transform crystal4PortalSpawn;

    public int currentCheckpoint = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Transform GetCheckpoint()
    {
        switch (currentCheckpoint)
        {
            case 0:
                return startSpawn;

            case 1:
                return crystal1PickupSpawn;

            case 2:
                return crystal1PortalSpawn;

            case 3:
                return crystal2PickupSpawn;

            case 4:
                return crystal2PortalSpawn;

            case 5:
                return crystal3PickupSpawn;

            case 6:
                return crystal3PortalSpawn;

            case 7:
                return crystal4PickupSpawn;

            case 8:
                return crystal4PortalSpawn;

            default:
                return startSpawn;
        }
    }

    public void SetCheckpoint(int checkpoint)
    {
        currentCheckpoint = checkpoint;

        Debug.Log("Checkpoint changed to: " + checkpoint);
    }
}