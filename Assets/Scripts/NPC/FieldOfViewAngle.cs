using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewAngle : MonoBehaviour
{
    [SerializeField] private float viewAngle; // 시야각 (120)도
    [SerializeField] private float viewDistance; // 시야거리 (10미터)
    [SerializeField] private LayerMask targetMask; // 타겟 마스크(플레이어)

    private Pig thePig;

    void Start()
    {
        thePig = GetComponent<Pig>();
    }

    // Start is called before the first frame update
    void Update()
    {
        View();
    }

    private Vector3 BoundaryAngle(float _angle)
    {
        _angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad)); // P점(Player)의 x,z 좌표값 구하는 공식
    }

    private void View()
    {
        Vector3 _leftBoundary = BoundaryAngle(-viewAngle * 0.5f);
        Vector3 _rightBoundary = BoundaryAngle(viewAngle * 0.5f);

        Debug.DrawRay(transform.position + transform.up, _leftBoundary, Color.red);
        Debug.DrawRay(transform.position + transform.up, _rightBoundary, Color.red);

        Collider[] _target = Physics.OverlapSphere(transform.position, viewDistance, targetMask); // 일정 반경안에 있는 물체를 저장

        for (int i = 0; i < _target.Length; i++)
        {
            Transform _targetTf = _target[i].transform;
            if (_targetTf.name == "Player") // 하이라키에 있는 객체 이름
            {
                Vector3 _direction = (_targetTf.position - transform.position).normalized; // 방향을 알아내는 / 거리는 빼줌 normalized
                float _angle = Vector3.Angle(_direction, transform.forward); // 돼지와 플레이어의 각도 차이

                if (_angle < viewAngle * 0.5f) // 시야각 내에 있는지
                {
                    RaycastHit _hit;
                    if (Physics.Raycast(transform.position + transform.up, _direction, out _hit, viewDistance)) 
                    {
                        if (_hit.transform.name == "Player") // 돼지부터 레이저를쏴서 플레이어가 맞는다면
                        {
                            Debug.Log("플레이어가 돼지 시야 내에 있습니다");
                            Debug.DrawRay(transform.position + transform.up, _direction, Color.blue);
                            thePig.Run(_hit.transform.position);
                        }
                    }
                }
            }
        }
    }
}
