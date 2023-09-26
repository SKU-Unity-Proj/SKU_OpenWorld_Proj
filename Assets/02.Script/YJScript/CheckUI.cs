using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckInteration : MonoBehaviour
{
    [SerializeField]
    private float range;  // 아이템 습득이 가능한 최대 거리

    private bool pickupActivated = false;  // 아이템 습득 가능할시 True 

    private RaycastHit hitInfo;  // 충돌체 정보 저장

    [SerializeField]
    private LayerMask layerMask;  // 특정 레이어를 가진 오브젝트에 대해서만 습득할 수 있어야 한다.

    [SerializeField]
    private Text actionText;  // 행동을 보여 줄 텍스트

    public GameObject portal;

    void Update()
    {
        CallInteration();
        TryAction();
    }

    private void TryAction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CallInteration();
            CanInteration();
        }
    }

    private void CallInteration()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, range, layerMask))
        {
             UIAppear();
        }
        else
            UIDisappear();
    }

    private void UIAppear()
    {
        pickupActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = "포탈 열기 " + "<color=green>" + "(E)" + "</color>";
    }

    private void UIDisappear()
    {
        pickupActivated = false;
        actionText.gameObject.SetActive(false);
    }

    private void CanInteration()
    {
        if (pickupActivated)
        {
            if (hitInfo.transform != null)
            {
                UIDisappear();
            }
            portal.SetActive(true);
        }
    }
}