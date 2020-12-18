using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")] // 에셋 메뉴에 추가
public class Item : ScriptableObject // 게임 오브젝트에 스크립트를 붙여 주지 않아도댐
{
    public string itemName; // 아이템의 이름
    [TextArea]
    public string itemDesc; // 아이템의 설명
    public ItemType itemType; // 아이템의 유형
    public Sprite itemImage; // 아이템의 이미지 // Image - 캔버스필요,  Sprite - 월드 내에 어느곳에나 띄워줌
    public GameObject itemPrefab; // 아이템의 프리펩

    public string weaponType; // 무기 유형

    public enum ItemType
    {
        Equipment, // 장비
        Used, // 소모품
        Ingredient, // 재료
        ETC // 기타
    }



}
