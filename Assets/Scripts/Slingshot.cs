using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Slingshot : MonoBehaviour
{
    public Rigidbody rb;
    public float forceMultiplier = 5f;
    public float maximumForce = 15f;
    public int trajectoryPoints = 30;
    public float timeStep = 0.1f;

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

        if (!isAiming) return;

        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
            startMousePos = GetMouseWorldPosition();

        else if (Input.GetMouseButton(0))
            ShowTrajectory(startMousePos, GetMouseWorldPosition());

        else if (Input.GetMouseButtonUp(0))
            LaunchProjectile();
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

        for (int i = 0; i < trajectoryPoints; i++)
        {
            float t = i * timeStep;
            Vector3 point = rb.position + (initialForce * t) + 0.5f * Physics.gravity * t * t;
            point.z = 0;
            lr.SetPosition(i, point);
        }
    }
}
