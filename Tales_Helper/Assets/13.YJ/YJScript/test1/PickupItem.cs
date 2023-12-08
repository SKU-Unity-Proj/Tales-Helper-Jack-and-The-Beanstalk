using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Animator anim;
    public float radius = 1;
    private bool takeItem = false;

    public GameObject playerEquipPoint; //자식으로 보내질 위치

    void Awake()
    {
        //Cursor.visible = false;
    }

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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Collider[] colliders =
                    Physics.OverlapSphere(this.transform.position, radius);

            foreach (Collider col in colliders)
            {
                if (col.gameObject.tag == "Item" && takeItem == false)
                {
                    anim.SetTrigger("PickupItem");

                    //자식으로 보내기 전 물리엔진 끄기
                    Collider itemCol = col.GetComponent<BoxCollider>();
                    itemCol.isTrigger = true;
                    Rigidbody itemRigid = col.GetComponent<Rigidbody>();
                    itemRigid.isKinematic = true;

                    //아이템을 자식으로 보내고 위치와 회전값 초기화
                    col.transform.SetParent(playerEquipPoint.transform);
                    col.transform.localPosition = Vector3.zero;
                    col.transform.rotation = new Quaternion(0, 0, 0, 0);

                    takeItem = true;
                    return;
                }
                else if (col.gameObject.tag == "Item" && takeItem == true)
                {
                    anim.SetTrigger("DropItem");

                    Invoke("Throw", 1.1f);

                    takeItem = false;
                    return;
                }
            }
        }
    }

    void Throw()
    {
        Collider itemCol = playerEquipPoint.GetComponentInChildren<BoxCollider>();
        Rigidbody itemRigid = playerEquipPoint.GetComponentInChildren<Rigidbody>();

        playerEquipPoint.transform.DetachChildren();

        itemCol.isTrigger = false;
        itemRigid.isKinematic = false;

        itemRigid.AddForce(transform.forward * 180);
        itemRigid.AddForce(transform.up * 150);
    }
}
