using UnityEngine;

public class SpinningPlatform : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float force = 25;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate around the Y axis by 'force' degrees per frame
        transform.Rotate(force * Time.deltaTime, 0f, 0f);
    }
}
