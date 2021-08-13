using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    public Camera cam;

    Vector3 velocity = Vector3.zero;
    Vector3 rotation = Vector3.zero;
    float cameraRotationX = 0f;
    float currentCameraRotationX = 0f;
    Vector3 thrusterForce = Vector3.zero;

    public float cameraRotationLimit = 85f;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Setters
    public void Move (Vector3 _veloctiy) 
    {
        velocity = _veloctiy;
    }
    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }
    public void RotateCamera(float _cameraRotationX)
    {
        cameraRotationX = _cameraRotationX;
    }
    public void ApplyThruster(Vector3 _thrusterForce)
    {
        thrusterForce = _thrusterForce;
    }


    private void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    // Perform movement based on velocity variable
    void PerformMovement() 
    { 
        if( velocity != Vector3.zero )
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

        if( thrusterForce != Vector3.zero )
        {
            rb.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }

    }

    // Perform rotation 
    void PerformRotation()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if(cam!=null)
        {
            // New Rotational computational
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            // Apply rotation to camera
            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f);
        }
    }
}
