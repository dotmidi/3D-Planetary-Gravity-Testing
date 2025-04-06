using UnityEngine;

public class Attractable : MonoBehaviour
{
    [SerializeField] private bool rotateToCenter = true;
    [SerializeField] private float gravityStrength = 100;
    [SerializeField] private Collider attractableCollider;
    [SerializeField] private Rigidbody attractableRigidbody;
    
    private Attractor _currentAttractor;
    
    private void Update()
    {
        if (_currentAttractor)
        {
            if (!_currentAttractor.attractedObjects.Contains(this))
            {
                _currentAttractor = null;
                return;
            }
            attractableRigidbody.useGravity = false;
            if (rotateToCenter) RotateToCenter();
        }
        else
        {
            attractableRigidbody.useGravity = true;
            // transform.rotation = Quaternion.identity;
        }
    }

    public void Attract(Attractor attractorObj)
    {
        Vector3 attractionDir = (attractorObj.transform.position - attractableRigidbody.position).normalized;
        attractableRigidbody.AddForce(attractionDir * (attractorObj.gravity * gravityStrength * Time.fixedDeltaTime));
        
        float distance = Vector3.Distance(attractorObj.transform.position, attractableRigidbody.position);
        attractableRigidbody.linearDamping = distance < attractorObj.attractorCollider.radius ? Mathf.Lerp(attractableRigidbody.linearDamping, 5f, Time.fixedDeltaTime) : 0f;

        if (!_currentAttractor) 
        {
            _currentAttractor = attractorObj;
        }
    }

    private void RotateToCenter()
    {
        if (!_currentAttractor) return;
        
        Vector3 directionToTarget = _currentAttractor.transform.position - transform.position;
        
        Quaternion targetRotation = Quaternion.FromToRotation(-transform.up, directionToTarget) * transform.rotation;

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }
}