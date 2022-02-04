using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate_Switch_Script : MonoBehaviour
{
    [SerializeField]
    GameObject red_Light;
    [SerializeField]
    GameObject blue_Light;
    private bool switch_Power_On = true;
    AudioSource audioSource;
    [SerializeField]
    GameObject electric_Gate;

    private void Start(){
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider collider){
        if(collider.gameObject.tag == "Player"){
            if(switch_Power_On){
                red_Light.SetActive(false);
                blue_Light.SetActive(true);
                switch_Power_On = false;
                audioSource.Play();
                electric_Gate.SetActive(false);
            }
            else
            {
                red_Light.SetActive(true);
                blue_Light.SetActive(false);
                switch_Power_On = true;
                audioSource.Play();
                electric_Gate.SetActive(true);
            }
        }
    }
}
