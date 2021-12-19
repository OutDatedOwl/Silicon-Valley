using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float maximumSpeed;

    [SerializeField]
    private float maximumAccel;

    [SerializeField]
    private float jumpSpeed;

    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    AudioSource audioSource;

    private float jumpButtonGracePeriod;
    public bool boostedSpeed;

    [HideInInspector]
    public Vector3 velocityXZ, velocity;

    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private float speed, accel;
    
    [SerializeField]
    private Game_Manager game_Manager;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private AnimationClip animatorClip;

    [SerializeField]
    private CharacterController characterController;

    private float ySpeed;
    private float normalAccel, normalSpeed;
    private float originalStepOffset;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;
    private float animationClipLength;
    public int chomp_Bark_Sound;
    private bool isJumping;
    private bool isGrounded;
    private bool idle_Alt;
    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        //hit = GetComponent<RaycastHit>();
        normalSpeed = speed;
        normalAccel = accel;
        Cursor.lockState = CursorLockMode.Locked;
        animator = GetComponent<Animator>();
        //animatorClip = animator.runtimeAnimatorController.animationClips;
        animationClipLength = animatorClip.length; 
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        //float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);
        movementDirection = Vector3.ClampMagnitude(movementDirection, 1);

        //animator.SetFloat("Input Magnitude", inputMagnitude, 0.05f, Time.deltaTime);
        Action_Inputs();
        On_Hit_Collide();
        //Anime();

        movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        movementDirection.Normalize();

        //speed = inputMagnitude * maximumSpeed;
        //accel = inputMagnitude * maximumAccel;
        //float normalSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            speed = speed + 1.0f;
            accel = accel + 1.0f;
            boostedSpeed = true;
        }
        else
            {
                speed  = normalSpeed;
                accel = normalAccel;
                boostedSpeed = false;
            }
        ySpeed += Physics.gravity.y * Time.deltaTime;

        if (Time.time - lastGroundedTime <= jumpButtonGracePeriod)
        {
            characterController.stepOffset = originalStepOffset;
            ySpeed = -0.5f;
            animator.SetBool("isGrounded", true);
            isGrounded = true;
            animator.SetBool("isJumping", false);
            isJumping = false;

            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod && !animator.GetBool("bite_Chomp"))
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
        velocityXZ = velocity;
        Check_Speed_Accel();
        velocityXZ = Vector3.Lerp(velocityXZ, movementDirection * speed, accel * Time.deltaTime);//movementDirection * speed;
        velocity.y = ySpeed;
        velocity = new Vector3(velocityXZ.x, velocity.y, velocityXZ.z);

        characterController.Move(velocity * Time.deltaTime);

        if (movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }        
    }

    private void Check_Speed_Accel(){
        if(speed >= maximumSpeed){
            speed = maximumSpeed;
        }
        if(accel >= maximumAccel){
            accel = maximumAccel;
        }
    }

    private void Action_Inputs()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jumpButtonPressedTime = Time.time;  
            audioSource.clip = game_Manager.audioClipArchive[1];    
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

        if(Input.GetKeyDown(KeyCode.E) && animator.GetBool("bite_Chomp") == !true && !animator.GetBool("isJumping")){
            animator.SetBool("bite_Chomp", true);
        }

    }

    private void Boing_Effect() {
        if(audioSource.clip = game_Manager.audioClipArchive[1]){
            audioSource.PlayOneShot(audioSource.clip);
        }
    }

    private void Chomp_Down() {
        chomp_Bark_Sound = Random.Range(0, 2);
        if(chomp_Bark_Sound == 0){
            audioSource.clip = game_Manager.audioClipArchive[2];
            audioSource.PlayOneShot(audioSource.clip, 0.3f);
        }
        if(chomp_Bark_Sound == 1){
            audioSource.clip = game_Manager.audioClipArchive[4];
            audioSource.PlayOneShot(audioSource.clip, 0.3f);
        }
        //if(audioSource.clip = game_Manager.audioClipArchive[2]){
        //    audioSource.PlayOneShot(audioSource.clip, 0.3f);
        //}
        //if(audioSource.clip = game_Manager.audioClipArchive[4]){
        //   audioSource.PlayOneShot(audioSource.clip, 0.3f);
        //}
    }

    private void Chomp_Down_Finish() {
        animator.SetBool("bite_Chomp",false);
    }

    private void On_Hit_Collide()
    {   
        if(Physics.Raycast(transform.position, transform.forward, out hit, 7f)){
            if(hit.collider.gameObject.layer == 6 && speed > 15){
                //if(!audioSource.isPlaying){
                //    animator.SetBool("has_Collided", true);
                //}
                //player_Collision_Something = true;
                if(!animator.GetBool("has_Collided")){
                    animator.SetBool("has_Collided", true);
                    StartCoroutine(Dog_Collide());
                }
                //Dog_Collide_Whimper_SX();
            }
        }
    }

    IEnumerator Dog_Collide(){
        //Dog_Collide_Whimper_SX();
        //if(animator.GetBool("has_Collided")){
        //    Dog_Collide_Whimper_SX();
        //}
        yield return new WaitForSeconds(1f);
        animator.SetBool("has_Collided", false);
        //Dog_Collide_Whimper_SX();
        //player_Collision_Something = animator.GetBool("has_Collided");
    }

    //private void Dog_Collide_Whimper_SX(){
    //    audioSource.clip = game_Manager.audioClipArchive[3];
    //   audioSource.PlayOneShot(audioSource.clip, 0.3f);
    //}

    private void Damage_Sound_Effect(){
        audioSource.clip = game_Manager.audioClipArchive[3];
        audioSource.PlayOneShot(audioSource.clip, 0.2f);
    }

    private void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawRay(this.transform.position, transform.forward * 7f);
    }
}