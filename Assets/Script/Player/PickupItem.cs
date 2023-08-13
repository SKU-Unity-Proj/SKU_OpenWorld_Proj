using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Animator anim;
    public GameObject Cube;
    public float radius = 1;
    public GameObject Key;
    private bool takeItem = false;

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
                    Destroy(col.gameObject);
                    Key.gameObject.SetActive(true);
                    takeItem = true;
                    return;
                }
                else if (takeItem == true)
                {
                    anim.SetTrigger("DropItem");
                    Key.gameObject.SetActive(false);
                    Instantiate(Cube, transform.position, transform.rotation);
                    takeItem = false;
                    return;
                }
            }
        }
    }
}
