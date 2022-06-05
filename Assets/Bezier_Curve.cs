using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier_Curve : MonoBehaviour
{
    [SerializeField]
    private Transform[] control_Points;

    private Vector3 gizmos_Postion;
    
    private void OnDrawGizmos(){
        for(float t = 0; t <= 1; t += 0.05f){
            gizmos_Postion = Mathf.Pow(1 - t, 3) * control_Points[0].position +
                3 * Mathf.Pow(1 - t, 2) * t * control_Points[1].position +
                3 * (1 - t) * Mathf.Pow(t, 2) *control_Points[2].position +
                Mathf.Pow(t, 3) * control_Points[3].position;
            
            Gizmos.DrawSphere(gizmos_Postion, 0.25f);
        }

        Gizmos.DrawLine(new Vector3(control_Points[0].position.x, control_Points[0].position.y),
            new Vector3(control_Points[1].position.x, control_Points[1].position.y));

        Gizmos.DrawLine(new Vector3(control_Points[2].position.x, control_Points[2].position.y),
            new Vector3(control_Points[3].position.x, control_Points[3].position.y));
    }
}
