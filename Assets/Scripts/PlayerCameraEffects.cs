using UnityEngine;

public class PlayerCameraEffects : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Rigidbody playerRigidbody;

    [SerializeField] private float baseFOV = 70f;
    [SerializeField] private float maxFOV = 80f;
    [SerializeField] private float speedForMaxFOV = 15f;
    [SerializeField] private float fovChangeSpeed = 5f;

    // Update is called once per frame
    void Update()
    {
        Vector3 horizontalVelocity = new Vector3(
            playerRigidbody.linearVelocity.x,
            0f,
            playerRigidbody.linearVelocity.z);

        float speed = horizontalVelocity.magnitude;

        float speedRatio = Mathf.Clamp01(speed / speedForMaxFOV);

        float targetFOV = Mathf.Lerp(
            baseFOV,
            maxFOV,
            speedRatio);

        playerCamera.fieldOfView = Mathf.Lerp(
            playerCamera.fieldOfView,
            targetFOV,
            fovChangeSpeed * Time.deltaTime);
    }
}
