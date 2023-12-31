using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.Timeline;

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

    private PlayableDirector playableDirector;

    void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();
    }

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
        portalText.text = " 포탈 열기 " + "<color=yellow>" + "(E)" + "</color>";
        beanText.gameObject.SetActive(true);
        beanText.text = " 콩 심기 " + "<color=yellow>" + "(E)" + "</color>";
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
        //플레이어 이동시키기
        beanStalk.SetActive(true);
        beanCam.MoveToTopOfPrioritySubqueue();
        beanCam.Priority = 11;
        mainCam.Priority = 10;
        yield return new WaitForSeconds(9f);

        //타임라인 실행
        playableDirector.Play();

        yield return new WaitForSeconds(9f);
        SceneManager.LoadScene("Intro");

        yield break;
    }
}