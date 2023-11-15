using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBoxCollision : MonoBehaviour
{
    private Rigidbody rigid;
    public float pushPower;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    public void AddForceBox()
    {
        rigid.AddForce(Vector3.forward * pushPower * Time.deltaTime);
    }
}
