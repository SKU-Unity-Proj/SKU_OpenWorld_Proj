using UnityEngine;
using System.Collections;

public class FollowCow : MonoBehaviour
{

    public Transform target; // 따라갈 타겟의 트랜스 폼

    private float relativeHeigth = 0f; // 높이 즉 y값
    private float zDistance = -3.0f;
    private float xDistance = 3.0f;
    public float dampSpeed = 4;


    void Start()
    {

        // 타겟의 트랜스 폼을 가져 왔으면.. 변수에 담는 것이 옳으나.. 좀 헤깔려셔 패스

    }

    void Update()
    {
        transform.LookAt(target.transform);

        Vector3 newPos = target.position + new Vector3(xDistance, relativeHeigth, -zDistance); // 타겟 포지선에 해당 위치를 더해.. 즉 타겟 주변에 위치할 위치를 담는다.. 일정의 거리를 구하는 방법
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * dampSpeed); //그 둘 사이의 값을 더해 보정한다. 이렇게 되면 멀어지면 따라간다.
    }
}
