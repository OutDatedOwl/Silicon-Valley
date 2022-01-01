using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    public AudioClip[] audioClipArchive;

    [HideInInspector]
    [SerializeField]
    Camera[] scene_Camera_Array;

    [HideInInspector]
    [SerializeField]
    Transform space_Ships;

    private void Update(){
        //scene_Camera_Array[0].transform.LookAt(space_Ships.position);
    }

}
