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
    public float maximumPitch = 0.6f;
    public float engineSpeed;
    private float saveMaxPitch;

    Player player;

    // Start is called before the first frame update
    void Start()
    {
        //boost_Smoke_Rate = engineSpeed;
        saveMaxPitch = maximumPitch;
        player = GetComponent<Player>();
        //audioSource = this.GetComponent<AudioSource>();
        audioSource.pitch = minimumPitch;
    }

    // Update is called once per frame
    void Update()
    {
        //var emission = exhaust_Trail.emission;
        //emission.rateOverTime = boost_Smoke_Rate;
        
         engineSpeed = player.speed;

        if(player.boostedSpeed){
            audioSource.pitch = saveMaxPitch + 0.3f;
        }
        else{
            if(engineSpeed < minimumPitch){
                audioSource.pitch = minimumPitch;
            }else if(engineSpeed > maximumPitch){
                audioSource.pitch = maximumPitch;
            }
            else{
                audioSource.pitch = engineSpeed;
            }
        }

        Exhaust_Smoke();
    }

    void Exhaust_Smoke(){
        //exhaust_Smoke = exhaust_Trail.emission;

    }
}
