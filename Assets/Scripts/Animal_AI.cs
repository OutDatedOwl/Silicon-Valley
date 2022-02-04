using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine.AI;
using UnityEngine;

public class Animal_AI : MonoBehaviour
{
    #region
    [SerializeField]
    private float maximumSpeed;

    [SerializeField]
    private float jumpSpeed;

    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    AudioSource audioSource_1;

    [SerializeField]
    AudioSource audioSource_2;

    private float jumpButtonGracePeriod;

    [HideInInspector]
    public Vector3 velocity;

    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private float speed;
    
    [SerializeField]
    private Game_Manager game_Manager;

    public Animator animator;

    private AnimationClip animatorClip;

    [SerializeField]
    private CharacterController characterController;

    [SerializeField]
    private float gravity = -9.8f;
    private float ySpeed;
    private float normalAccel, normalSpeed;
    private float originalStepOffset;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;
    private float animationClipLength;
    private float distance;
    private bool idle_Alt;
    private bool is_Making_Animal_Sound;
    private bool new_Partol_Point = true;
    private RaycastHit hit;
    private NavMeshAgent animal_Player;
    Vector3 direction_Animal_Waypoint_Move;
    Vector3 new_Pos;
    Vector3 new_Pos_Run;
    Vector3 new_Pos_Direction;
    GameObject sphere;
    #endregion

    //test booleans
    public bool shut_Down_Animal;
    public bool player_Has_Control;
    public Transform player_Pos;
    public CinemachineFreeLook cinema;

    // Start is called before the first frame update
    void Start()
    {
        //hit = GetComponent<RaycastHit>();
        animal_Player = GetComponent<NavMeshAgent>();
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
        if(!shut_Down_Animal && characterController.enabled){
            if(!is_Making_Animal_Sound && !player_Has_Control){
                StartCoroutine(wait_Until_Next_Animal_Sound());
            }
            if(player_Has_Control){
                cinema.LookAt = transform;
                cinema.Follow = transform;
                Animal_Player();
            }    
            else
                Animal_Computer();
        }
        if(shut_Down_Animal){
            animator.SetBool("offline", true);
        }
    }

    //If AI controlled Animal_Computer()
    #region 
    private void Animal_Computer(){
        animal_Player.isStopped = false;
        //Animal Flees
        Animal_Check(this.gameObject.tag);
        /*if(sphere_Spawn){
            if(animal_Player.velocity == Vector3.zero){
                sphere.transform.position = transform.position + conversion_From_XY(Animal_Choose_Random_Waypoint(20, 5));
                animal_Player.SetDestination(sphere.transform.position);
                StartCoroutine(wait_Until_Animal_Move_Again());
            }
            direction_Animal_Waypoint_Move = sphere.transform.position - transform.position;
            //print(conversion_From_XY(Animal_Choose_Random_Waypoint(20, 5)));
        }*/
        //Action_Inputs();
    }

    IEnumerator wait_Until_Animal_Move_Again(){
        //sphere_Spawn = false;
        animal_Player.destination = new_Pos;
        yield return new WaitForSeconds(Random.Range(1,20));
        new_Partol_Point = true;
        //sphere_Spawn = true;
        //sphere.transform.position = transform.position + conversion_From_XY(Animal_Choose_Random_Waypoint(20, 5));
    }

    private Vector3 conversion_From_XY(Vector2 from_XY){
        return new Vector3(from_XY.x, 0, from_XY.y);
    }

    private Vector2 Animal_Choose_Random_Waypoint(float axRadius,float inRadius){
        var v = Random.insideUnitCircle;
        return v.normalized * inRadius + v*(axRadius - inRadius);
    }    
    #endregion

