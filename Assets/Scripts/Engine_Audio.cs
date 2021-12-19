using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine_Audio : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    ParticleSystem exhaust_Trail;

    //public float boost_Smoke_Rate = 2f;
    public float minimumPitch = 0.5f;
    public float maximumPitch = 0.7f;
    //public float engineSpeed;
    private float saveMaxPitch;

    Vector3 lastPosition;
    float moveMinimum;

    Player player;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        //boost_Smoke_Rate = engineSpeed;
        saveMaxPitch = maximumPitch;
        player = GetComponent<Player>();
        anim = player.GetComponent<Animator>();
        //audioSource = this.GetComponent<AudioSource>();
        audioSource.pitch = minimumPitch;
    }

    // Update is called once per frame
    void Update()
    {
        //var emission = exhaust_Trail.emission;
        //emission.rateOverTime = boost_Smoke_Rate;
        
         //engineSpeed = 1;

        if(player.boostedSpeed){
            audioSource.pitch = saveMaxPitch + 0.5f;
        }
        else{
            if(anim.GetBool("isRunning")){
                audioSource.pitch = maximumPitch;
            }
            else{
                audioSource.pitch = minimumPitch;
            }
        }

        Exhaust_Smoke();
    }

    void Exhaust_Smoke(){
        //exhaust_Smoke = exhaust_Trail.emission;

    }
}
