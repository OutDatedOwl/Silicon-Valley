using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float maximumSpeed;

    [SerializeField]
    private float rotationSpeed;

    AudioSource audioSource;

    [SerializeField]
    private float jumpSpeed;

    private float jumpButtonGracePeriod;
    public bool boostedSpeed;

    public float speed;

    [SerializeField]
    private Transform cameraTransform;
    

    [SerializeField]
    private Animator animator;
    private CharacterController characterController;
    private float ySpeed;
    private float originalStepOffset;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;
    private bool isJumping;
    private bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);

        //animator.SetFloat("Input Magnitude", inputMagnitude, 0.05f, Time.deltaTime);
        Action_Inputs();

        movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        movementDirection.Normalize();

        speed = inputMagnitude * maximumSpeed;
        float normalSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            speed = speed + 5.0f;
            boostedSpeed = true;
        }
        else
            {
                speed  = normalSpeed;
                boostedSpeed = false;
            }
        ySpeed += Physics.gravity.y * Time.deltaTime;
        //print(characterController.isGrounded);

        if (Time.time - lastGroundedTime <= jumpButtonGracePeriod)
        {
            characterController.stepOffset = originalStepOffset;
            ySpeed = -0.5f;
            animator.SetBool("isGrounded", true);
            isGrounded = true;
            animator.SetBool("isJumping", false);
            isJumping = false;

            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
            {
                ySpeed = jumpSpeed;
                animator.SetBool("isJumping", true);
                isJumping = true;
                jumpButtonPressedTime = null;
                lastGroundedTime = null;
            }
        }
        else
        {
            characterController.stepOffset = 0;
        }

        Vector3 velocity = movementDirection * speed;
        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);

        if (movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }        
    }

    private void Action_Inputs()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jumpButtonPressedTime = Time.time;      
        }

        if (characterController.isGrounded)
        {
            lastGroundedTime = Time.time;
        }



        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) )
        {
            animator.SetBool("isRunning", true);
        }
        else
        animator.SetBool("isRunning", false);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void Boing_Effect(){
        //audioSource.PlayOneShot(audioClip);
    }
}

/*public class Player : MonoBehaviour
{
    public CharacterController controller; // Player controller

    //public GameObject target; // Target where Player throws bomb

    public float Maximumspeed, jump_Speed, vert_velocity = 0; // Speed of Player
    public Transform cam;

    public float accel;
    float single_Step;

    public Vector3 offSet;

    //Vector3 camera_Vector;

    Vector3 directionMove, camF, camR, new_Direction; // Input of X and Y axis
    Vector3 velocity, velocityXZ, new_Vel;

    public Animator anim; // Animate Player

    public bool move_Camera = false;
    private float inputV;
    private float inputH;

    private void Start()
    {
        //controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        AnimatePlayer();
        DoMove();
        Calculate_Camera();
        Jump();
        //camera_Vector = -transform.forward * 4 + Vector3.up;
        //velocity.y = vert_velocity;
        //if(Input.GetKeyDown(KeyCode.E)){
        //    move_Camera = true;             
            //camera_Vector = -transform.forward * 2 + Vector3.up;
        //}
        //print(cam.position);
        //transform.rotation = Quaternion.LookRotation(new_Direction);
        controller.Move(velocity * Time.deltaTime);
    }

    void FixedUpdate()
    {
        Gravity();
        //DoMove();
        PlayerRotation();
        //transform.rotation = Quaternion.LookRotation(new_Direction);
        
    }

        private void LateUpdate()
    {
        if(!move_Camera){
            //cam.position = transform.position + offSet;
        }
        else
            {
                cam.position = Vector3.Slerp(cam.position, transform.position + camera_Vector * 2, 900f);
                cam.rotation = Quaternion.LookRotation(-camera_Vector);
                move_Camera = false;
            }
        
        
        //cam.LookAt(transform.position);
        //cam.rotation = Quaternion.Euler(25, cam.rotation.y, cam.rotation.z);
    }

    void Calculate_Camera() {
        camF = cam.forward;
        camR = cam.right;

        camF.y = 0;
        camR.y = 0;
        camF = camF.normalized;
        camR = camR.normalized;
    }

    void PlayerRotation() 
    {
        //single_Step = speed * Time.deltaTime;
        new_Vel = new Vector3(velocity.x, velocityXZ.y, velocity.z);
        new_Direction = Vector3.RotateTowards(transform.forward, new_Vel, single_Step, 0.0f);
    }

    // Allow Player to move
    public void DoMove()
    {   
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 directionMove = new Vector3(horizontalInput, 0, verticalInput);
        float inputMagnitude = Mathf.Clamp01(directionMove.magnitude);
        
        float speed = inputMagnitude * Maximumspeed;
        directionMove.Normalize();

        Vector3 velocity = directionMove * speed;
        velocity.y = vert_velocity;
        //directionMove = new Vector3(Input.GetAxisRaw("Horizontal") * speed, 0, Input.GetAxisRaw("Vertical") * speed);
        //directionMove = Vector3.ClampMagnitude(directionMove, 1);
        //directionMove = Quaternion.AngleAxis(cam.rotation.eulerAngles.y, Vector3.up) * directionMove;


        //float yStore = directionMove.y;
        //velocityXZ = new Vector3(velocity.x, velocityXZ.y, velocity.z);
        //print(controller.isGrounded);
        //print("Velocity:" + velocity);
        //velocityXZ.y = 0;
        //camF = cam.forward;
        //camR = cam.right;
        //camF.y = 0;
        //camR.y = 0;
        //camF = camF.normalized;
        //camR = camR.normalized;
        //velocityXZ = Vector3.Lerp(velocityXZ, directionMove * speed, accel * Time.deltaTime);
        //velocity = new Vector3(velocityXZ.x, velocity.y, velocityXZ.z);
        //velocity = camF * velocity.z + camR * velocity.x;
        //velocity = Vector3.ClampMagnitude(velocity, 1);
        //print("CAMF " + camF);
        //print("CAMR " + camR);
        //velocity.y = velocityXZ.y;
        //directionMove.y = yStore;

    }

    void AnimatePlayer()
    {
        inputH = Input.GetAxis("Horizontal");
        inputV = Input.GetAxis("Vertical");

        //anim.SetFloat("InputV", inputV);
        //anim.SetFloat("InputH", inputH);
    }

    // Apply gravity because we used character controller
    public void Gravity()
    {
        if (controller.isGrounded)
        {
            vert_velocity = -0.5f; // To keep player grounded at all times by minor gravity
        }
        else
        {
            vert_velocity += Physics.gravity.y * Time.deltaTime; // Slowly apply physics as player leaves ground
            //print("VelocityXZ_gravity_float" + velocity);
        }
        Mathf.Clamp(velocity.y, -10, 10);
        //print(controller.isGrounded);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        //Gizmos.DrawRay(transform.position, camera_Vector * 10);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, new_Direction * 5);
        //Gizmos.color = Color.red;
        //Gizmos.DrawRay(transform.position, new_Vel* 4);
    }

    void Jump()
    {
        if (controller.isGrounded && Input.GetButtonDown("Jump"))
        {
            vert_velocity = jump_Speed;
        }
    }
}*/