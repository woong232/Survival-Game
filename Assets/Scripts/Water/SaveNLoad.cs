using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPos;
    public Vector3 playerRot;

    public List<int> invenArrayNum = new List<int>();
    public List<string> invenItemName = new List<string>(); // 슬롯은 직렬화 불가
    public List<int> invenItemNum = new List<int>();

}

public class SaveNLoad : MonoBehaviour
{
    private SaveData saveData = new SaveData();

    private string SAVE_DATA_DIRECTORY;
    private string SAVE_FILENAME = "/SaveFile.txt";

    private PlayerController theplayer;
    private Inventory theInven;

    // Start is called before the first frame update
    void Start()
    {
        SAVE_DATA_DIRECTORY = Application.dataPath + "/Saves/"; // 현재 프로젝트 폴더 + /Saves/

        if (!Directory.Exists(SAVE_DATA_DIRECTORY)) // 디렉토리가없으면 폴더 생성
            Directory.CreateDirectory(SAVE_DATA_DIRECTORY);
    }

    public void SaveData() // json 이용
    {
        theplayer = FindObjectOfType<PlayerController>();
        theInven = FindObjectOfType<Inventory>();

        saveData.playerPos = theplayer.transform.position; // 위치저장
        saveData.playerRot = theplayer.transform.eulerAngles; // 바라보는 방향 저장

        Slot[] slots = theInven.GetSlots(); // 인벤토리 아이템 저장
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                saveData.invenArrayNum.Add(i);
                saveData.invenItemName.Add(slots[i].item.itemName);
                saveData.invenItemNum.Add(slots[i].itemCount);
            }
        }

        string json = JsonUtility.ToJson(saveData); // 저장된 정보를 json 화

        File.WriteAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME, json); // 경로에 이름 붙여서 json 에있는 정보 쓰기

        Debug.Log("저장 완료");
        Debug.Log(json);
    }

    public void LoadData() // json -> savedata
    {
        if (File.Exists(SAVE_DATA_DIRECTORY + SAVE_FILENAME))
        {
            string loadJson = File.ReadAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME);
            saveData = JsonUtility.FromJson<SaveData>(loadJson);

            theplayer = FindObjectOfType<PlayerController>();
            theInven = FindObjectOfType<Inventory>();

            theplayer.transform.position = saveData.playerPos;
            theplayer.transform.eulerAngles = saveData.playerRot;

            for (int i = 0; i < saveData.invenItemName.Count; i++)
            {
                theInven.LoadToInven(saveData.invenArrayNum[i], saveData.invenItemName[i], saveData.invenItemNum[i]);
            }

            Debug.Log("로드 완료");
        }
        else
            Debug.Log("세이브 파일이 없습니다.");
    }
}
