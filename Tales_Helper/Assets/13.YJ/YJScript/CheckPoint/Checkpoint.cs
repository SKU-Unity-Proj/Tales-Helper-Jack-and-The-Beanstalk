using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int episodeNum; // 체크포인트 번호

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CheckpointManager checkpointManager = GetComponentInParent<CheckpointManager>();

            if(checkpointManager != null)
            {
                checkpointManager.ClearEpisodeUpdate(episodeNum);
            }

            this.gameObject.SetActive(false);
        }
    }
}
