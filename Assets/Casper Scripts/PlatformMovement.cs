using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    private Vector3 _startPosition;

    private float _elapsedTime;
    private float _frequency = 2f;
    private float _amplitude = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _startPosition = transform.position;

        _elapsedTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        _elapsedTime += Time.deltaTime * _frequency;
        transform.position = _startPosition + Vector3.up * Mathf.Sin(_elapsedTime) * _amplitude;
    }
}