using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//IDLE 상태 일 때 할 일 하고 있게 하기
//TRACE 상태일 때 몇 초간 플레이어가 시야에 없으면 IDLE 상태로 전환
//시야각의 높이 구별로 공격 모션 다르게 하기 (플레이어가 위에 있는지 아래있는지에 따라)
//걸어다닐때 걸어다니는 애니메이션


public class GiantFSM : MonoBehaviour
{
    [SerializeField] private float viewAngle;
    [SerializeField] private float viewDistance;

    private Transform playerTr;
    private Transform giantTr;
    private UnityEngine.AI.NavMeshAgent agent;
    private GameObject catchBox;
    private Animator anim;
    private int findSeconds = 0;

    public enum State
    {
        WALK,
        IDLE,
        TRACE,
        ATTACK
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

    public State state;

    void Awake()
    {
        StartCoroutine(View());
    }

    void Start()
    {
        ChangeState(State.IDLE);
        StartCoroutine(CheckMonsterState());
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        giantTr = GetComponent<Transform>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        catchBox = this.transform.GetChild(0).gameObject;
        anim = this.GetComponent<Animator>();
    }

    void Update()
    {

    }

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
    }
    private void IDLE()
    {
    }
    private void IDLETrigger(Collider other)
    {

    }
    private void IDLEExit()
    {
        
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

    }




    private void ATTACKEnter()
    {
        Debug.Log("ATTACK Start");
        anim.SetTrigger("Catch");
    }
    private void ATTACK()
    {
        //ChangeState(State.IDLE);
    }
    private void ATTACKTrigger(Collider other)
    {

    }
    private void ATTACKExit()
    {
        catchBox.SetActive(false);
    }
}
