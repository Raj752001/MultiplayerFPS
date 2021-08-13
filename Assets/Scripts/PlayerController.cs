using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float lookSensitivity = 3f;
    public float thrusterForce = 1000f;
    public float thrusterFuelBurnSpeed = 1f;
    public float thrusterFuelRegainSpeed = 0.3f;
    private float thrusterFuelAmount = 1f;

    public float GetThrusterFuelAmount()
    {
        return thrusterFuelAmount;
    }

    public LayerMask environmentMask;

    [Header("Spring Setting")]
    public float jointSpring = 20f;
    public float jointMaxForce = 40f;


    // Component cashing
    PlayerMotor motor;
    ConfigurableJoint joint;
    Animator animator;

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();

        SetJointSetting(jointSpring);
    }

    private void Update()
    {
        if (PauseMenu.IsOn)
            return;

        // Setting target position for spring
        // This makes the physics act right when it comes to applying gravity
        // when flying over objects
        RaycastHit _hit;
        if (Physics.Raycast(transform.position, Vector3.down, out _hit, 100f, environmentMask))
        {
            joint.targetPosition = new Vector3(0f, -_hit.point.y, 0f);
        }
        else
        {
            joint.targetPosition = Vector3.zero;
        }

        // Calculate movenent velocity as a 3D vector
        float _xMov = Input.GetAxis("Horizontal");
        float _zMov = Input.GetAxis("Vertical");

        Vector3 _moveHorizontal = transform.right * _xMov;
        Vector3 _moveVertical = transform.forward * _zMov;

        // Final movement vector
        Vector3 _velocity = (_moveHorizontal + _moveVertical) * speed;

        // Animate movement
        animator.SetFloat("ForwardVelocity", _zMov);

        // Apply movement
        motor.Move(_velocity);

        // Calculate rotation w.r.t Y-Axis as a 3D Vector
        float _yRot = Input.GetAxisRaw("Mouse X");

        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * lookSensitivity;

        // Apply rotation
        motor.Rotate(_rotation);

        // Calculate camera rotation w.r.t X-Axis as a 3D Vector
        float _xRot = Input.GetAxisRaw("Mouse Y");

        float _cameraRotation = _xRot * lookSensitivity;

        // Apply camera rotation
        motor.RotateCamera(_cameraRotation);

        // Calculate the thrusterForce based on input
        Vector3 _thrusterForce = Vector3.zero;
        if (Input.GetButton("Jump") && thrusterFuelAmount > 0f)
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

            if(thrusterFuelAmount >= 0.01f)
            {
                _thrusterForce = Vector3.up * thrusterForce;
                SetJointSetting(0f);
            }
        }
        else
        {
            thrusterFuelAmount += thrusterFuelRegainSpeed * Time.deltaTime;
            SetJointSetting(jointSpring);
        }

        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);

        // Apply thruster force
        motor.ApplyThruster(_thrusterForce);

    }

    private void SetJointSetting(float _jointSpring)
    {
        joint.yDrive = new JointDrive { 
            positionSpring = _jointSpring,
            maximumForce = jointMaxForce
        };
    }

}
