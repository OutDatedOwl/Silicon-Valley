using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Game_Manager : MonoBehaviour
{
    public AudioClip[] audioClipArchive;

    [HideInInspector]
    public bool next_Scene_Bool;

    //[HideInInspector]
    //private bool npc_Interact_Zone_Tony;
    [HideInInspector]
    public bool mission_00;
    [HideInInspector]
    public bool objective_00_01;
    [HideInInspector]
    public bool objective_00_02;

    private bool is_Co_Routine_Running;

    [HideInInspector]
    [SerializeField]
    private TextMeshProUGUI objective_Text;

    [HideInInspector]
    public Tony_Script tony_Script;

    [HideInInspector]
    public Open_Sesame open_Sesame;

    [HideInInspector]
    public Charlies_Angels charlies_Angels;

    [SerializeField]
    private Image interact_Prompt_Dog;

    //[HideInInspector]
    [SerializeField]
    private Player_Micro_Chip player;

    //[HideInInspector]
    [SerializeField]
    private Slider player_Health;

    [HideInInspector]
    [SerializeField]
    private Intro_Script_Text intro_Script_Text;

    //[HideInInspector]
    public Camera[] scene_Camera_Array;

    [HideInInspector]
    [SerializeField]
    private Transform space_Ships;

    private void Update(){

        if(player.micro_Chip_Mode){
            if(!is_Co_Routine_Running && player_Health.value != 0){
                StartCoroutine(Player_Micro_Chip_Health_Deplete());
            }
        }

        if(SceneManager.GetActiveScene().buildIndex == 1){ //INTRO
            scene_Camera_Array[0].transform.LookAt(space_Ships.position);
            if(Vector3.Distance(new Vector3(22, 128.5521f, -1096f), space_Ships.transform.position) < 200f && !next_Scene_Bool){
                next_Scene_Bool = true;
                scene_Camera_Array[0].gameObject.SetActive(false);
                scene_Camera_Array[1].gameObject.SetActive(true);
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            if(intro_Script_Text.dialogue_Queue.Count == 0 && intro_Script_Text.level_One_Ready){
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }

        if(SceneManager.GetActiveScene().buildIndex == 2){ //LEVEL ONE

        }
        if(open_Sesame.npc_Interact_Zone_Tony){
            interact_Prompt_Dog.gameObject.SetActive(true);
        }
        else
            interact_Prompt_Dog.gameObject.SetActive(false);
        if(mission_00){
            Mission_Tonys_Little_Lamb();
        }
    }

    IEnumerator Player_Micro_Chip_Health_Deplete(){
        is_Co_Routine_Running = true;
        player_Health.value = player_Health.value - 0.5f;
        yield return new WaitForSeconds(1f);
        //player_Health.value = player_Health.value - 0.5f;
        is_Co_Routine_Running = false;
    }

    private void Mission_Tonys_Little_Lamb(){
        objective_Text.text = "Sheep Gathered: " + charlies_Angels.sheep_Count;
    }

    public void Game_Start(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
