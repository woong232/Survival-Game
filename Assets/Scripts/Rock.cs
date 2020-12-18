using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField]
    private int hp; // 바위의 체력

    [SerializeField]
    private float destoryTime; // 파편 제거 시간

    [SerializeField]
    private SphereCollider col; // 구체 컬라이더


    // 필요한 게임 오브젝트
    [SerializeField]
    private GameObject go_rock; // 일반 바위
    [SerializeField]
    private GameObject go_debris; // 깨진 바위
    [SerializeField]
    private GameObject go_effect_prefabs; // 채굴 이펙트
    [SerializeField]
    private GameObject go_rock_item_prefab; // 돌맹이 아이템

    // 돌맹이 아이템 등장 개수
    [SerializeField]
    private int maxCount;
    [SerializeField]
    private int minCount;


    // 필요한 사운드 이름
    [SerializeField] private string strike_Sound;
    [SerializeField] private string destroy_Sound;

    public void Mining() // 채굴
    {
        SoundManager.instance.PlaySE(strike_Sound); // 곡괭이가 부딪치는 소리

        var clone = Instantiate(go_effect_prefabs, col.bounds.center, Quaternion.identity); // 튀는 파편
        Destroy(clone, destoryTime); // 파편 제거

        hp--; // 바위의 체력 감소

        if (hp <= 0) // 0이 되면 바위 파괴
            Destruction();
    }

    private void Destruction() // 바위 파괴
    {
        SoundManager.instance.PlaySE(destroy_Sound); // 바위가 부서지는 소리

        col.enabled = false; // 바위 오브젝트 비활성화
        for (int i = 0; i < Mathf.Round(Random.Range(minCount,maxCount)); i++) // 랜덤 요소
        {
            Instantiate(go_rock_item_prefab, go_rock.transform.position, Quaternion.identity); // 아이템 생성
        }
        
        Destroy(go_rock); // 바위 파괴
        
        go_debris.SetActive(true); // 갈라진 파편 생성
        Destroy(go_debris, destoryTime); // 일정 시간뒤 파편 제거
    }
}
