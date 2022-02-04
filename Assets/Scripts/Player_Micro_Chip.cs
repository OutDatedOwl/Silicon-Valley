using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Player_Micro_Chip : MonoBehaviour
{
    #region
    [SerializeField]
    private float maximumSpeed;

    [SerializeField]
    private float jumpSpeed;

    [SerializeField]
    private float rotationSpeed;

    private float jumpButtonGracePeriod;

    [HideInInspector]
    public Vector3 velocity;

    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private float speed;
    
    [SerializeField]
    private Game_Manager game_Manager;

    [SerializeField]
    private Animator animator;

    private AnimationClip animatorClip;

    private Animal_AI animal_Bot;

    [HideInInspector]
    public CharacterController characterController;

    //[HideInInspector]
    public bool micro_Chip_Mode = true;

    //[SerializeField]
    private float gravity = -9.8f;
    private float ySpeed;
    private float normalAccel, normalSpeed;
    private float originalStepOffset;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;
    private float animationClipLength;
    private float distance;
    private bool idle_Alt;
    private bool animal_Is_Close;
    private RaycastHit hit;
    Vector3 direction_Animal_Waypoint_Move;
    Vector3 new_Pos;
    GameObject sphere;
    #endregion

    //test booleans
    //public bool player_Has_Control;
    public Transform player_Pos;
    public CinemachineFreeLook cinema;

    // Start is called before the first frame update
    void Start()
    {
        normalSpeed = speed;
        Cursor.lockState = CursorLockMode.Locked;
        animator = GetComponent<Animator>();
        //animatorClip = animator.runtimeAnimatorController.animationClips;
        //animationClipLength = animatorClip.length; 
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;
    }

    // Update is called once per frame
    void Update()
    {   
        if(Input.GetKeyDown(KeyCode.T)){
            if(!Check_For_Animals(this.transform.position, 10f) && !micro_Chip_Mode){
                animal_Bot.gameObject.layer = 7;
                animal_Bot.player_Has_Control = false;
                animal_Bot.shut_Down_Animal = true;
                animal_Bot.animator.SetBool("offline", true);
                micro_Chip_Mode = true;
            }
        }
        if(micro_Chip_Mode){
            cinema.LookAt = transform;
            cinema.Follow = transform;
            Micro_Chip_Movement();
        }
        else{
            transform.position = animal_Bot.transform.position;
        }
        // if(player_Has_Control){
        //     cinema.LookAt = transform;
        //     cinema.Follow = transform;
        //     Animal_Takeover();
        // }    
        // else
        //     return;
        //     //Animal_Movement();
    }

    private bool Check_For_Animals(Vector3 center, float radius){
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        foreach (var hitCollider in hitColliders){
            if(hitCollider.gameObject.layer == 7){ // Layer 7 is Animals 
                hitCollider.gameObject.layer = 9;
                animal_Bot = hitCollider.GetComponent<Animal_AI>();
                animal_Bot.player_Has_Control = true;
                animal_Bot.shut_Down_Animal = false;
                animal_Bot.animator.SetBool("offline", false);
                micro_Chip_Mode = false;
                return true;
            }
        }
        return false;
    }

    //If Player controlled Animal_Takeover()
    #region
    private void Micro_Chip_Movement(){
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * speed;
        //movementDirection = Vector3.ClampMagnitude(movementDirection, 1);

        Action_Inputs();

        movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        movementDirection.Normalize();

        ySpeed += gravity * Time.deltaTime;

        if (Time.time - lastGroundedTime <= jumpButtonGracePeriod)
        {
            characterController.stepOffset = originalStepOffset;
            ySpeed = -0.5f;

            animator.SetBool("isGrounded", true);
            //isGrounded = true;
            animator.SetBool("isJumping", false);
            //isJumping = false;

            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
            {
                ySpeed = jumpSpeed;
                animator.SetBool("isJumping", true);
                //isJumping = true;
                jumpButtonPressedTime = null;
                lastGroundedTime = null;
            }
        }
        else
        {
            characterController.stepOffset = 0;
        }
        //velocityXZ = velocity;
        velocity = movementDirection * magnitude;
        velocity.y = ySpeed;
        Check_Speed_Accel();

        if(characterController.enabled){
            characterController.Move(velocity * Time.deltaTime);
        }

        if (movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }    
    }
    #endregion
   
    private void Check_Speed_Accel(){
        if(speed >= maximumSpeed){
            speed = maximumSpeed;
        }
    }

    private void Action_Inputs()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jumpButtonPressedTime = Time.time;     
        }

        if(Input.GetButtonDown("Jump") && !characterController.isGrounded){
            gravity = -6f;
        }

        if (characterController.isGrounded)
        {
            gravity = -40f;
            lastGroundedTime = Time.time;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            animator.SetBool("isRunning", true);
        }
        else
            animator.SetBool("isRunning", false);

        if(characterController.isGrounded && animator.GetBool("isRunning")){

        }
        else
        {

        }
    }

    private void OnDrawGizmos(){
        Gizmos.color = Color.red;
        //Gizmos.DrawRay(this.transform.position, transform.forward * 7f);
        Gizmos.DrawRay(this.transform.position, direction_Animal_Waypoint_Move * 7f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, 10f);
    }
}
