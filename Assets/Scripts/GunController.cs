using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class GunController : MonoBehaviour
{
    // 활성화 여부
    public static bool isActivate = false;

    //현재 장착된 총
    [SerializeField]
    private Gun currentGun;

    // 1초에 1씩 감소 0이되면 발사 / 연사속도 계산
    private float currentFireRate;


    // 상태 변수
    private bool isReload = false;
    [HideInInspector]
    public bool isFineSightMode = false;

    //본래 포지션 값
    [SerializeField]
    private Vector3 originPos;

    private AudioSource audioSource; // mp3플레이어 효과음 재생

    // 레이저 충돌정보
    private RaycastHit hitInfo;
    [SerializeField] private LayerMask layerMask;

    // 필요한 컴포넌트
    [SerializeField]
    private Camera theCam; // 카메라 정중앙에 발사
    private Crosshair theCrosshair;
    private PlayerController thePlayerController;

    //피격 이펙트
    [SerializeField]
    private GameObject hit_effect_prefab;


    void Start()
    {
        originPos = Vector3.zero;
        audioSource = GetComponent<AudioSource>();
        theCrosshair = FindObjectOfType<Crosshair>();
        thePlayerController = FindObjectOfType<PlayerController>();


    }

    // Update is called once per frame
    void Update()
    {
        if (!Inventory.inventoryActivated && !CraftManual.isActivated)
        {
            if (isActivate)
            {
                GunFireRateCalc();
                TryFire();
                TryReload();
                TryFineSight();
            }
        }
    }

    private void GunFireRateCalc() // 연사속도 계산
    {
        if (currentFireRate > 0)
            currentFireRate -= Time.deltaTime; // 60분의 1 = 1 / 1초에 1씩 감소 (60프레임이 거의 1초)
    }

    private void TryFire() // 발사 시도
    {
        if (Input.GetButton("Fire1") && currentFireRate <= 0 && !isReload && !thePlayerController.Runcheck()) // 연사속도 가 0이되면 방아쇠 당김 시도
        {
            Fire();
        }
    }

    private void Fire() // 발사 전 계산
    {
        if (!isReload)
        {
            if (currentGun.currentBulletCount > 0)
                Shoot(); // 실질적인 발사
            else
            {
                CancelFineSight();
                StartCoroutine(ReloadCoroutine());
            }
        }
    }

    private void Shoot() // 발사후 계산
    {
        theCrosshair.FireAnimation();
        currentGun.currentBulletCount--; // 총알 감소
        currentFireRate = currentGun.fireRate; // 연사속도 재계산
        PlaySE(currentGun.fire_Sound); // 발사 사운드
        currentGun.muzzleFlash.Play(); // 총구 섬광 
        Hit();
        StartCoroutine(RetroActionCoroutine()); // 반동
    }

    private void Hit()
    {
        if (Physics.Raycast(theCam.transform.position, theCam.transform.forward + 
            new Vector3(Random.Range(-theCrosshair.GetAccuracy() - currentGun.accuracy, theCrosshair.GetAccuracy() + currentGun.accuracy), 
                        Random.Range(-theCrosshair.GetAccuracy() - currentGun.accuracy, theCrosshair.GetAccuracy() + currentGun.accuracy),
                        0)
            , out hitInfo, currentGun.range, layerMask))
        {
            //                             프리펩 생성         맞은곳에           각도 설정 (바라본곳에 앞으로)
            GameObject clone = Instantiate(hit_effect_prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(clone, 2f); // 객체에 넣어 2초뒤에 제거 (메모리 관리)
        }
    }

    private void TryReload() // 재장전 시도
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReload && currentGun.currentBulletCount < currentGun.reloadBulletCount)
        {
            CancelFineSight();
            StartCoroutine(ReloadCoroutine());
        }
    }

    public void CancelReload()
    {
        if (isReload)
        {
             StopAllCoroutines();
             isReload = false;
        }
    }

     // 재장전 
    IEnumerator ReloadCoroutine()
    {
        if (currentGun.carryBulletCount > 0)
        {
            isReload = true;
            currentGun.anim.SetTrigger("Reload");

            currentGun.carryBulletCount += currentGun.currentBulletCount;
            currentGun.currentBulletCount = 0;

            yield return new WaitForSeconds(currentGun.reloadTime);

            if (currentGun.carryBulletCount >= currentGun.reloadBulletCount)
            {
                currentGun.currentBulletCount = currentGun.reloadBulletCount;
                currentGun.carryBulletCount -= currentGun.reloadBulletCount;
            }
            else
            {
                currentGun.currentBulletCount = currentGun.carryBulletCount;
                currentGun.carryBulletCount = 0;
            }


            isReload = false;
        }
        else
        {
            Debug.Log("총알이 없습니다.");
        }
    }

    private void TryFineSight() // 정조준 시도
    {
        if (Input.GetButtonDown("Fire2") && !isReload) // 우클릭
        {
            FineSight();
        }
    }

    public void CancelFineSight() // 정조준 취소 (정조준시 리로드 할경우 취소)
    {
        if(isFineSightMode)
            FineSight();
    }

    private void FineSight() // 정조준 로직
    {
        isFineSightMode = !isFineSightMode; // 스위치
        currentGun.anim.SetBool("FineSightMode", isFineSightMode); // 애니메이션
        theCrosshair.FineSightAnimation(isFineSightMode);

        if (isFineSightMode)
        {
            StopAllCoroutines();
            StartCoroutine(FineSightActivateCoroutine()); // 우클릭시
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(FineSightDeactivateCoroutine()); // 한번더 우클릭시
        }
    }

    IEnumerator FineSightActivateCoroutine() // 정조준 코루틴
    {
        while (currentGun.transform.localPosition != currentGun.fineSightOriginPos) // 정조준 포지션값
        {
            currentGun.transform.localPosition =
                Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.2f); //러프 
            yield return null;
        }
    }

    IEnumerator FineSightDeactivateCoroutine() // 원래 위치로
    {
        while (currentGun.transform.localPosition != originPos) // 원래 포지션값
        {
            currentGun.transform.localPosition =
                Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.2f);
            yield return null;
        }
    }

    IEnumerator RetroActionCoroutine() // 반동코루틴
    {
        Vector3 recoilBack = new Vector3(currentGun.retroActionForce, originPos.y, originPos.z); // 안했을때 최대반동
        Vector3 retroActionRecoilBack = 
            new Vector3(currentGun.retroActionFineSightForce, currentGun.fineSightOriginPos.y, currentGun.fineSightOriginPos.z); //정조준 했을때 최대반동
        

        if (!isFineSightMode) // 정조준 아닐때
        {
            currentGun.transform.localPosition = originPos; // 두번째 발사부터 반동이 중복되기 때문에 처음위치로 돌려 연출

            
            // 반동 시작
            while (currentGun.transform.localPosition.x <= currentGun.retroActionForce - 0.02f) // 반동까지 여유 (러프때문)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }


            //원위치
            while (currentGun.transform.localPosition != originPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.1f);
                yield return null;
            }

        }
        else // 정조준 일때
        {
            currentGun.transform.localPosition = currentGun.fineSightOriginPos;

            // 반동 시작
            while (currentGun.transform.localPosition.x <= currentGun.retroActionFineSightForce - 0.02f) // 반동까지 여유 (러프때문)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, retroActionRecoilBack, 0.4f);
                yield return null;
            }
            // 원위치
            while (currentGun.transform.localPosition != currentGun.fineSightOriginPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.1f);
                yield return null;
            }
        }
        
    }

    private void PlaySE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }

    public Gun GetGun()
    {
        return currentGun;
    }

    public bool GetFineSightMode()
    {
        return isFineSightMode;
    }

    public void GunChange(Gun _gun)
    {
        if (WeaponManager.currentWeapon != null)
            WeaponManager.currentWeapon.gameObject.SetActive(false); // 원래 들고있던총을 비활성화

        currentGun = _gun;
        WeaponManager.currentWeapon = currentGun.GetComponent<Transform>();
        WeaponManager.CurrentWeaponAnimator = currentGun.anim;


        currentGun.transform.localPosition = Vector3.zero;
        currentGun.gameObject.SetActive(true);
        isActivate = true;
    }

}
