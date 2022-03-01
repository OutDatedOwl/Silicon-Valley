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

    private Queue animal_Queue = new Queue();

    //public float[] animals_Nearby = new float[10];
    private Hashtable animals_Nearby = new Hashtable();

    private int animal_Array_Position;

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

    private Collider[] hitColliders;

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
    private float animal_Distance_Stored;
    private bool idle_Alt;
    private bool animal_Bot_Distance_Acquired;
    //private bool transfer_Bodies;
    //private bool animal_Is_Close;
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
        Keep_Track_Of_Distance(this.transform.position, 10f);
        if(Input.GetKeyDown(KeyCode.T)){
            Check_For_Animals();
/*             if(!Check_For_Animals(this.transform.position, 10f) && !micro_Chip_Mode){ // No Animals in area, micro chip will remove itself from current animal 
                if(animal_Bot_Test.gameObject.layer == 9){
                    animal_Bot_Test.gameObject.layer = 7;
                    animal_Bot_Test.player_Has_Control = false;
                    animal_Bot_Test.shut_Down_Animal = true;
                    animal_Bot_Test.animator.SetBool("offline", true);
                    micro_Chip_Mode = true;
                }
            } */
        }
        if(micro_Chip_Mode){
            cinema.LookAt = transform;
            cinema.Follow = transform;
            Micro_Chip_Movement();
        }
        if(!micro_Chip_Mode){
            transform.position = animal_Bot.transform.position; // Animal found then Microchip jumps into that animal
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

    private void Keep_Track_Of_Distance(Vector3 center, float radius){
        hitColliders = Physics.OverlapSphere(center, radius);
        foreach (var hitCollider in hitColliders){ // Animals found in area--------------------------

            if(hitCollider.gameObject.layer == 7){ // Layer 7 is Animals
            float animal_Distance = Vector3.Distance(hitCollider.transform.position, transform.position);
            animal_Distance_Stored = animal_Distance;
            print("The distance from " + hitCollider.transform.position + " from " + hitCollider.name + " is " + animal_Distance);
                //animals_Nearby.Add(Vector3.Distance(transform.position, hitCollider.transform.position), hitCollider.name);
            }
        }
    }

    private void Check_For_Animals(){
        //Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        Debug.Log(animals_Nearby["DOG_SILICON_LEGS"]);
        //Debug.Log(Vector3.Distance(transform.position, hitColliders[0].transform.position));
        foreach (var hitCollider in hitColliders){ // Animals found in area--------------------------
/*             if(hitCollider.gameObject.layer == 9){
                //animal_Bot_Test = hitCollider.GetComponent<Animal_AI>();
                animal_Bot.player_Has_Control = false;
                animal_Bot.shut_Down_Animal = true;
                animal_Bot.animator.SetBool("offline", true);
                micro_Chip_Mode = true;
                animal_Bot.gameObject.layer = 7;
                //Debug.Log("ROE");
                //break;
            } */
/*             if(animal_Bot_Test != null){
                if(animal_Bot_Test.old_Bot){
                    //animal_Bot_Test.gameObject.layer = 7;
                    animal_Bot_Test.player_Has_Control = false;
                    animal_Bot_Test.shut_Down_Animal = true;
                    animal_Bot_Test.animator.SetBool("offline", true);
                    micro_Chip_Mode = true;
                    animal_Bot_Test.old_Bot = false;
                    //animal_Bot_Test_2 = animal_Bot_Test;
                    //old_Bot = false;
                    //Debug.Log("AL");
                }
            } */
            if(hitCollider.gameObject.layer == 7){ // Layer 7 is Animals
                //Debug.Log(Vector3.Distance(transform.position, hitCollider.transform.position));
                animal_Bot = hitCollider.GetComponent<Animal_AI>(); // --------- GET ANIMAL SCRIPT
                //float animal_Bot_One = Vector3.Distance(transform.position, hitCollider.transform.position);
                //animals_Nearby.Add(animal_Bot_One, animal_Bot.name);
/*                 if(animal_Array_Position > hitColliders.Length){
                    animal_Array_Position = 0;
                    for(int i = 0; i < hitColliders.Length; i++){
                        float closest_Animal = Mathf.Min(i, i + 1);  
                        Debug.Log(closest_Animal);         
                    }
                } */
                if(animal_Array_Position > hitColliders.Length){
                    animal_Array_Position = 0;
                }
                //animals_Nearby[animal_Array_Position] = animal_Bot_One;
                animal_Array_Position = animal_Array_Position + 1;
                //Debug.Log(hitColliders.Length);
                //animal_Queue.Enqueue(animal_Bot_One);
                if(animal_Bot_Distance_Acquired){
                    if(!animal_Bot.shut_Down_Animal){
                        //animal_Bot_Test.gameObject.layer = 7;
                        animal_Bot.player_Has_Control = false;
                        animal_Bot.shut_Down_Animal = true;
                        animal_Bot.animator.SetBool("offline", true);
                        micro_Chip_Mode = true;
                        //animal_Bot_Test.old_Bot = false;
                        //animal_Bot_Test_2 = animal_Bot_Test;
                        //old_Bot = false;
                        continue;
                    }
                    if(animal_Bot.shut_Down_Animal){
                        animal_Bot.player_Has_Control = true;
                        animal_Bot.shut_Down_Animal = false;
                        animal_Bot.animator.SetBool("offline", false);
                        micro_Chip_Mode = false; 
                        //old_Bot = true;
                        //animal_Bot.gameObject.layer = 9;
                        //animal_Bot_Test = animal_Bot; // store the info of the Player's bot
                        //animal_Bot_Test.old_Bot = true;
                        continue;
                    }
                }
            }
        }
/*         if(animal_Bot_Test_2 != null){
            animal_Bot_Test_2.gameObject.layer = 7;
        } */
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
