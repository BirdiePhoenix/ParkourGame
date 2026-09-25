using System;
using UnityEngine;

public class PlayerCameraEffects : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Rigidbody playerRigidbody;

    [SerializeField] private float baseFOV = 70f;
    [SerializeField] private float maxFOV = 80f;
    [SerializeField] private float speedForMaxFOV = 15f;
    [SerializeField] private float fovChangeSpeed = 5f;
    
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float wallRunTilt = 12f;
    [SerializeField] private float wallRunTiltSpeed = 6f;
    
    [SerializeField] private float minLandingSpeed = 2f;
    [SerializeField] private float maxLandingSpeed = 8f;
    [SerializeField] private float landingKickStrength = 0.12f;
    [SerializeField] private float landingSpringStrength = 100f;
    [SerializeField] private float landingSpringDamping = 12f;

    private Vector3 originalLocalPosition;
    private float landingOffset;
    private float landingVelocity;

    private void Awake()
    {
        originalLocalPosition = transform.localPosition;
    }

    private void OnEnable()
    {
        playerMovement.Landed += OnLanded;
    }

    private void OnDisable()
    {
        playerMovement.Landed -= OnLanded;
    }

    // Update is called once per frame
    void Update()
    {
        // FOV
        Vector3 horizontalVelocity = new Vector3(
            playerRigidbody.linearVelocity.x,
            0f,
            playerRigidbody.linearVelocity.z
        );

        float speed = horizontalVelocity.magnitude;

        float speedRatio = Mathf.Clamp01(speed / speedForMaxFOV);

        float targetFOV = Mathf.Lerp(
            baseFOV,
            maxFOV,
            speedRatio
        );

        playerCamera.fieldOfView = Mathf.Lerp(
            playerCamera.fieldOfView,
            targetFOV,
            fovChangeSpeed * Time.deltaTime
        );
        
        // Tilt
        float targetTilt = 0f;

        if (playerMovement.IsWallRunning)
        {
            if (playerMovement.WallLeft)
            {
                targetTilt = -wallRunTilt;
            }
            else if (playerMovement.WallRight)
            {
                targetTilt = wallRunTilt;
            }
        }

        Vector3 currentRotation = transform.localEulerAngles;

        float currentTilt = currentRotation.z;

        if (currentTilt > 180f)
        {
            currentTilt -= 360f;
        }

        float newTilt = Mathf.Lerp(
            currentTilt,
            targetTilt,
            wallRunTiltSpeed * Time.deltaTime
        );
        
        transform.localRotation = Quaternion.Euler(
            currentRotation.x,
            currentRotation.y,
            newTilt
        );
        
        // Landing
        float springForce = -landingOffset * landingSpringStrength;
        float dampingForce = -landingVelocity * landingSpringDamping;

        landingVelocity +=
            (springForce + dampingForce) * Time.deltaTime;

        landingOffset +=
            landingVelocity * Time.deltaTime;

        transform.localPosition =
            originalLocalPosition +
            Vector3.up * landingOffset;
    }

    private void OnLanded(float impactSpeed)
    {
        float impactRatio = Mathf.InverseLerp(
            minLandingSpeed,
            maxLandingSpeed,
            impactSpeed
        );

        landingVelocity -= landingKickStrength * impactRatio;
    }
}
