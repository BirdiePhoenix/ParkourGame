using System.Collections;
using UnityEngine;


public class DroppingPlatform : MonoBehaviour
{
    private Vector3 originPos;
    private Vector3 targetPos;

    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }


    private IEnumerator Falling()
    {
        yield return new WaitForSeconds(2);
        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = true; 
        

        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Falling());
        }
    }

}
