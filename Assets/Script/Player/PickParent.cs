using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickParent : MonoBehaviour
{
    public Animator anim;
    public float radius = 1;
    private bool takeItem = false;

    public GameObject playerEquipPoint; //�ڽ����� ������ ��ġ

    void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    void Update()
    {
        Pickup();
    }


    void Pickup()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider[] colliders =
                    Physics.OverlapSphere(this.transform.position, radius);

            foreach (Collider col in colliders)
            {
                if (col.gameObject.tag == "Item" && takeItem == false)
                {
                    anim.SetTrigger("PickupItem");

                    //�ڽ����� ������ �� �������� ����
                    Collider itemCol = col.GetComponent<BoxCollider>();
                    itemCol.isTrigger = true;
                    Rigidbody itemRigid = col.GetComponent<Rigidbody>();
                    itemRigid.isKinematic = true;

                    //�������� �ڽ����� ������ ��ġ�� ȸ���� �ʱ�ȭ
                    col.transform.SetParent(playerEquipPoint.transform);
                    col.transform.localPosition = Vector3.zero;
                    col.transform.rotation = new Quaternion(0, 0, 0, 0);

                    takeItem = true;
                    return;
                }
                else if (col.gameObject.tag == "Item" && takeItem == true)
                {
                    playerEquipPoint.transform.DetachChildren();

                    Collider itemCol = col.GetComponent<BoxCollider>();
                    itemCol.isTrigger = false;
                    Rigidbody itemRigid = col.GetComponent<Rigidbody>();
                    itemRigid.isKinematic = false;

                    anim.SetTrigger("DropItem");

                    takeItem = false;
                    return;
                }
            }
        }
    }
}
