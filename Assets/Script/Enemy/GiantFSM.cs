using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//IDLE 상태 일 때 해당 위치에 가서 할 일 하게 만들기
//TRACE 상태일 때 몇 초간 플레이어가 시야에 없으면 IDLE 상태로 전환
//시야각의 높이 구별로 공격 모션 다르게 하기 (플레이어가 위에 있는지 아래있는지에 따라)
//걸어다닐때 걸어다니는 애니메이션 안됨
//checkRoom 못가져옴
//할 일 항상 하는게 아니라 몇 초동안 순찰 상태 만들기


public class GiantFSM : MonoBehaviour
{
    [SerializeField] private float viewAngle;
    [SerializeField] private float viewDistance;
    [SerializeField] private GameObject KitchenPos;

    private Transform playerTr;
    private UnityEngine.AI.NavMeshAgent agent;
    private GameObject catchBox;
    private GameObject checkRoom;
    private Animator anim;
    private int findSeconds;
    private bool isMove;
    private bool isPos;

    void Awake()
    {
        StartCoroutine(View());
        //nevMeshAgent 회전 제어
        //agent.updateRotation = false;
    }

    void Start()
    {
        ChangeState(State.IDLE);
        StartCoroutine(CheckMonsterState());
        anim = this.GetComponent<Animator>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        catchBox = this.transform.GetChild(0).gameObject;
        checkRoom = this.transform.GetChild(1).gameObject;

        isMove = false;
        isPos = false;
    }

    void Update()
    {
        RoomMove();
    }

    #region 시야각
    private Vector3 BoundaryAngle(float _angle)
    {
        _angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
    }

