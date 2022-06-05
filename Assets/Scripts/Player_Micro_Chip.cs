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
    private Transform[] control_Points;
    
    [SerializeField]
    private float speed;
    
    [SerializeField]
    private Game_Manager game_Manager;

    [SerializeField]
    private Animator animator;

    private AnimationClip animatorClip;

    private Animal_AI animal_Bot;
    private Animal_AI test_animal_Bot;
    //private Animal_AI free_Animal;

    private Collider[] hitColliders;

    [HideInInspector]
    public CharacterController characterController;

    //[HideInInspector]
    public bool micro_Chip_Mode = true;

    private GameObject[] animal_Close_Array;

    [SerializeField]
    private float speedModifier;
    private float tParam;
    private Vector3 gizmos_Position, starting_Point, mid_Point_1,
        mid_Point_2, ending_Point;
    private float gravity = -9.8f;
    private float ySpeed;
    private float normalAccel, normalSpeed;
    private float originalStepOffset;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;
    private float animationClipLength;
    private float distance;
    private bool idle_Alt;
    private RaycastHit hit;
    private Vector3 test_Line, control_Point_1, control_Point_2, 
        control_Point_3, control_Point_4;
    //Vector3 direction_Animal_Waypoint_Move;
    //Vector3 new_Pos;
    public CinemachineFreeLook cinema;
    private Renderer micro_Chip;
    public bool animal_Contact, changing_Animal;
    private bool player_Micro_Chip_Moving;
    private float wait_Timer;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        micro_Chip = this.GetComponentInChildren<Renderer>();
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
        if(player_Micro_Chip_Moving){
            wait_Timer += Time.deltaTime;
            if(wait_Timer > 2f){
                wait_Timer = 0f;
                //player_Micro_Chip_Moving = false;
            }
        }
        Keep_Track_Of_Distance(transform.position, 10f);
        if(Input.GetKeyDown(KeyCode.T)){
            Check_For_Animals();
        }
        if(!micro_Chip_Mode && !changing_Animal){
            transform.position = test_animal_Bot.transform.position; // Animal found then Microchip jumps into that animal
            //micro_Chip.enabled = false;
            if(transform.position == test_animal_Bot.transform.position){
                micro_Chip.enabled = false;
            }
        }
        else
            micro_Chip.enabled = true;
        if(!changing_Animal){
            starting_Point = control_Points[0].position;
            mid_Point_1 = control_Points[1].position;
            mid_Point_2 =  control_Points[2].position;
            ending_Point = control_Points[3].position;
        }
    }

    void FixedUpdate(){
        if(micro_Chip_Mode){
            cinema.LookAt = transform;
            cinema.Follow = transform;
            Micro_Chip_Movement();
            //micro_Chip.enabled = true;
        }

        if(changing_Animal){
           StartCoroutine(Commencing_Change_Sequence());
        }
    }

    private IEnumerator Commencing_Change_Sequence(){
        
        while(tParam < 1){
            tParam += Time.deltaTime * speedModifier / 100f;
            transform.position = (Mathf.Pow(1 - tParam, 3) * starting_Point +
                3 * Mathf.Pow(1 - tParam, 2) * tParam * mid_Point_1 +
                3 * (1 - tParam) * Mathf.Pow(tParam, 2) * mid_Point_2 +
                Mathf.Pow(tParam, 3) * ending_Point);
                
                //transform.position = chip_Location.position;
                yield return new WaitForFixedUpdate();
        }
        tParam = 0f;
        StopAllCoroutines();
        changing_Animal = false;
    }

    private void Keep_Track_Of_Distance(Vector3 center, float radius){
        hitColliders = Physics.OverlapSphere(center, radius);
        //Vector3 currentPos = transform.position;

        if(!Contact_Made()){
            animal_Bot = null;
        }
    }

    private bool Contact_Made(){ // If closest animal is not player_Controlled
        float closest_Distance = Mathf.Infinity;
        Transform close_Animal = null;
        animal_Contact = false;
        foreach (var hitCollider in hitColliders){ // Animals found in area--------------------------
            if(hitCollider.gameObject.layer == 7){ // Layer 7 is Animals

                float current_Distance;
                animal_Contact = true;
                current_Distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if(current_Distance < closest_Distance){
                    test_Line = (hitCollider.transform.position - transform.position ) / 2; // Finding mid-points for Bezier-curve control points
                    test_Line = transform.position + test_Line;
                    control_Point_3 = (test_Line - transform.position) / 2;
                    control_Point_3 = hitCollider.transform.position - control_Point_3;
                    control_Point_2 = (hitCollider.transform.position - test_Line) / 2;
                    control_Point_2 = transform.position + control_Point_2;

                    closest_Distance = current_Distance;
                    close_Animal = hitCollider.transform;
                    //print(close_Animal.name + " is the closest animal at " + Vector3.Distance(transform.position, close_Animal.position));
                    //test_animal_Bot = close_Animal.GetComponent<Animal_AI>();
                    animal_Bot = close_Animal.GetComponent<Animal_AI>(); 
                        control_Points[0].position = transform.position; // SHOULD BE FROM WHERE I PRESSED KEY T
                        control_Points[1].position = control_Point_2 + Vector3.up * 2;
                        control_Points[2].position = control_Point_3 + Vector3.up * 2;
                        control_Points[3].position = animal_Bot.transform.position;
                }
            }

        }
        
        if(animal_Contact){
            return true;
        }
        else
            return false;
    }

    private void Check_For_Animals(){ // Press T Key, will control nearby animal

        if(test_animal_Bot != null){
            if(!test_animal_Bot.shut_Down_Animal){
                changing_Animal = true;
                test_animal_Bot.gameObject.layer = 7;
                player_Micro_Chip_Moving = true;
                test_animal_Bot.player_Has_Control = false;
                test_animal_Bot.shut_Down_Animal = true;
                test_animal_Bot.animator.SetBool("offline", true);
                micro_Chip_Mode = true;
                test_animal_Bot = null;
            }
        }

        if(animal_Bot != null){
            //test_animal_Bot = animal_Bot;
            if(animal_Bot.shut_Down_Animal){
                changing_Animal = true;                
                test_animal_Bot = animal_Bot;
                animal_Bot.gameObject.layer = 9;
                player_Micro_Chip_Moving = true;
                animal_Bot.player_Has_Control = true;
                animal_Bot.shut_Down_Animal = false;
                animal_Bot.animator.SetBool("offline", false);
                micro_Chip_Mode = false;
            }
        }
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
        Gizmos.color = Color.white;
        //Gizmos.DrawRay(this.transform.position, transform.forward * 7f);
        //Gizmos.DrawRay(this.transform.position, direction_Animal_Waypoint_Move * 7f);
        //Gizmos.DrawWireSphere(this.transform.position, 10f);


        if(animal_Contact){
            for(float t = 0; t <= 1; t += 0.05f){
                gizmos_Position = Mathf.Pow(1 - t, 3) * control_Points[0].position +
                3 * Mathf.Pow(1 - t, 2) * t * control_Points[1].position +
                3 * (1 - t) * Mathf.Pow(t, 2) *control_Points[2].position +
                Mathf.Pow(t, 3) * control_Points[3].position;
            
                Gizmos.DrawSphere(gizmos_Position, 0.25f);
            }
        }

        //Gizmos.DrawLine(new Vector3(control_Points[0].position.x, control_Points[0].position.y),
        //    new Vector3(control_Points[1].position.x, control_Points[1].position.y));

        //Gizmos.DrawLine(new Vector3(control_Points[2].position.x, control_Points[2].position.y),
        //    new Vector3(control_Points[3].position.x, control_Points[3].position.y));
        Gizmos.DrawWireSphere(control_Points[0].position, 1f);
        Gizmos.DrawWireSphere(control_Points[3].position, 2f);

        Gizmos.color = Color.blue;
        if(animal_Contact){
            Gizmos.DrawSphere(control_Points[1].position, 0.5f);
        }
        Gizmos.color = Color.cyan;
        if(animal_Contact){
            Gizmos.DrawSphere(control_Points[2].position, 0.5f);
        }

        Gizmos.DrawSphere(starting_Point, 1f);
        Gizmos.DrawSphere(mid_Point_1, 1f);
        Gizmos.DrawSphere(mid_Point_2, 1f);
        Gizmos.DrawSphere(ending_Point, 1f);
    }
}
