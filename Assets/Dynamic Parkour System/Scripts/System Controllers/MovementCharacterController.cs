
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Climbing
{
    public enum MovementState { Walking, Running }

    [RequireComponent(typeof(ThirdPersonController))]
    public class MovementCharacterController : MonoBehaviour
    {
        public bool showDebug = true;  // 디버그 메시지 표시 여부

        [HideInInspector] public Rigidbody rb;  // 캐릭터의 Rigidbody 컴포넌트
        [HideInInspector] public bool limitMovement = false;  // 움직임 제한 여부
        [HideInInspector] public bool stopMotion = false;  // 모션 중지 여부
        [HideInInspector] public float velLimit = 0;  // 속도 제한 값
        [HideInInspector] public float curSpeed = 6f;  // 현재 속도

        private ThirdPersonController controller;  // ThirdPersonController 참조
        private MovementState currentState;  // 현재 움직임 상태
        private Animator anim;  // 애니메이터 컴포넌트
        private Vector3 velocity;  // 현재 속도 벡터
        private float smoothSpeed;  // 부드러운 속도 조절을 위한 변수

        // 착지 및 낙하 이벤트를 위한 델리게이트와 이벤트
        public delegate void OnLandedDelegate();
        public delegate void OnFallDelegate();
        public event OnLandedDelegate OnLanded;
        public event OnFallDelegate OnFall;

        [Header("Movement Settings")]
        public float walkSpeed;  // 걷는 속도
        public float JogSpeed;  // 조깅 속도
        public float RunSpeed;  // 달리기 속도
        public float fallForce;  // 낙하 시 적용되는 힘

        [Header("Feet IK")]  // 발 IK 관련 설정
        public bool enableFeetIK = true;  // 발 IK 활성화 여부

        [SerializeField] private float heightFromGroundRaycast = 0.7f;  // 땅으로부터의 레이캐스트 높이
        [Range(0, 2f)] [SerializeField] private float raycastDownDistance = 1.5f;  // 레이캐스트 거리
        [SerializeField] private float pelvisOffset = 0f;  // 골반 오프셋

        [Range(0, 1f)] [SerializeField] private float pelvisUpDownSpeed = 0.25f;  // 골반 위아래 이동 속도
        [Range(0, 1f)] [SerializeField] private float feetToIKPositionSpeed = 0.25f;  // 발 IK 위치로 이동하는 속도

        public string leftFootAnim = "LeftFootCurve";  // 왼쪽 발 애니메이션 이름
        public string rightFootAnim = "RightFootCurve";  // 오른쪽 발 애니메이션 이름

        private Vector3 leftFootPosition, leftFootIKPosition, rightFootPosition, rightFootIKPosition;  // 발 위치 및 IK 위치 변수
        private Quaternion leftFootIKRotation, rightFootIKRotation;  // 발 IK 회전 변수
        private float lastPelvisPositionY, lastLeftFootPosition, lastRightFootPosition;  // 이전 골반 및 발 위치

        void Start()
        {
            // 컴포넌트 초기화
            controller = GetComponent<ThirdPersonController>();
            rb = GetComponent<Rigidbody>();
            anim = controller.characterAnimation.animator;
            SetCurrentState(MovementState.Walking);
        }

        void Update()
        {
            // 플레이어 점프 및 착지 처리
            if (controller.isJumping)
            {
                controller.allowMovement = true;

                if (!controller.isGrounded)
                {
                    Fall();
                }
                else if (controller.isGrounded && controller.onAir)
                {
                    Landed();
                }
            }

        }

        private void FixedUpdate()
        {
            // 땅에 닿았을 때 움직임 제한
            if (controller.isGrounded)
            {
                limitMovement = CheckBoundaries();
            }

            // 플레이어 움직임 적용
            if (!controller.dummy)
            {
                if (!stopMotion && !controller.characterAnimation.animState.IsName("Fall"))
                {
                    ApplyInputMovement();
                }
            }

            // 낙하 중 움직임 부여
            if (!controller.dummy && controller.isJumping && controller.characterInput.movement != Vector2.zero && !controller.isVaulting)
            {
                rb.position += (transform.forward * walkSpeed) * Time.fixedDeltaTime;
            }

            // 발 IK 위치 조절
            if (!enableFeetIK || controller.dummy)
                return;
            if (anim == null)
                return;

            // IK 위치 얻기
            AdjustFeetTarget(ref rightFootPosition, HumanBodyBones.RightFoot);
            AdjustFeetTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);

            // 땅으로 레이캐스트
            FeetPositionSolver(rightFootPosition, ref rightFootIKPosition, ref rightFootIKRotation);
            FeetPositionSolver(leftFootPosition, ref leftFootIKPosition, ref leftFootIKRotation);
        }

        #region Movement



        public void ApplyInputMovement()
        {
            if (GetState() == MovementState.Running)
            {
                velocity.Normalize(); // 달리기 상태에서는 속도를 정규화
            }

            if (velocity.magnitude > 0.3f)
            {
                // 플레이어의 움직임에 입력을 적용
                smoothSpeed = Mathf.Lerp(smoothSpeed, curSpeed, Time.fixedDeltaTime * 2);
                rb.velocity = new Vector3(velocity.x * smoothSpeed, velocity.y * smoothSpeed + rb.velocity.y, velocity.z * smoothSpeed);

                // 불규칙한 표면에 플레이어가 있을 때 움직임 조절
                RaycastHit hit;
                controller.characterDetection.ThrowRayOnDirection(transform.position, Vector3.down, 1.0f, out hit);
                if (hit.normal != Vector3.up)
                {
                    controller.inSlope = true; // 경사면에 있다는 것을 표시
                    rb.velocity += -new Vector3(hit.normal.x, 0, hit.normal.z) * 1.0f; // 경사면 방향 반대로 움직임 조절
                    rb.velocity = rb.velocity + Vector3.up * Physics.gravity.y * 1.6f * Time.fixedDeltaTime; // 추가 중력 적용
                }
                else
                {
                    controller.inSlope = false; // 경사면이 아닌 경우 플래그 해제
                }

                // 작은 장애물을 자동으로 넘어가는 기능
                AutoStep();

                // 애니메이션의 움직임 속도 설정
                controller.characterAnimation.SetAnimVelocity(rb.velocity);
            }
            else
            {
                // 입력이 없을 때 움직임을 천천히 줄임
                smoothSpeed = Mathf.SmoothStep(smoothSpeed, 0, Time.fixedDeltaTime * 20);
                rb.velocity = new Vector3(rb.velocity.normalized.x * smoothSpeed, rb.velocity.y, rb.velocity.normalized.z * smoothSpeed);
                controller.characterAnimation.SetAnimVelocity(controller.characterAnimation.GetAnimVelocity().normalized * smoothSpeed);
            }

            // 중력을 적용하여 플레이어가 떨어질 때 가속도 증가
            if (rb.velocity.y <= 0)
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (fallForce - 1) * Time.fixedDeltaTime;
            }
        }

        /// <summary>
        /// 플레이어가 의도치 않게 떨어지지 않도록 합니다.
        /// </summary>
        public bool CheckBoundaries()
        {
            bool ret = false;

            // 현재 플레이어의 위치를 기준으로 앞쪽과 조금 위의 위치를 계산
            Vector3 origin = transform.position + transform.forward * 0.5f + new Vector3(0, 0.5f, 0);

            float right = 0.25f;

            // 표면의 한계를 감지하기 위해 아래쪽으로 레이캐스트를 발사
            if (!controller.characterDetection.ThrowRayOnDirection(origin, Vector3.down, 1))
                ret = CheckSurfaceBoundary();
            // 추가로 오른쪽 방향에 대해서도 검사
            else if (!controller.characterDetection.ThrowRayOnDirection(origin + transform.right * right, Vector3.down, 1) && ret == false)
                ret = CheckSurfaceBoundary();
            // 추가로 왼쪽 방향에 대해서도 검사
            else if (!controller.characterDetection.ThrowRayOnDirection(origin + transform.right * -right, Vector3.down, 1) && ret == false)
                ret = CheckSurfaceBoundary();

            // 디버그 모드에서는 레이캐스트의 경로를 그려줍니다.
            if (showDebug)
            {
                Debug.DrawLine(origin, origin + Vector3.down * raycastDownDistance);
                Debug.DrawLine(origin, origin + Vector3.down * 1);
                Debug.DrawLine(origin + transform.right * right, origin + transform.right * right + Vector3.down * 1);
                Debug.DrawLine(origin + transform.right * -right, origin + transform.right * -right + Vector3.down * 1);
            }

            return ret;
        }

        /// <summary>
        /// 표면에서 떨어지지 않기 위한 속도를 계산합니다.
        /// </summary>
        private bool CheckSurfaceBoundary()
        {
            // 현재 플레이어 위치의 앞쪽과 조금 아래의 위치를 계산
            Vector3 origin2 = transform.position + transform.forward * 0.8f + new Vector3(0, -0.05f, 0);

            // 디버그 모드에서는 레이의 시작점과 끝점을 그려줍니다.
            if (showDebug)
                Debug.DrawLine(origin2, transform.position + new Vector3(0, -0.05f, 0));

            // 플레이어가 위치한 표면으로부터 아래 방향으로 레이캐스트를 발사
            RaycastHit hit1;
            if (controller.characterDetection.ThrowRayOnDirection(origin2, -transform.forward, 1, out hit1))
            {
                if (showDebug)
                    Debug.DrawLine(hit1.point, hit1.point + hit1.normal, Color.cyan);

                // 모서리 감지를 위해 추가 레이캐스트 발사
                RaycastHit hit2;
                RaycastHit hit3;
                controller.characterDetection.ThrowRayOnDirection(origin2 + transform.right * 0.05f, -transform.forward, 1, out hit2);
                controller.characterDetection.ThrowRayOnDirection(origin2 + transform.right * -0.05f, -transform.forward, 1, out hit3);

                // 각 레이캐스트의 노말 값을 조정
                if (hit2.normal == Vector3.zero)
                    hit2.normal = hit1.normal;
                if (hit3.normal == Vector3.zero)
                    hit3.normal = hit1.normal;

                // 현재 표면의 접선 벡터 계산
                Vector3 right = Vector3.Cross(Vector3.up, hit1.normal);
                // 속도 벡터를 접선에 투영
                velLimit = Vector3.Dot(velocity.normalized, right);

                // 모서리 감지를 위한 노말 체크
                if (hit1.normal != hit2.normal || hit1.normal != hit3.normal)
                    velLimit = 0;

                // 거의 수직인 방향으로 이동하려면 속도 제한
                if (velLimit < 0.4 && velLimit > -0.4)
                {
                    velLimit = 0;
                }

                // 속도 제한
                if (velLimit < -0.7)
                    velLimit = -0.7f;
                else if (velLimit > 0.7)
                    velLimit = 0.7f;

                // 새로운 속도 계산
                velocity = right * velLimit;

                return true;
            }

            return false;
        }

        public Vector3 GetVelocity() { 
            return rb.velocity; // 현재의 속도 반환
        }

        public void SetVelocity(Vector3 value)
        {
            velocity = value;
        }

        public void ResetSpeed()
        {
            smoothSpeed = 0;
        }

        public void SetCurrentState(MovementState state)
        {
            currentState = state;

            switch (currentState)
            {
                case MovementState.Walking:
                    curSpeed = walkSpeed;
                    break;
                case MovementState.Running:
                    curSpeed = RunSpeed;
                    smoothSpeed = curSpeed;
                    break;
            }
        }

        public MovementState GetState()
        {
            return currentState;
        }

        public void SetKinematic(bool active)
        {
            rb.isKinematic = active;
        }

        public void EnableFeetIK()
        {
            enableFeetIK = true;
            lastPelvisPositionY = 0;
            leftFootIKPosition = Vector3.zero;
            rightFootIKPosition = Vector3.zero;
        }
        public void DisableFeetIK()
        {
            enableFeetIK = true;
            lastPelvisPositionY = 0;
            leftFootIKPosition = Vector3.zero;
            rightFootIKPosition = Vector3.zero;
        }

        public void ApplyGravity()
        {
            rb.velocity += Vector3.up * -0.300f;
        }

        public void Fall()
        {
            controller.onAir = true; // 공중에 있음을 표시
            OnFall(); // 떨어짐 이벤트 호출
        }

        public void Landed()
        {
            OnLanded(); // 착지 이벤트 호출
            controller.isJumping = false;
            controller.onAir = false; // 공중에서 떨어짐 상태 해제
        }

        #endregion

        #region Foot IK

        // Animator의 IK (Inverse Kinematics) 로직을 처리하는 메서드
        private void OnAnimatorIK(int layerIndex)
        {
            // 발 IK 기능이 꺼져 있거나, 컨트롤러가 더미 상태이거나, 애니메이터가 null일 경우 반환
            if (!enableFeetIK || controller.dummy || anim == null)
                return;

            // 골반 높이 조절
            MovePelvisHeight();

            // 왼쪽 발 IK 위치 및 회전 설정
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, anim.GetFloat(leftFootAnim));
            MoveFeetToIKPoint(AvatarIKGoal.LeftFoot, leftFootIKPosition, leftFootIKRotation, ref lastLeftFootPosition);

            // 오른쪽 발 IK 위치 및 회전 설정
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, anim.GetFloat(rightFootAnim));
            MoveFeetToIKPoint(AvatarIKGoal.RightFoot, rightFootIKPosition, rightFootIKRotation, ref lastRightFootPosition);
        }

        // 발을 IK 포인트로 움직이는 함수
        void MoveFeetToIKPoint(AvatarIKGoal foot, Vector3 positionIKHolder, Quaternion rotationHolder, ref float lastFootPositionY)
        {
            Vector3 targetIKPosition = anim.GetIKPosition(foot);

            // IK 위치가 설정된 경우
            if (positionIKHolder != Vector3.zero)
            {
                // 위치 변환
                targetIKPosition = transform.InverseTransformPoint(targetIKPosition);
                positionIKHolder = transform.InverseTransformPoint(positionIKHolder);

                // Y 위치 보간
                float yVariable = Mathf.Lerp(lastFootPositionY, positionIKHolder.y, feetToIKPositionSpeed);
                lastFootPositionY = yVariable;
                targetIKPosition.y += yVariable;

                targetIKPosition = transform.TransformPoint(targetIKPosition);
            }

            anim.SetIKRotation(foot, rotationHolder);
            anim.SetIKPosition(foot, targetIKPosition);
        }

        // 골반 높이를 조절하는 함수
        private void MovePelvisHeight()
        {
            // 만약 발 IK 위치가 설정되지 않은 경우
            if (rightFootIKPosition == Vector3.zero || leftFootIKPosition == Vector3.zero || lastPelvisPositionY == 0)
            {
                lastPelvisPositionY = anim.bodyPosition.y;
                return;
            }

            // 각 발에서의 높이 차이 계산
            float leftOffsetPosition = leftFootIKPosition.y - transform.position.y;
            float rightOffsetPosition = rightFootIKPosition.y - transform.position.y;
            float totalOffset = leftOffsetPosition < rightOffsetPosition ? leftOffsetPosition : rightOffsetPosition;

            // 골반 위치 보간
            Vector3 newPelvisPosition = anim.bodyPosition + Vector3.up * totalOffset;
            newPelvisPosition.y = Mathf.Lerp(lastPelvisPositionY, newPelvisPosition.y, pelvisUpDownSpeed);
            anim.bodyPosition = newPelvisPosition;

            lastPelvisPositionY = anim.bodyPosition.y;
        }

        // 레이캐스트를 사용하여 발의 위치를 지상에서 찾는 함수
        private void FeetPositionSolver(Vector3 fromRaycastPosition, ref Vector3 feetIKPositions, ref Quaternion feetIKRotations)
        {
            RaycastHit feetHit;

            // 디버그 모드에서는 레이의 경로를 그려줍니다.
            if (showDebug)
                Debug.DrawLine(fromRaycastPosition, fromRaycastPosition + Vector3.down * (raycastDownDistance + heightFromGroundRaycast), Color.green);

            // 레이캐스트를 아래로 발사하여 지면 감지
            if (controller.characterDetection.ThrowRayOnDirection(fromRaycastPosition, Vector3.down, raycastDownDistance + heightFromGroundRaycast, out feetHit))
            {
                feetIKPositions = fromRaycastPosition;
                feetIKPositions.y = feetHit.point.y + pelvisOffset;
                feetIKRotations = Quaternion.FromToRotation(Vector3.up, feetHit.normal) * transform.rotation;
                return;
            }

            feetIKPositions = Vector3.zero;
        }

        // 지면과의 거리를 유지하며 발의 대상 위치를 조절하는 함수
        private void AdjustFeetTarget(ref Vector3 feetPositions, HumanBodyBones foot)
        {
            feetPositions = anim.GetBoneTransform(foot).position;
            feetPositions.y = transform.position.y + heightFromGroundRaycast;
        }


        #endregion

        #region Auto Step

        // 캐릭터가 자동으로 작은 장애물이나 계단을 오를 수 있도록 도와주는 메서드
        private void AutoStep()
        {
            // 캐릭터가 경사면에 있을 경우 반환 (경사면에서는 자동 스텝 기능을 사용하지 않음)
            if (controller.inSlope)
                return;

            // Raycast를 위한 초기 오프셋
            Vector3 offset = new Vector3(0, 0.01f, 0);
            RaycastHit hit;

            // 캐릭터 앞쪽으로 레이를 발사하여 장애물 감지
            if (controller.characterDetection.ThrowRayOnDirection(transform.position + offset, transform.forward, controller.slidingCapsuleCollider.radius + 0.1f, out hit))
            {
                // 장애물 위쪽으로 레이를 발사하여 스텝 가능한지 확인
                if (!controller.characterDetection.ThrowRayOnDirection(transform.position + offset + new Vector3(0, controller.stepHeight, 0), transform.forward, controller.slidingCapsuleCollider.radius + 0.2f, out hit))
                {
                    // 스텝 가능할 경우 캐릭터의 위치를 위로 이동
                    rb.position += new Vector3(0, controller.stepVelocity, 0);
                }
            }
            // 캐릭터의 왼쪽 앞쪽으로 레이를 발사하여 장애물 감지
            else if (controller.characterDetection.ThrowRayOnDirection(transform.position + offset, transform.TransformDirection(new Vector3(-1.5f, 0, 1)), controller.slidingCapsuleCollider.radius + 0.1f, out hit))
            {
                // 장애물 위쪽으로 레이를 발사하여 스텝 가능한지 확인
                if (!controller.characterDetection.ThrowRayOnDirection(transform.position + offset + new Vector3(0, controller.stepHeight, 0), transform.TransformDirection(new Vector3(-1.5f, 0, 1)), controller.slidingCapsuleCollider.radius + 0.2f, out hit))
                {
                    // 스텝 가능할 경우 캐릭터의 위치를 위로 이동
                    rb.position += new Vector3(0, controller.stepVelocity, 0);
                }
            }
            // 캐릭터의 오른쪽 앞쪽으로 레이를 발사하여 장애물 감지
            else if (controller.characterDetection.ThrowRayOnDirection(transform.position + offset, transform.TransformDirection(new Vector3(1.5f, 0, 1)), controller.slidingCapsuleCollider.radius + 0.1f, out hit))
            {
                // 장애물 위쪽으로 레이를 발사하여 스텝 가능한지 확인
                if (!controller.characterDetection.ThrowRayOnDirection(transform.position + offset + new Vector3(0, controller.stepHeight, 0), transform.TransformDirection(new Vector3(1.5f, 0, 1)), controller.slidingCapsuleCollider.radius + 0.2f, out hit))
                {
                    // 스텝 가능할 경우 캐릭터의 위치를 위로 이동
                    rb.position += new Vector3(0, controller.stepVelocity, 0);
                }
            }
        }


        #endregion 
    }

}