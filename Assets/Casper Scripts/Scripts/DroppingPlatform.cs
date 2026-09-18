using System.Collections;
using UnityEngine;


public class DroppingPlatform : MonoBehaviour
{
    private Vector3 originPos;
    private Quaternion originRotate; // <-- Change type to Quaternion to match transform.rotation

    private Rigidbody rb;

    [SerializeField] private float timeBeforeDropp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Quaternion rotation = transform.rotation;
        originRotate = rotation; // <-- This assignment is now valid since both are Quaternion
        originPos = transform.position;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    private IEnumerator Falling()
    {
        yield return new WaitForSeconds(timeBeforeDropp);
        rb.constraints = RigidbodyConstraints.None;
        // Removed the incorrect assignment to rb.rotation
        rb.useGravity = true;

        yield return new WaitForSeconds(3);
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        transform.rotation = originRotate;
        transform.position = originPos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Falling());
        }
    }
}
