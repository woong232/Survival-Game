using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 스피드 조정 변수
    [SerializeField] private float walkSpeed; // SerializeField 는 private 라도 인스펙터창에 띄워줌 private는 다른 스크립트에서 참조 불가능
    [SerializeField] private float runSpeed;
    [SerializeField] private float crouchSpeed; // 앉기 속도

    [SerializeField] private float swimSpeed;
    [SerializeField] private float swimFastSpeed;
    [SerializeField] private float upSwimSpeed;

                     private float applySpeed; // 걷기와 뛰기 스피드를 바꿔서 사용할때 

    [SerializeField] private float jumpForce; // 점프 힘

    //민감도
    [SerializeField] private float lookSensitivity;

    //상태 변수
    private bool isWalk = false;
    private bool isRun = false; // 뛰고 있는지 아닌지
    private bool isCrouch = false; // 앉아있는지 아닌지
    private bool isGround = true; //땅에 있는지 없는지

    // 움직임 체크 변수
    private Vector3 lastPos; // 전프레임과 이번 프레일을 비교해서 걷고있는지 체크하기위해

    //앉아을때 얼마나 앉을지 결정하는 변수
    [SerializeField]
    private float crouchPosY; // 얼마나 앉을지
    private float originPosY; // 원래 위치
    private float applyCrouchPosY; // 적용 변수

    private CapsuleCollider capsuleCollider; // 땅착지 여부를 판별하기 위한 컴포넌트  <- 캡슐 땅 -> 매쉬콜라이더

    //카메라 한계
    [SerializeField] private float cameraRotationLimit;
                     private float currentCameraRotationX = 0f;

    // 필요한 컴포넌트  
    [SerializeField]
    private Camera theCamera;
    private Rigidbody myRigid;
    private GunController theGunController;
    private Crosshair theCrosshair;
    private StatusController theStatusController;


    // 초기화 함수
    void Start()
    {
            capsuleCollider = GetComponent<CapsuleCollider>();
            myRigid = GetComponent<Rigidbody>();
            theGunController = FindObjectOfType<GunController>();
            theCrosshair = FindObjectOfType<Crosshair>();
            theStatusController = FindObjectOfType<StatusController>();

            //초기화
            applySpeed = walkSpeed;
            originPosY = theCamera.transform.localPosition.y; // 로컬 포지션인 이유 플레이어의 종속되어 있기 때문에 상대적인 값 조정이 필요함
            applyCrouchPosY = originPosY;
    }

    // 프레임마다 업데이트되는 함수
    void Update()
    {
        if (GameManager.canPlayerMove)
        {
            WaterCheck();
            IsGround();
            TryJump();
            if (!GameManager.isWater)
            {
                TryRun(); // 무브위에 설정해야 뛰기를 먼저 판별하고 스피드를 대입함
            }
            TryCrouch();
            Move();
            MoveCheck();
            CameraRotation();
            CharacterRotation();
        }
    }

    private void WaterCheck()
    {
        if (GameManager.isWater)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                applySpeed = swimFastSpeed;
            }
            else
            {
                applySpeed = swimSpeed;
            }
        }
    }

    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    private void Crouch() // 컨트롤 키 눌렀을 때 활성화
    {
        isCrouch = !isCrouch;  // 값 반전
        theCrosshair.CrouchingAnimation(isCrouch);

        if (isCrouch) // true 일때
        {
            applySpeed = crouchSpeed; // 앉기속도로 변환
            applyCrouchPosY = crouchPosY; // 카메라 Y값 조정
        }
        else
        {
            applySpeed = walkSpeed; // iscorouch가 false 가 됬을때 걷기 속도로 변환
            applyCrouchPosY = originPosY; // 원래 Y값
        }

        StartCoroutine(CrouchCoroutine());
    }

    IEnumerator CrouchCoroutine() // 앉기 모션 구현
    {
        float _posY = theCamera.transform.localPosition.y; // 카메라 상대적 위치값 대입
        int count = 0;

        while (_posY != applyCrouchPosY) //서서히 증가 // posY 가 조건과 동일하면 while문을 빠져나감
        {
            count++;
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.3f); // (1,2,0.5) -> 1과 2사이를 0.5만큼씩 증가
            theCamera.transform.localPosition = new Vector3(0, _posY, 0); //카메라 위치값 변환
            if (count > 15) // Lerp 함수는 완전히 0과 1이 되기 힘들기 때문에 count값을 지정해 빠져나오게 함
                break;
            yield return null; // 1프레임마다 실행 
        }
        theCamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0f); // 15번 실행후 목적지 도달 -> Lerp 함수의 단점 보완
    }

    private void IsGround() // 땅에있는지 여부 판별
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f); // 레이저를 쏘는것 (어디에서, 어디로, 얼마나)
                                                                                                              //캡슐위치, 땅쪽으로, 캡슐 y값의 반 -> 계단 대각선은 오차의 문제로 0.1f 여유
        theCrosshair.JumpingAnimation(!isGround);                                                                                                     
    }

    private void TryJump() // 스페이스바를 누르고 땅에 있다면 점프
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround && theStatusController.GetCurrentSP() > 0 && !GameManager.isWater)
            Jump();
        else if (Input.GetKey(KeyCode.Space) && GameManager.isWater)
            UpSwim();
    }

    private void UpSwim()
    {
        myRigid.velocity = transform.up * upSwimSpeed;
    }

    private void Jump()
    {
        if (isCrouch) // 앉았을때 점프시 점프와 함께 앉기 해제
            Crouch();
        theStatusController.DecreaseStamina(100); // 스태미나 -100
        myRigid.velocity = transform.up * jumpForce; // 속도를 위(0,1,0)로 바꿔 점프힘을 곱함
    }

    private void TryRun() // 왼쪽 쉬프트를 눌렀을때 런스피드 대입 / 땠을 때 워크스피드 대입 
    {
        if (Input.GetKey(KeyCode.LeftShift) && theStatusController.GetCurrentSP() > 0)
        {
            Running();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) || theStatusController.GetCurrentSP() <= 0)
        {
            RunningCancel();
        }
    }

    private void Running()
    {
        if (isCrouch) // 뛰고있을때 점프시 점프와 함께 뛰기 해제
            Crouch();

        theGunController.CancelFineSight();

        isRun = true;
        theCrosshair.RunningAnimation(isRun);
        theStatusController.DecreaseStamina(10);
        applySpeed = runSpeed;
    }

    private void RunningCancel()
    {
        isRun = false;
        theCrosshair.RunningAnimation(isRun);
        applySpeed = walkSpeed;
    }

    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal"); // 오른쪽이면 1 왼쪽이면 -1 반환
        float _moveDirZ = Input.GetAxisRaw("Vertical"); // 위면 1 아래면 -1 반환

        Vector3 _moveHorizontal = transform.right * _moveDirX; // (1,0,0) * (1/-1, 0, 0) 나아갈 방향 결정
        Vector3 _moveVertical = transform.forward * _moveDirZ; // (0,0,1) * (0, 0, 1/-1) 나아갈 방향 결정

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed; // 속도 결정 // normalized == 값을 (1,0,1) 에서 (0.5, 0, 0.5) 로 합쳐서 1로만들어줌

        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime); //실제로 움직임 구현 포지션 * 속도 * 델타타임(시간 균등화)
        // Time.deltaTime == 0.016 1/60
    }

    private void MoveCheck()
    {
        if (!isRun && !isCrouch && !isGround)
        {
            if (Vector3.Distance(lastPos,transform.position) >= 0.01f) // 마지막 위치값과 현재값의 거리 // why 경사면에서의 미세한움직임이 걷고있다고 하기엔 애매하므로
                isWalk = true;
            else
                isWalk = false;

            theCrosshair.WalkingAnimation(isWalk);
            lastPos = transform.position;
        }
  }

    private void CharacterRotation()
    {
        // 좌우 캐릭터 회전
        float _yRotation = Input.GetAxisRaw("Mouse X"); // 좌우 마우스 값 입력
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity; // Y의 벡터값 * 민감도
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY)); // MoveRotation은 쿼터니언으로 값을 받음 _characterRotationY -> 쿼터니언값으로 바꿔줌
    }

    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y"); // 마우스 위아래 
        float _cameraRotationX = _xRotation * lookSensitivity; // 카메라X값을 마우스 방향 * 민감도
        currentCameraRotationX -= _cameraRotationX; // 현재 카메라 값에 값을 더하거나 뺌
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit); // (현재 카메라 값, -45도, 45도 ) 값으로 현재 카메라 값을 가둬놓음

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f); // 각도값 (로테이션) 카메라 각도값을 변경 시킴
    }

    public bool Runcheck()
    {
        return isRun;
    }
}
