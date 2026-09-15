using UnityEngine;

public class PlatformMovementLeftRight : MonoBehaviour
{
    private Vector3 _startPosition;

    [SerializeField] private float _elapsedTime;
    [SerializeField] private float _frequency = 2f;
    [SerializeField] private float _amplitude = 3f;

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
        transform.position = _startPosition + Vector3.left * Mathf.Sin(_elapsedTime) * _amplitude;
    }
}