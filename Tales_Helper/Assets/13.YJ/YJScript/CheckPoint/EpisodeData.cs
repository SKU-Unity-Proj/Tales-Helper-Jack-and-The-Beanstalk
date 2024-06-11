using UnityEngine;

[CreateAssetMenu(fileName = "EpisodeData", menuName = "ScriptableObjects/EpisodeData", order = 1)]
public class EpisodeData : ScriptableObject
{
    public int currentEpisodeNum;
    public GameObject[] episodeObjects;
    public string[] functionNames;
}