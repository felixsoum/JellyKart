using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Kart : MonoBehaviour
{
    public GameObject mesh;
    public Vector3 cameraOffset = new Vector3(0, 7, -10);
    public float cameraPitchAngle = 15;

    //Speed variables
    public float maxVelocity = 200;
    public const float meshSteeringSpeed = 20;
    public float maxSteeringSpeed = 100;
    public float acceleration = 20;
    public float deacceleration = 20;
    public float friction = 3;
    float steeringSpeed;

    //Air variables
    public float extraGravity = 350;
    public float groundAirThreshold = 1;

    //Flip variables
    public float flipAngle = 45;
    public float timeFlippedBeforeReset = 3;
    float currentTimeFlipped = 0;
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
        bool isFlipped = IsFlipped();

        if (isFlipped)
        {
            currentTimeFlipped += Time.deltaTime;
            if (currentTimeFlipped > timeFlippedBeforeReset)
            {
                transform.position += Vector3.up;
                transform.up = Vector3.up;
            }
        }
        else
        {
            currentTimeFlipped = 0;
        }

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
            nextVelocity += Vector3.down * extraGravity * Time.deltaTime;
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
        var hits = Physics.RaycastAll(transform.position + Vector3.up, Vector3.down, groundAirThreshold);
        //Debug.DrawLine(transform.position + Vector3.up, transform.position + -transform.up * groundAirThreshold);
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

    bool IsFlipped()
    {
        return Vector3.Angle(transform.up, Vector3.up) > 45f;
    }

    void TrackCamera()
    {
        Camera camera = Camera.main;
        Vector3 direction = transform.forward;
        direction.y = 0;
        direction.Normalize();
        camera.transform.position = transform.position + direction * cameraOffset.z + Vector3.up * cameraOffset.y;

        float cameraAngle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);

        camera.transform.eulerAngles = new Vector3(cameraPitchAngle, cameraAngle, 0);
    }
}
