using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprite_Look_Camera : MonoBehaviour
{
    [SerializeField]
    private Camera cam; 

    void Update()
    {
        transform.LookAt(cam.transform);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }
}
