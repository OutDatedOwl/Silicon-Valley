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

    public string[] text_Parse_Array;
    
    [SerializeField]
    private Game_Manager game_Manager;
    
    [SerializeField]
    private AudioSource[] audio_Source_Array;

    private int next_Dialogue_Line;

    public float speech_Speed;

    // Start is called before the first frame update
    void Start()
    {
        //audio_Source_0 = speech_Speed Sounds
        //audio_Source_1 = ambience
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
            StartCoroutine(Dialogue_Beep_Boop());
            if(dialogue_Queue.Count != 0){
                textMesh.text = dialogue_Queue.Dequeue();
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
    if(dialogue_Line ==  10){
        audio_Source_Array[1].PlayOneShot(game_Manager.audioClipArchive[2]);
    }
}

    private void Parse_Dialogue_Text(){
        if(next_Dialogue_Line < dialogue_Text.Length){
            text_Parse_Array = dialogue_Text[next_Dialogue_Line].Split(char.Parse(" "));
            next_Dialogue_Line++;
        }
        if(next_Dialogue_Line > text_Parse_Array.Length - 1){
            //print("OUT OF DIALOGUE");
        }
    }

    private IEnumerator Dialogue_Beep_Boop(){
        for(int i = 0; i < text_Parse_Array.Length; i++){
            yield return new WaitForSeconds(speech_Speed);
            audio_Source_Array[0].pitch = Random.Range(0.8f, 1.0f);
            audio_Source_Array[0].PlayOneShot(game_Manager.audioClipArchive[0]);
        }
    }
}
