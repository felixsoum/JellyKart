using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Kart : MonoBehaviour
{
    public GameObject mesh;
    public GameObject cameraOffset;

    float cameraOffsetAngle;
    const float maxVelocity = 200f;
    const float meshSteeringSpeed = 20f;
    float maxSteeringSpeed = 100f;
    float acceleration = 20f;
    float deacceleration = 20f;
    float friction = 3f;
    float steeringSpeed;
    float extraGravity = 300f;
    float groundAirThreshold = 3f;
    new Rigidbody rigidbody;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
	
	void Update()
    {
        TrackCamera();

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        bool isDrifting = Input.GetButton("Jump");
        bool isGrounded = IsGrounded();

        Vector3 nextVelocity = rigidbody.velocity;
        if (nextVelocity.magnitude > 0)
        {
            nextVelocity = nextVelocity.normalized * nextVelocity.magnitude * (1 - friction * Time.deltaTime);
        }

        Vector3 steeringDirection = transform.forward;
        float driftingFactor = isDrifting ? 1 : 1f;

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

        if (!isGrounded)
        {
            //Debug.LogError("AIR");
            //steeringSpeed *= 1 - friction * Time.deltaTime;
            nextVelocity += Vector3.down * extraGravity * Time.deltaTime;
            steeringAngle /= 8;
        }

        steeringDirection = Quaternion.Euler(0, steeringAngle, 0) * transform.forward;
        nextVelocity += steeringDirection * Mathf.Sqrt(Mathf.Abs(steeringSpeed)) * Mathf.Sign(steeringSpeed);

        rigidbody.AddTorque(transform.up * steeringAngle * Time.deltaTime * 0.5f, ForceMode.VelocityChange);

        Vector3 nextMeshAngles = mesh.transform.localEulerAngles;
        if (nextMeshAngles.y > 180)
        {
            nextMeshAngles.y -= 360;
        }

        nextMeshAngles.y += (steeringAngle/2f - nextMeshAngles.y) * (meshSteeringSpeed * Time.deltaTime);
        mesh.transform.localEulerAngles = nextMeshAngles;

        if (nextVelocity.magnitude > maxVelocity)
        {
            nextVelocity = nextVelocity.normalized * maxVelocity;
        }       
        rigidbody.velocity = nextVelocity;
    }

    bool IsGrounded()
    {
        var hits = Physics.RaycastAll(transform.position + Vector3.up, -transform.up, groundAirThreshold);
        Debug.DrawLine(transform.position + Vector3.up, transform.position + -transform.up * groundAirThreshold);
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

    void TrackCamera()
    {
        Camera.main.transform.position = cameraOffset.transform.position;
        Camera.main.transform.rotation = cameraOffset.transform.rotation;
    }
}
