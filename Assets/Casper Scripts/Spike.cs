using System.Collections;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private Vector3 _startPosition;
    private Vector3 _topPosition;

    [SerializeField] private float _movementSpeed = 2f;
    [SerializeField] private float _amplitude = 3f;
    [SerializeField] private float _waitTime = 3f;

    private void Start()
    {
        _startPosition = transform.position;
        _topPosition = _startPosition + Vector3.up * _amplitude;

        StartCoroutine(MovePlatform());
    }

    private IEnumerator MovePlatform()
    {
        while (true)
        {
            // Move from the starting position to the top.
            yield return MoveToPosition(_topPosition);

            // Wait at the top.
            yield return new WaitForSeconds(_waitTime);

            // Move back down to the starting position.
            yield return MoveToPosition(_startPosition);

            // Wait at the bottom.
            yield return new WaitForSeconds(_waitTime);
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                _movementSpeed * Time.deltaTime
            );

            yield return null;
        }
    }
}