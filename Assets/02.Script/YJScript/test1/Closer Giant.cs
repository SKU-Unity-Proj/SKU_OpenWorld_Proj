using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class CloserGiant : MonoBehaviour
{
    public float ShakeDuration = 0.1f;          //ī�޶� ��鸲 ȿ���� ���ӵǴ� �ð�
    public float ShakeAmplitude = 1.2f;         //ī�޶� �Ķ����
    public float ShakeFrequency = 2.0f;         //ī�޶� �Ķ����

    private float ShakeElapsedTime = 0f;

    public CinemachineVirtualCamera VirtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    [SerializeField]
    private LayerMask layerMask;
    public float radius = 3;

    void Start()
    {
        if (VirtualCamera != null)
            virtualCameraNoise = VirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }
    /*
    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, radius, layerMask);

        foreach (Collider col in colliders)
        {
            StartCoroutine("FeetVibration");
            Debug.Log("Overlap");
        }
    }
    */
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Giant")
        {
            StartCoroutine("FeetVibration");
            Debug.Log("Enter");
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Giant")
        {
            Debug.Log("Stop");
            StopAllCoroutines();
        }
    }


    IEnumerator FeetVibration()
    {
        virtualCameraNoise.m_AmplitudeGain = ShakeAmplitude;
        virtualCameraNoise.m_FrequencyGain = ShakeFrequency;

        yield return new WaitForSecondsRealtime(0.1f);

        virtualCameraNoise.m_AmplitudeGain = 0f;

        yield return new WaitForSecondsRealtime(1f);
        StartCoroutine("FeetVibration");
    }
}
