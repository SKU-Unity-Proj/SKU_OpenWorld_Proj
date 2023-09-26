using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckInteration : MonoBehaviour
{
    [SerializeField]
    private float range;  // ������ ������ ������ �ִ� �Ÿ�

    private bool pickupActivated = false;  // ������ ���� �����ҽ� True 

    private RaycastHit hitInfo;  // �浹ü ���� ����

    [SerializeField]
    private LayerMask layerMask;  // Ư�� ���̾ ���� ������Ʈ�� ���ؼ��� ������ �� �־�� �Ѵ�.

    [SerializeField]
    private Text actionText;  // �ൿ�� ���� �� �ؽ�Ʈ

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
        actionText.text = "��Ż ���� " + "<color=green>" + "(E)" + "</color>";
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