using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RoomData
{
    public string roomName;
    public Vector3 playerStartPosition;
    public string completionCondition; // �Ϸ� ���� (��: "killAllEnemies", "collectItem")
}

[CreateAssetMenu(fileName = "RoomDataList", menuName = "ScriptableObjects/RoomDataList", order = 1)]
public class RoomDataList : ScriptableObject
{
    public List<RoomData> rooms;
}
