using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Intro_Script_Text : MonoBehaviour
{
    TextMeshProUGUI textMesh;
    private Queue<string> dialogue_Queue; // Used for push_pop dialogue
    
    [SerializeField]
    private string[] dialogue_Text; // Array of stirngs used for dialogue

    private string[] text_Parse_Array;
    
    [SerializeField]
    private Game_Manager game_Manager;
    
    private Animator anim_One_Punch;
    private Animator anim_Dog_Intro;
    private Animator anim_Brain_Drain;

    //[HideInInspector]
    [SerializeField]
    private AudioSource[] audio_Source_Array;
    //audio_Source_0 = speech_Speed Sounds
    //audio_Source_1 = wind_Moan_Ambience

    [SerializeField]
    private GameObject one_Punch_Man;
    [SerializeField]
    private GameObject brain_Drain;
    [SerializeField]
    private GameObject dog_Intro;

    private char[] charSeparators = new char[] {'-',' '};

    private int next_Dialogue_Line;

    [HideInInspector]
    [SerializeField]
    private float speech_Speed;

    // Start is called before the first frame update
    void Start()
    {
        anim_One_Punch = one_Punch_Man.GetComponent<Animator>();
        anim_Dog_Intro = dog_Intro.GetComponent<Animator>();
        anim_Brain_Drain = brain_Drain.GetComponent<Animator>(); 
        audio_Source_Array[0] = game_Manager.GetComponent<AudioSource>();
        textMesh = GetComponent<TextMeshProUGUI>();
        //audio_Source.clip = game_Manager.audioClipArchive[0];
        dialogue_Queue = new Queue<string>();
        for(int i = 0; i <  dialogue_Text.Length; i++){ // Pushing the strings from array to Queue
            dialogue_Queue.Enqueue(dialogue_Text[i]); // 4 text DIALOGUE in the Queue
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)){
            Parse_Dialogue_Text();
            Dialogue_Audio(next_Dialogue_Line);
            StopAllCoroutines();
            StartCoroutine(Dialogue_Beep_Boop());
            if(dialogue_Queue.Count != 0){
                //string sentence = dialogue_Queue.Dequeue();
                //StartCoroutine(Type_Sentence(sentence));
            }
            //for(int i = 0; i < text_Parse_Array.Length; i++){
                //StartCoroutine(Dialogue_Beep_Boop());
                //audio_Source.Play();
            //}
        }
    }

private void Dialogue_Audio(int dialogue_Line){
    if(dialogue_Line ==  5){
        audio_Source_Array[1].PlayOneShot(game_Manager.audioClipArchive[1]);
    }
    if(dialogue_Line == 6){
        audio_Source_Array[2].PlayOneShot(game_Manager.audioClipArchive[3]);
        one_Punch_Man.SetActive(true);
    }
    if(dialogue_Line ==  9){
        audio_Source_Array[2].PlayOneShot(game_Manager.audioClipArchive[4]);
        anim_One_Punch.SetBool("hologram_Finish", true);
    }
    if(dialogue_Line == 14){
        audio_Source_Array[1].PlayOneShot(game_Manager.audioClipArchive[2]);
    }
    if(dialogue_Line ==  15){
        audio_Source_Array[2].PlayOneShot(game_Manager.audioClipArchive[3]);
        dog_Intro.SetActive(true);
        one_Punch_Man.SetActive(false);
    }
    if(dialogue_Line == 17){
        audio_Source_Array[2].PlayOneShot(game_Manager.audioClipArchive[4]);
        anim_Dog_Intro.SetBool("hologram_Finish", true);
    }
    if(dialogue_Line ==  18){
        audio_Source_Array[2].PlayOneShot(game_Manager.audioClipArchive[5]);
        brain_Drain.SetActive(true);
        dog_Intro.SetActive(false);
    }
    if(dialogue_Line == 19){
        anim_Brain_Drain.SetBool("brain_Drain_Finish", true);
    }
}

    private void Parse_Dialogue_Text(){
        if(next_Dialogue_Line < dialogue_Text.Length){
            text_Parse_Array = dialogue_Text[next_Dialogue_Line].Split(charSeparators);
            next_Dialogue_Line++;
        }
        if(next_Dialogue_Line > text_Parse_Array.Length - 1){
            //print("OUT OF DIALOGUE");
        }
    }

    private IEnumerator Dialogue_Beep_Boop(){
        textMesh.text = "";
        for(int i = 0; i < text_Parse_Array.Length; i++){
            textMesh.text += text_Parse_Array[i] + " ";
            yield return new WaitForSeconds(speech_Speed);
            if(next_Dialogue_Line == 14){
                break;
            }
            if(next_Dialogue_Line == 16){
                break;
            }
            if(next_Dialogue_Line == 18){
                break;
            }
            if(next_Dialogue_Line == 20){
                break;
            }
            if(next_Dialogue_Line == 35){
                break;
            }
            else
            {
                audio_Source_Array[0].pitch = Random.Range(0.8f, 1.0f);
                audio_Source_Array[0].PlayOneShot(game_Manager.audioClipArchive[0]);
            }
        }
    }
}
