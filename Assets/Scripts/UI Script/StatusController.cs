using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{
    // 체력
    [SerializeField]
    private int hp;
    private int currentHp;

    // 스태미나
    [SerializeField]
    private int sp;
    private int currentSp;

    // 스태미나 증가량
    [SerializeField]
    private int spIncreaseSpeed;

    // 스태미나 재회복 딜레이
    [SerializeField]
    private int spRechargeTime;
    private int currentSpRechargeTime;

    // 스태미나 감소 여부
    private bool spUsed;

    // 방어력
    [SerializeField]
    private int dp;
    private int currentDp;

    // 배고픔
    [SerializeField]
    private int hungry;
    private int currenthungry;

    // 배고픔이 줄어드는 속도
    [SerializeField]
    private int hungryDecreaseTime;
    private int currentHungryDecreaseTime;

    // 목마름
    [SerializeField]
    private int thirsty;
    private int currentThirsty;

    // 목마름이 줄어드는 속도
    [SerializeField]
    private int thirstyDecreaseTime;
    private int currentThirstyDecreaseTime;

    // 만족도
    [SerializeField]
    private int satisfy;
    private int currentSatisfy;

    // 필요한 이미지
    [SerializeField]
    private Image[] images_Gauge;

    // 변수의 상수화
    private const int HP = 0, DP = 1, SP = 2, HUNGRY = 3, THiRSTY = 4, SATISFY = 5;

    // Start is called before the first frame update
    void Start()
    {
        currentHp = hp;
        currentDp = dp;
        currentSp = sp;
        currenthungry = hungry;
        currentSatisfy = satisfy;
        currentThirsty = thirsty;
    }

    // Update is called once per frame
    void Update()
    {
        Hungry();
        Thirsty();
        SPRechargeTime();
        SPRecover();
        GaugeUpdate();
    }

    private void SPRechargeTime() // SP 리차지 딜레이
    {
        if (spUsed)
        {
            if (currentSpRechargeTime < spRechargeTime)
                currentSpRechargeTime++;
            else
                spUsed = false;
        }
    }

    private void SPRecover() // SP 증가량만큼 회복
    {
        if (!spUsed && currentSp < sp)
        {
            currentSp += spIncreaseSpeed;
        }

    }

    private void Hungry() // 시간당 배고픔 감소
    {
        if (currenthungry > 0)
        {
            if (currentHungryDecreaseTime <= hungryDecreaseTime) // hungryDecreaseTime 만큼 지나서 1씩증가하다가 넘으면 
                currentHungryDecreaseTime++;
            else
            {
                currenthungry--; // 배고픔 1 감소
                currentHungryDecreaseTime = 0; // 초기화
            }
        }
        else
            Debug.Log("배고픔 수치가 0이 되었습니다.");
    }

    private void Thirsty() // 목마름 감소
    {
        if (currentThirsty > 0)
        {
            if (currentThirstyDecreaseTime <= thirstyDecreaseTime) // hungryDecreaseTime 만큼 지나서 1씩증가하다가 넘으면 
                currentThirstyDecreaseTime++;
            else
            {
                currentThirsty--; // 배고픔 1 감소
                currentThirstyDecreaseTime = 0; // 초기화
            }
        }
        else
            Debug.Log("목마름 수치가 0이 되었습니다.");
    }

    private void GaugeUpdate() // UI 게이지 업데이트
    {
        images_Gauge[HP].fillAmount = (float)currentHp / hp;
        images_Gauge[SP].fillAmount = (float)currentSp / sp;
        images_Gauge[DP].fillAmount = (float)currentDp / dp;
        images_Gauge[HUNGRY].fillAmount = (float)currenthungry / hungry;
        images_Gauge[THiRSTY].fillAmount = (float)currentThirsty / thirsty;
        images_Gauge[SATISFY].fillAmount = (float)currentSatisfy / satisfy;
    }

    public void IncreaseSP(int _count) // 아이템에 의한 체력 회복 
    {
        if (currentSp + _count < sp)
            currentSp += _count;
        else
            currentSp = sp;
    }

    public void IncreaseHP(int _count) // 아이템에 의한 체력 회복 
    {
        if (currentHp + _count < hp)
            currentHp += _count;
        else
            currentHp = hp;
    }

    public void DecreaseHP(int _count) // 체력 감소
    {
        currentHp -= _count;

        if (currentHp <= 0)
            Debug.Log("캐릭터의 체력이 0이 되었습니다.");
    }

    public void IncreaseDP(int _count) // 방어력 증가
    {
        if (currentDp + _count < dp)
            currentDp += _count;
        else
            currentDp = dp;
    }

    public void DecreaseDP(int _count) // 방어력 감소
    {
        currentDp -= _count;

        if (currentDp <= 0)
            Debug.Log("캐릭터의 방어력이 0이 되었습니다.");
    }

    public void IncreaseHungry(int _count) // 배고픔 수치 증가
    {
        if (currenthungry + _count < hungry)
            currenthungry += _count;
        else
            currenthungry = hungry;
    }

    public void DecreaseHungry(int _count) // 배고픔 수치 감소
    {
        if (currenthungry - _count < 0)
            currenthungry = 0;
        else
            currenthungry -= _count;
    }

    public void IncreaseThirsty(int _count) // 목마름 수치 증가
    {
        if (currentThirsty + _count < thirsty)
            currentThirsty += _count;
        else
            currentThirsty = thirsty;
    }

    public void DecreaseThirsty(int _count) // 목마름 수치 감소
    {
        if (currentThirsty - _count < 0)
            currentThirsty = 0;
        else
            currentThirsty -= _count;
    }

    public void DecreaseStamina(int _count) // 스태미나 감소
    {
        spUsed = true;
        currentSpRechargeTime = 0;

        if (currentSp - _count > 0)
            currentSp -= _count;
        else
            currentSp = 0;
    }

    public int GetCurrentSP() // GET SP
    {
        return currentSp;
    }

}
