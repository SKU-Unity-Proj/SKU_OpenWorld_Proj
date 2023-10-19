using UnityEngine;
using System.Collections;

public class FollowCow : MonoBehaviour
{

    public Transform target; // ���� Ÿ���� Ʈ���� ��

    private float relativeHeigth = 0f; // ���� �� y��
    private float zDistance = -3.0f;
    private float xDistance = 3.0f;
    public float dampSpeed = 4;


    void Start()
    {

        // Ÿ���� Ʈ���� ���� ���� ������.. ������ ��� ���� ������.. �� ������ �н�

    }

    void Update()
    {
        transform.LookAt(target.transform);

        Vector3 newPos = target.position + new Vector3(xDistance, relativeHeigth, -zDistance); // Ÿ�� �������� �ش� ��ġ�� ����.. �� Ÿ�� �ֺ��� ��ġ�� ��ġ�� ��´�.. ������ �Ÿ��� ���ϴ� ���
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * dampSpeed); //�� �� ������ ���� ���� �����Ѵ�. �̷��� �Ǹ� �־����� ���󰣴�.
    }
}
