using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    // 충돌한 오브젝트의 컬라이더
    private List<Collider> colliderList = new List<Collider>();

    [SerializeField] private int layerGround; // 지상 레이어
    private const int IGNORE_RAYCAST_LAYER = 2;

    [SerializeField] private Material green;
    [SerializeField] private Material red;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ChangeColor();
    }

    private void ChangeColor()
    {
        if (colliderList.Count > 0)
            SetColor(red);
        else
            SetColor(green);
    }

    private void SetColor(Material mat)
    {
        foreach (Transform tf_Child in this.transform) // 자식개체의 트랜스폼 갯수만큼
        {
            var newMaterials = new Material[tf_Child.GetComponent<Renderer>().materials.Length]; // 기존 메테리얼

            for (int i = 0; i < newMaterials.Length; i++) // 메테리얼의 렝스만큼
            {
                newMaterials[i] = mat; // 색깔
            }

            tf_Child.GetComponent<Renderer>().materials = newMaterials; // 색 바꾸기
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != layerGround && other.gameObject.layer != IGNORE_RAYCAST_LAYER)
            colliderList.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != layerGround && other.gameObject.layer != IGNORE_RAYCAST_LAYER)
            colliderList.Remove(other);
    }

    public bool IsBuildable()
    {
        return colliderList.Count == 0;
    }

}