    IEnumerator View()
    {
        Vector3 _leftBoundary = BoundaryAngle(-viewAngle * 0.5f);
        Vector3 _rightBoundary = BoundaryAngle(viewAngle * 0.5f);

        Debug.DrawRay(transform.position + transform.up, _leftBoundary, Color.red);
        Debug.DrawRay(transform.position + transform.up, _rightBoundary, Color.red);

        Collider[] _target = Physics.OverlapSphere(transform.position, viewDistance);

        for (int i = 0; i < _target.Length; i++)
        {
            Transform _targetTf = _target[i].transform;
            if (_targetTf.name == "Player")
            {
                Vector3 _direction = (_targetTf.position - transform.position).normalized;
                float _angle = Vector3.Angle(_direction, transform.forward);

                if (_angle < viewAngle * 0.5f)
                {
                    RaycastHit _hit;
                    if (Physics.Raycast(transform.position + transform.up, _direction, out _hit, viewDistance))
                    {
                        if (_hit.transform.name == "Player")
                        {
                            //Debug.Log("플레이어가 시야 내에 있습니다.");

                            if (this.state != State.TRACE && catchBox.activeSelf == false)
                            {
                                ChangeState(State.TRACE);
                            }

                            Debug.DrawRay(transform.position + transform.up, _direction, Color.blue);
                        }
                        else if (this.state == State.TRACE && _targetTf==null)
                        {
                            CantFindPlayer();
                            Debug.Log("cantfind");
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(View());

    }
    #endregion

    void  CantFindPlayer()
    {
        findSeconds++;
        if(findSeconds > 20)
        {
            ChangeState(State.IDLE);
        }
    }

    #region 거인 FSM

    public enum State
    {
        WALK,
        IDLE,
        TRACE,
        ATTACK
    }

    public State state;

    IEnumerator CheckMonsterState()
    {
        yield return new WaitForSeconds(0.5f);

        switch (state)
        {
            case State.WALK:
                Walk();
                break;
            case State.IDLE:
                IDLE();
                break;
            case State.TRACE:
                TRACE();
                break;
            case State.ATTACK:
                ATTACK();
                break;
        }
        StartCoroutine(CheckMonsterState());
    }

    private void OnTriggerStay(Collider other)
    {
        if (state == State.WALK)
        {
            WalkTriggerStay(other);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (state)
        {
            case State.WALK:
                WalkTrigger(other);
                break;
            case State.IDLE:
                IDLETrigger(other);
                break;
            case State.TRACE:
                TRACETrigger(other);
                break;
            case State.ATTACK:
                ATTACKTrigger(other);
                break;
        }
    }

    public void ChangeState(State state)
    {
        switch (this.state)
        {
            case State.WALK:
                WalkExit();
                break;
            case State.IDLE:
                IDLEExit();
                break;
            case State.TRACE:
                TRACEExit();
                break;
            case State.ATTACK:
                ATTACKExit();
                break;
        }

        this.state = state;

        switch (state)
        {
            case State.WALK:
                WalkEnter();
                break;
            case State.IDLE:
                IDLEEnter();
                break;
            case State.TRACE:
                TRACEEnter();
                break;
            case State.ATTACK:
                ATTACKEnter();
                break;
        }
    }



    private void WalkEnter()
    {

    }
    private void Walk()
    {

    }
    private void WalkTrigger(Collider other)
    {

    }
    private void WalkTriggerStay(Collider other)
    {

    }
    private void WalkExit()
    {

    }




    private void IDLEEnter()
    {
        findSeconds = 0;
        transform.GetChild(1).gameObject.SetActive(true);
        StartCoroutine("IdleToWander");
    }
    private void IDLE()
    {

    }
    private void IDLETrigger(Collider IDLEcol)
    {
        if (IDLEcol.GetComponent<Collider>().CompareTag("Kitchen"))
        {
            isMove = true;
        }
    }
    private void IDLEExit()
    {
        transform.GetChild(1).gameObject.SetActive(false);
    }




    private void TRACEEnter()
    {
        Debug.Log("Trace Start");
        catchBox.SetActive(true);
    }
    private void TRACE()
    {
        agent.SetDestination(playerTr.position);
    }
    private void TRACETrigger(Collider other)
    {
        if (other.GetComponent<Collider>().CompareTag("Player"))
        {
            ChangeState(State.ATTACK);
        } 
    }
    private void TRACEExit()
    {
        catchBox.SetActive(false);
    }




    private void ATTACKEnter()
    {
        Debug.Log("ATTACK Start");
        anim.SetTrigger("Catch");
    }
    private void ATTACK()
    {
        ChangeState(State.IDLE);
    }
    private void ATTACKTrigger(Collider other)
    {

    }
    private void ATTACKExit()
    {
        
    }
    #endregion

    #region 거인 배회
    IEnumerator IdleToWander()
    {
        int changeTime = Random.Range(1, 5);

        yield return new WaitForSeconds(changeTime);

        float currentTime = 0;
        float maxTime = 10;

        //이동 속도 설정
        //agent.speed = status.WalkSpeed;

        //목표 위치 설정
        agent.SetDestination(CalculateWanderPosition());

        //목표 위치로 회전
        Vector3 to = new Vector3(agent.destination.x, 0, agent.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation = Quaternion.LookRotation(to - from);

        while (true)
        {
            currentTime += Time.deltaTime;

            //목표 위치에 근접하거나 오랜시간 배회하기 상태에 머물러 있으면
            to = new Vector3(agent.destination.x, 0, agent.destination.z);
            from = new Vector3(transform.position.x, 0, transform.position.z);
            if ((to - from).sqrMagnitude < 0.1f || currentTime >= maxTime && state == State.IDLE)
            {
                StartCoroutine("IdleToWander");
            }
            yield return null;
        }
    }

    private Vector3 CalculateWanderPosition()
    {
        float wanderRadius = 30; //현재 위치를 원점으로 하는 반지름
        int wanderJitter = 0; //선택된 각도
        int wanderJitterMin = 0;
        int wanderJitterMax = 360;

        //현재 적 캐릭터가 월드의 중심 위치와 크기(구역을 벗어난 행동을 하지 않도록)
        Vector3 rangePosition = Vector3.zero;
        Vector3 rangeScale = Vector3.one * 100.0f;

        //자신의 위치를 중심으로 반지름, 거리, 각도에 위치한 좌표를 목표지점으로 설정
        wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targerPosition = transform.position + SetAngle(wanderRadius, wanderJitter);

        //생성된 목표위치가 자신의 이동구역을 벗어나지 않게 조절
        targerPosition.x = Mathf.Clamp(targerPosition.x, rangePosition.x - rangeScale.x * 0.5f, rangePosition.x + rangeScale.x * 0.5f);
        targerPosition.y = 0.0f;
        targerPosition.z = Mathf.Clamp(targerPosition.z, rangePosition.z - rangeScale.z * 0.5f, rangePosition.z + rangeScale.z * 0.5f);

        return targerPosition;
    }

    private Vector3 SetAngle(float radius, int angle)
    {
        Vector3 position = Vector3.zero;

        position.x = Mathf.Cos(angle) * radius;
        position.z = Mathf.Sin(angle) * radius;

        return position;
    }
    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, agent.destination - transform.position);
    }
    */
    #endregion
    //방 위치에 따라 청소, TV보러 가기 등 할 일 위치로 이동하는 스크립트
    private void RoomMove()
    {
        if (isMove == true && isPos == false && state == State.IDLE )
        {
            var dir = KitchenPos.transform.position - transform.position;
            transform.position += dir.normalized * Time.deltaTime * 4f;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 4f);
            //걷기 애니메이션 키기
        }
        
        if (Vector3.Distance(transform.position, KitchenPos.transform.position) <= 0.3f)
        {
            isMove = false;
            isPos = true;
            //걷기 애니메이션 끄기
        }
        else
        {
            isPos = false;
        }
    }
}
