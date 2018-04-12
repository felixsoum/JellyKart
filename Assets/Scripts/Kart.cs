using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Kart : MonoBehaviour
{
    public GameObject arrow;
    public GameObject mesh;

    const float maxVelocity = 200f;

    const float meshSteeringSpeed = 20f;

    float acceleration = 400f;
    float maxSteeringSpeed = 600f;
    float deacceleration = 400f;
    float friction = 200f;
    float steeringSpeed;
    const float cameraSpeed = 10f;
    new Rigidbody rigidbody;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
	
	void Update()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        bool isDrifting = Input.GetButton("Jump");
        Vector3 nextVelocity = rigidbody.velocity;

        if (nextVelocity.magnitude > 0)
        {
            float deltaFriction = friction * Time.deltaTime;
            if (nextVelocity.magnitude > deltaFriction)
            {
                nextVelocity = nextVelocity.normalized * (nextVelocity.magnitude - deltaFriction);
            }
            else
            {
                nextVelocity = Vector3.zero;
            }
        }

        Vector3 steeringDirection = transform.forward;
        float driftingFactor = isDrifting ? 0.75f : 1f;
        if (vertical > 0)
        {
            steeringSpeed = Mathf.Min(steeringSpeed + acceleration * driftingFactor * Time.deltaTime, maxSteeringSpeed * driftingFactor);
        }
        else if (vertical < 0)
        {
            steeringSpeed = Mathf.Max(steeringSpeed - acceleration * Time.deltaTime, -maxSteeringSpeed);
        }
        else
        {
            steeringSpeed = Mathf.Max(steeringSpeed - deacceleration * Time.deltaTime, 0);
        }


        float steeringAngle = 45f * horizontal;
        if (isDrifting)
        {
            steeringAngle *= 2;
        }
        steeringDirection = Quaternion.Euler(0, steeringAngle, 0) * transform.forward;
        Vector3 kartOrientation = Quaternion.Euler(0, steeringAngle / 2f, 0) * transform.forward;
        arrow.transform.forward = steeringDirection;
        nextVelocity += steeringDirection * steeringSpeed * Time.deltaTime;

        mesh.transform.forward = Vector3.Lerp(mesh.transform.forward, kartOrientation, meshSteeringSpeed * Time.deltaTime);

        rigidbody.useGravity = !IsGrounded();

        if (nextVelocity.magnitude > maxVelocity)
        {
            nextVelocity = nextVelocity.normalized * maxVelocity;
        }

        rigidbody.velocity = nextVelocity;


        if (nextVelocity.magnitude > 0)
        {
            Vector3 newForward = nextVelocity;
            newForward.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, newForward.normalized, cameraSpeed * Time.deltaTime);

        }
    }

    bool IsGrounded()
    {
        var hits = Physics.RaycastAll(transform.position, -transform.up, 0.6f);
        Debug.DrawLine(transform.position, transform.position + -transform.up * 0.6f);
        foreach (var hit in hits)
        {
            if (hit.collider.tag == "Player")
            {
                continue;
            }
            return true;
        }
        return false;
    }
}
