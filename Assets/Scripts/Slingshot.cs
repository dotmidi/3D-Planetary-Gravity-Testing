using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Planet
{
    public Transform planetTransform;
    public float mass = 1000f;
}

[RequireComponent(typeof(LineRenderer))]
public class Slingshot : MonoBehaviour
{
    [Header("Projectile Settings")]
    public Rigidbody rb;
    public float forceMultiplier = 5f;
    public float maximumForce = 15f;
    
    [Header("Trajectory Settings")]
    public int trajectoryPoints = 30;
    public float timeStep = 0.1f;
    
    [Header("Gravity Settings")]
    public List<Planet> planets;
    public float gravitationalConstant = 0.1f;

    private bool isAiming = false;
    private Vector3 startMousePos;
    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = trajectoryPoints;

        if (rb == null)
            Debug.LogWarning("Rigidbody not assigned on Slingshot.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            isAiming = true;

        if (!isAiming)
            return;

        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startMousePos = GetMouseWorldPosition();
        }
        else if (Input.GetMouseButton(0))
        {
            ShowTrajectory(startMousePos, GetMouseWorldPosition());
        }
        else if (Input.GetMouseButtonUp(0))
        {
            LaunchProjectile();
        }
    }

    private void LaunchProjectile()
    {
        Vector3 endMousePos = GetMouseWorldPosition();
        Vector3 direction = (startMousePos - endMousePos).normalized;
        float distance = Vector3.Distance(startMousePos, endMousePos);
        Vector3 force = direction * Mathf.Clamp(distance * forceMultiplier, 0, maximumForce);
        force.z = 0;

        if (rb != null)
            rb.linearVelocity = force;

        lr.positionCount = 0;
        isAiming = false;
    }

    private Vector3 GetMouseWorldPosition()
    {
        if (Camera.main == null) return Vector3.zero;
        
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(Camera.main.transform.position.y);
        return Camera.main.ScreenToWorldPoint(mouseScreenPos);
    }

    private void ShowTrajectory(Vector3 startPos, Vector3 currentPos)
    {
        Vector3 direction = (startPos - currentPos).normalized;
        float distance = Vector3.Distance(startPos, currentPos);
        Vector3 initialForce = direction * Mathf.Clamp(distance * forceMultiplier, 0, maximumForce);
        
        Vector3 simulatedPosition = rb.position;
        Vector3 simulatedVelocity = initialForce;

        lr.positionCount = trajectoryPoints;

        for (int i = 0; i < trajectoryPoints; i++)
        {
            Vector3 gravityAccel = CalculateGravityAcceleration(simulatedPosition);
            
            simulatedVelocity += gravityAccel * timeStep;
            simulatedPosition += simulatedVelocity * timeStep;
            simulatedPosition.z = 0;
            
            lr.SetPosition(i, simulatedPosition);
        }
    }

    private Vector3 CalculateGravityAcceleration(Vector3 position)
    {
        Vector3 acceleration = Vector3.zero;
        foreach (Planet planet in planets)
        {
            if (planet.planetTransform == null)
                continue;

            Vector3 dir = planet.planetTransform.position - position;
            float distanceSqr = dir.sqrMagnitude;
            if (distanceSqr < 0.01f) continue;
            
            acceleration += gravitationalConstant * planet.mass * dir.normalized / distanceSqr;
        }
        return acceleration;
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        Vector3 totalGravityForce = Vector3.zero;
        foreach (Planet planet in planets)
        {
            if (planet.planetTransform == null)
                continue;
            
            Vector3 direction = planet.planetTransform.position - rb.position;
            float distanceSqr = direction.sqrMagnitude;
            if (distanceSqr < 0.01f) continue;
            
            totalGravityForce += gravitationalConstant * planet.mass * direction.normalized / distanceSqr;
        }
        rb.AddForce(totalGravityForce, ForceMode.Acceleration);
    }
}
