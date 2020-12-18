using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CloseWeaponController : MonoBehaviour
{   // 미완성 , 추상 클래스


    // 현재 장착된 Hand형 타입 무기
    [SerializeField]
    protected CloseWeapon currentCloseWeapon;

    // 공격중?
    protected bool isAttack = false; // 공격중인지
    protected bool isSwing = false; // 팔을 휘두르고 있는지

    protected RaycastHit hitInfo; // 닿은 오브젝트의 정보
    [SerializeField] protected LayerMask layerMask;




    protected void TryAttack()
    {
        if (!Inventory.inventoryActivated && !CraftManual.isActivated && !CraftManual.isPreviewActivated)
        {
            if (Input.GetButton("Fire1")) // 왼쪽버튼 누르면
            {
                if (!isAttack)
                {
                    StartCoroutine(AttackCoroutine());
                }
            }
        }
    }

    protected IEnumerator AttackCoroutine()
    {
        isAttack = true; // 중복 방지
        currentCloseWeapon.anim.SetTrigger("Attack"); // 공격 애니메이션

        yield return new WaitForSeconds(currentCloseWeapon.attackDelayA); // 딜레이
        isSwing = true; // 스윙 참

        StartCoroutine(HitCoroutine()); // isSwing 이 참일때 반복

        yield return new WaitForSeconds(currentCloseWeapon.attackDelayB); // 일정시간 딜레이
        isSwing = false; // 코루틴 반복문 빠져나옴

        yield return new WaitForSeconds(currentCloseWeapon.attackDelay - currentCloseWeapon.attackDelayA - currentCloseWeapon.attackDelayB); // A 딜레이와 B 딜레이 만큼 대기 하고 다음 클릭 딜레이주기위해 대기
        isAttack = false; // 다시 마우스 클릭을 할수 있게


    }

    // 미완성 = 추상 코루틴 자식클래스보고 완성시키라 함
    protected abstract IEnumerator HitCoroutine();


    protected bool CheckObject()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentCloseWeapon.range, layerMask)) // 캐릭터 좌표에서, 앞으로, 충돌한 오브젝트 정보 출력, 공격범위만큼
        {
            return true; // 있다면 트루
        }

        return false; // 아니면 펄스
    }

    // 완성 함수이지만, 추가 편집이 가능한 함수
    public virtual void CloseWeaponChange(CloseWeapon _CloseWeapon)
    {
        if (WeaponManager.currentWeapon != null)
            WeaponManager.currentWeapon.gameObject.SetActive(false); // 원래 들고있던총을 비활성화

        currentCloseWeapon = _CloseWeapon;
        WeaponManager.currentWeapon = currentCloseWeapon.GetComponent<Transform>();
        WeaponManager.CurrentWeaponAnimator = currentCloseWeapon.anim;


        currentCloseWeapon.transform.localPosition = Vector3.zero;
        currentCloseWeapon.gameObject.SetActive(true);
    }
}
