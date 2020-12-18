using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    private float range; // 습득 가능한 최대 거리

    private bool pickupActivated = false; // 습득 가능할 시 true

    private RaycastHit hitinfo; // 충돌체 정보 저장

    [SerializeField]
    private LayerMask layerMask; // 아이템 레이어만 반응

    // 필요한 컴포넌트
    [SerializeField]
    private Text actionText; // 아이템에 따라 보여질 텍스트
    [SerializeField]
    private Inventory theInventory;


    // Update is called once per frame
    void Update()
    {
        CheckItem(); 
        TryAction();
    }

    private void TryAction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckItem();
            CanPickUp();
        }
    }

    private void CheckItem() // 아이템이 있는지 체크
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitinfo, range, layerMask))
        {
            if (hitinfo.transform.tag == "Item")
            {
                ItemInfoAppear();
            }
        }
        else
            InfoDisappear();
    }


    private void CanPickUp() // 아이템 획득
    {
        if (pickupActivated)
        {
            if (hitinfo.transform != null)
            {
                Debug.Log(hitinfo.transform.GetComponent<ItemPickUp>().item.itemName + "을(를) 획득했습니다.");
                theInventory.AcquireItem(hitinfo.transform.GetComponent<ItemPickUp>().item); // 인벤토리 추가
                Destroy(hitinfo.transform.gameObject); // 획득한 아이템 파괴
                InfoDisappear(); // 아이템 정보 비활성화
            }
        }
    }


    private void ItemInfoAppear()
    {
        pickupActivated = true; // 픽업이 가능한 상태
        actionText.gameObject.SetActive(true); // 텍스트 활성화
        actionText.text =  hitinfo.transform.GetComponent<ItemPickUp>().item.itemName + " 획득 " + "<color=yellow>"+ "(E)" + "</color>"; // 텍스트 띄움
    }

    private void InfoDisappear() // 텍스트 비활성화
    {
        pickupActivated = false;
        actionText.gameObject.SetActive(false);
    }
}