    //If Player controlled Animal_Player()
    #region
    private void Animal_Player(){
        animal_Player.isStopped = true;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * speed;
        //movementDirection = Vector3.ClampMagnitude(movementDirection, 1);

        //Action_Inputs();
        Animal_Check(this.gameObject.tag);

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

        characterController.Move(velocity * Time.deltaTime);

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

    private void Sheep_Action_Inputs()
    {
        //If Player Controls this animal
        if(player_Has_Control){
            if (Input.GetButtonDown("Jump"))
            {
                jumpButtonPressedTime = Time.time;  
                audioSource_2.clip = game_Manager.audioClipArchive[1];    
            }

            if(Input.GetButtonDown("Jump") && !characterController.isGrounded){ // Sheep Float
                gravity = -6f;
                animator.SetBool("isFloating", true);
                if(audioSource_2.isPlaying){
                    audioSource_2.Stop();
                }
                audioSource_2.clip = game_Manager.audioClipArchive[6];
                audioSource_2.PlayOneShot(audioSource_2.clip);
            }

            if (characterController.isGrounded)
            {
                gravity = -40f;
                animator.SetBool("isFloating", false);
                lastGroundedTime = Time.time;
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                animator.SetBool("isRunning", true);
            }
            else
                animator.SetBool("isRunning", false);

            if(characterController.isGrounded && animator.GetBool("isRunning")){
                audioSource_1.clip = game_Manager.audioClipArchive[5];
                if(!audioSource_1.isPlaying){
                    //Animal_Sounds(this.gameObject.tag);
                    audioSource_1.PlayOneShot(audioSource_1.clip, 0.3f);
                }
            }
            else
            {
                audioSource_1.Stop();
            }
        }
        //If Computer controls this animal
        else
        {
            if(Vector3.Distance(player_Pos.position, transform.position) < 20){
                new_Pos_Direction = transform.position - player_Pos.position;
                new_Pos_Run = transform.position + new_Pos_Direction;
                //sphere_Spawn = false;
                animal_Player.destination = new_Pos_Run;
            }
            if(animal_Player.velocity == Vector3.zero && new_Partol_Point){
                //animal_Player.SetDestination(sphere.transform.position);
                new_Partol_Point = false;
                new_Pos = transform.position + conversion_From_XY(Animal_Choose_Random_Waypoint(20, 5));
                StartCoroutine(wait_Until_Animal_Move_Again());
            }

            if (animal_Player.velocity != Vector3.zero)
            {
                animator.SetBool("isRunning", true);
            }
            else
                animator.SetBool("isRunning", false);

            if(animal_Player.isOnNavMesh && animator.GetBool("isRunning")){
                audioSource_1.clip = game_Manager.audioClipArchive[5];
                if(!audioSource_1.isPlaying){
                    audioSource_1.PlayOneShot(audioSource_1.clip, 0.3f);
                }
            }
            else
            {
                audioSource_1.Stop();
            }
        }
    }

    private void Dog_Action_Inputs(){
        if(player_Has_Control){
            if (Input.GetButtonDown("Jump"))
            {
                jumpButtonPressedTime = Time.time;  
                audioSource_2.clip = game_Manager.audioClipArchive[1];    
            }

            if(Input.GetButtonDown("Jump") && !characterController.isGrounded){
                gravity = -6f;
                if(audioSource_2.isPlaying){
                    audioSource_2.Stop();
                }
                audioSource_2.clip = game_Manager.audioClipArchive[6];
                audioSource_2.PlayOneShot(audioSource_2.clip);
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
                audioSource_1.clip = game_Manager.audioClipArchive[13];
                if(!audioSource_1.isPlaying){
                    audioSource_1.PlayOneShot(audioSource_1.clip, 0.3f);
                }
            }
            else
            {
                audioSource_1.Stop();
            }
        }
    }

    private void Boing_Effect() {
        if(audioSource_2.clip = game_Manager.audioClipArchive[1]){
            if(!audioSource_2.isPlaying){
                audioSource_2.PlayOneShot(audioSource_2.clip);
            }
        }
    }

    IEnumerator wait_Until_Next_Animal_Sound(){
        if(this.gameObject.tag == "1"){
            int choose_Clip = Random.Range(1,4);
            is_Making_Animal_Sound = true;
            if(choose_Clip == 1){
                audioSource_1.PlayOneShot(game_Manager.audioClipArchive[9]);
            }
            if(choose_Clip == 2){
                audioSource_1.PlayOneShot(game_Manager.audioClipArchive[10]);
            }
            if(choose_Clip == 3){
                audioSource_1.PlayOneShot(game_Manager.audioClipArchive[11]);
            }
            yield return new WaitForSeconds(Random.Range(1,20));
            is_Making_Animal_Sound = false;
        }
    }

    private void Animal_Check(string animal_Name){
        int animal_Name_Switch;
        int.TryParse(animal_Name, out animal_Name_Switch);
        switch(animal_Name_Switch){
            case 1:
                Sheep_Action_Inputs(); // SHEEP---------------------------------------------- 1
                break;
            case 2:
                Dog_Action_Inputs(); // DOG-------------------------------------------------- 2
                break;
        }
    }

    private void Foot_Step(){
        audioSource_1.PlayOneShot(audioSource_1.clip, 0.3f);
    }

    private void OnDrawGizmos(){
        Gizmos.color = Color.red;
        //Gizmos.DrawRay(this.transform.position, transform.forward * 7f);
        //Gizmos.DrawRay(this.transform.position, animal_Player.getde * 7f);
    }
}