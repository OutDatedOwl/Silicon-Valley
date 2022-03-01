using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Open_Sesame : MonoBehaviour
{
    [HideInInspector]
    [SerializeField]
    private Animator left_Door_Anim;

    [HideInInspector]
    [SerializeField]
    private Animator right_Door_Anim;

    [SerializeField]
    private Animator tony;

    [HideInInspector]
    public bool npc_Interact_Zone_Tony;

    public bool play_Cinematic;

    [SerializeField]
    private CharacterController player;

    [SerializeField]
    private Game_Manager game_Manager;

    //[HideInInspector]
    [SerializeField]
    private Transform cinematic_Wait_00;

    private void Update(){
        game_Manager.scene_Camera_Array[1].transform.LookAt(tony.transform.position);

        if(Input.GetKeyDown(KeyCode.E) && !game_Manager.mission_00 && (npc_Interact_Zone_Tony || play_Cinematic)){
            game_Manager.tony_Script.text_To_Screen();
            npc_Interact_Zone_Tony = false;
            player.enabled = false;
            play_Cinematic = true;
            player.transform.position = cinematic_Wait_00.transform.position;

            left_Door_Anim.SetBool("Open_Sesame", true);
            right_Door_Anim.SetBool("Open_Sesame", true);
            tony.SetBool("tony_Come_Out_To_Play", true);
            game_Manager.scene_Camera_Array[0].gameObject.SetActive(false);
            game_Manager.scene_Camera_Array[1].gameObject.SetActive(true);
        }
        if(game_Manager.tony_Script.dialogue_Queue.Count == 0){
            tony.SetBool("tony_Come_Out_To_Play", false);
            left_Door_Anim.SetBool("Open_Sesame", false);
            right_Door_Anim.SetBool("Open_Sesame", false);
            game_Manager.mission_00 = true;
            play_Cinematic = false;
            StartCoroutine(cinematic_End());
        }
    }

    private void OnTriggerEnter(Collider collider){
        if(collider.tag == "2" && game_Manager.tony_Script.dialogue_Queue.Count != 0){ // If Dog enters zone
            npc_Interact_Zone_Tony = true;
            //play_Cinematic = true;
        }
    }

    private void OnTriggerExit(Collider collider){
        npc_Interact_Zone_Tony = false;
        //play_Cinematic = false;
    }

IEnumerator cinematic_End(){

    yield return new WaitForSeconds(2f);
        player.enabled = true;
        game_Manager.scene_Camera_Array[0].gameObject.SetActive(true);
        game_Manager.scene_Camera_Array[1].gameObject.SetActive(false);
        npc_Interact_Zone_Tony = false;
        game_Manager.mission_00 = true;
    }

}
