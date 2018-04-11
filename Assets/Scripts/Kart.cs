using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Kart : MonoBehaviour
{
    const float maxSpeed = 100f;
    Vector3 steeringForce = Vector3.zero;

    float acceleration = 1000f;
    float deacceleration = 10f;
    float fakeGravity = 5f;

    new Rigidbody rigidbody;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
	
	void Update()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");


        if (vertical > 0.5f)
        {
            Vector3 steeringDirection = Quaternion.Euler(0, 90f * horizontal, 0) * transform.forward;
            steeringForce += steeringDirection * acceleration * Time.deltaTime;
        }
        else
        {
            rigidbody.velocity *= deacceleration * Time.deltaTime;
        }

        rigidbody.AddForce(steeringForce * Time.deltaTime);
        rigidbody.useGravity = !IsGrounded();

        if (steeringForce.magnitude > 10f)
        {
            Vector3 newForward = rigidbody.velocity;
            newForward.y = 0;
            transform.forward = newForward.normalized;

        }
	}

    bool IsGrounded()
    {
        var hits = Physics.RaycastAll(transform.position, Vector3.down, 0.6f);
        Debug.DrawLine(transform.position, transform.position + Vector3.down * 0.6f);
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
