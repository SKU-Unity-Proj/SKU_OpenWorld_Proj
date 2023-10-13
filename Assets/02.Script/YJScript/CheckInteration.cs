using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class CheckInteration : MonoBehaviour
{
    [SerializeField]
    private float range;

    private bool interationActivated = false;

    private RaycastHit hitInfo;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private Text portalText;
    [SerializeField]
    private Text beanText;

    public GameObject portal;
    public GameObject beanStalk;
    public CinemachineVirtualCamera mainCam;
    public CinemachineVirtualCamera beanCam;

    void Update()
    {
        CheckUI();
        TryAction();
    }

    private void TryAction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckUI();
            CanInteration();
        }
    }

    private void CheckUI()
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
        interationActivated = true;
        portalText.gameObject.SetActive(true);
        portalText.text = " Æ÷Å» ¿­±â " + "<color=yellow>" + "(E)" + "</color>";
        beanText.gameObject.SetActive(true);
        beanText.text = " Äá ½É±â " + "<color=yellow>" + "(E)" + "</color>";
    }

    private void UIDisappear()
    {
        interationActivated = false;
        portalText.gameObject.SetActive(false);
        beanText.gameObject.SetActive(false);
    }

    private void CanInteration()
    {
        if (interationActivated)
        {
            if (hitInfo.transform != null)
                UIDisappear();

            if (hitInfo.transform.name == "JackPoster")
                portal.SetActive(true);

            if (hitInfo.transform.name == "BeanSpot")
                StartCoroutine("GrowBean");
        }
    }

    IEnumerator GrowBean()
    {
        beanStalk.SetActive(true);
        beanCam.MoveToTopOfPrioritySubqueue();
        yield return new WaitForSeconds(6f);
        mainCam.MoveToTopOfPrioritySubqueue();
        yield break;
    }
}