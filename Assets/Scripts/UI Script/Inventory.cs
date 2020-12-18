using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static bool inventoryActivated = false;


    // 필요한 컴포넌트
    [SerializeField]
    private GameObject go_InventoryBase; // 인벤토리 베이스
    [SerializeField]
    private GameObject go_SlotsParents; // 슬롯들

    private SlotToolTip theSlotToolTip;

    public Slot[] GetSlots()
    {
        return slots;}

    [SerializeField] private Item[] items;

    public void LoadToInven(int _arrayNum, string _itemName, int _itemNum) // 배열 넘버, 아이템 이름, 아이템 갯수
    {
        for (int i = 0; i < items.Length; i++)
            if(items[i].itemName == _itemName)
                slots[_arrayNum].AddItem(items[i], _itemNum);
    }

    // 슬롯들
    private Slot[] slots; // 슬롯 패런츠에서 받아오는 슬롯을 저장하는 변수

    
    // Start is called before the first frame update
    void Start()
    {
        slots = go_SlotsParents.GetComponentsInChildren<Slot>(); // 슬롯들을 받아서 저장
        theSlotToolTip = FindObjectOfType<SlotToolTip>();
    }

    // Update is called once per frame
    void Update()
    {
        TryOpenInventory(); // I 키로 인벤토리 창 활성화
    }

    private void TryOpenInventory() // I 키로 인벤토리 창 활성화
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryActivated = !inventoryActivated; // 상태 반전

            if (inventoryActivated)
                OpenInventory();
            else
            {
                CloseInventory();
                theSlotToolTip.HideToolTip();
            }

        }
    }

    private void OpenInventory()
    {
        GameManager.isOpenInventory = true;
        go_InventoryBase.SetActive(true); // 인벤토리창 활성화
    }

    private void CloseInventory()
    {
        GameManager.isOpenInventory = false;
        go_InventoryBase.SetActive(false); // 인벤토리창 비활성화
    }

    public void AcquireItem(Item _item, int _count = 1) // 아이템 획득
    {
        if (Item.ItemType.Equipment != _item.itemType) // 아이템 타입이 장비가 아닐때 개수 증가
        {
            for (int i = 0; i < slots.Length; i++) 
            {
                if (slots[i].item != null) 
                {
                    if (slots[i].item.itemName == _item.itemName)
                    {
                        slots[i].SetSlotCount(_count); // 개수 증가
                        return;
                    }
                }
            }
        }


        for (int i = 0; i < slots.Length; i++) // 아이템이 장비타입일때 개수는 증가하지않음
        {
            if (slots[i].item == null) // 아이템이 없으면 아이템 추가
            {
                slots[i].AddItem(_item, _count);
                return;
            }
        }
    }
}
