using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBox : MonoBehaviour
{


    void Start()
    {
        PushBoxCollision call = GameObject.Find("Push Box").GetComponent<PushBoxCollision>();
    }

    void OnTriggerStay()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            //call.AddForceBox();
        }
    }
}
