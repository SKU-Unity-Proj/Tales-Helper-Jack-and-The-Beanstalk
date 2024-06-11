using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int episodeNum; // üũ����Ʈ ��ȣ

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
