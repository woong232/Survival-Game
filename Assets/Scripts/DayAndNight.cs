using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayAndNight : MonoBehaviour
{
    [SerializeField] private float secondPerRealTimeSecond; // 게임 세계의 100초 = 현실세계의 1초

    [SerializeField] private float fogDensityCalc; // 증감량 비율

    [SerializeField] private float nightFogDensity; // 밤 상태의 Fog 밀도
    [SerializeField] private float dayFogDensity; // 낮 상태의 fog 밀도
    private float currentFogDensity; // 계산
    public float currentFogdensity;


    void Update()
    {
        transform.Rotate(Vector3.right, 0.1f * secondPerRealTimeSecond * Time.deltaTime); // 우측으로 1초당 secondPerRealTimeSecond 의 역수만큼
        currentFogdensity = RenderSettings.fogDensity;

        if (transform.eulerAngles.x >= 170)
            GameManager.isNight = true;
        else if (transform.eulerAngles.x >= 5)
            GameManager.isNight = false;

        if (GameManager.isNight)
        {
            if (currentFogDensity <= nightFogDensity)
            {
                currentFogDensity += 0.1f * fogDensityCalc * Time.deltaTime;
                RenderSettings.fogDensity = currentFogDensity;
            }
        }
        else
        {
            if (currentFogDensity >= dayFogDensity)
            {
                currentFogDensity -= 0.1f * fogDensityCalc * Time.deltaTime;
                RenderSettings.fogDensity = currentFogDensity;
            }
        }
    }
}
