using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Charlies_Angels : MonoBehaviour
{
    [SerializeField]
    private Game_Manager game_Manager;

    [SerializeField]
    private TextMeshProUGUI textMesh;

    AudioSource audio_Source;

    [HideInInspector]
    public int sheep_Count;

    private void Start(){
        audio_Source = game_Manager.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider collider){
        if(collider.gameObject.layer == 7){
            if(sheep_Count <= 4){
                sheep_Count++;
                //textMesh.text = "Sheep: " + sheep_Count;
                audio_Source.PlayOneShot(game_Manager.audioClipArchive[8], 1.0f);
            }
            if(sheep_Count == 4){
                audio_Source.PlayOneShot(game_Manager.audioClipArchive[12], 1.0f);
                game_Manager.mission_00 = false;
                game_Manager.objective_00_01 = true;
            }
        }
    }

    private void OnTriggerExit(Collider collider){
        if(collider.gameObject.layer == 7){
            sheep_Count--;
        }
    }
}
