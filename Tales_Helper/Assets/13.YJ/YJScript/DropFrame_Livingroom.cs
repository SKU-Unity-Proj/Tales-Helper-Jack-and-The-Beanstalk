using DiasGames.Abilities;
using UnityEngine;

public class DropFrame_Livingroom : MonoBehaviour
{
    private Rigidbody rigid;
    public GameObject player;
    private PushHavyObjectAbility pushAbility;
    public GameObject swapCamArea;

    public bool isDrop = true;
    public float GroundedOffset;
    private float GroundedRadius = 0.1f;
    public LayerMask groundLayer;

    private bool oneAction = false;

    void Awake()
    {
        pushAbility = player.GetComponent<PushHavyObjectAbility>();
        rigid = GetComponent<Rigidbody>();

        swapCamArea.SetActive(false);
    }

    void Update()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - GroundedOffset);
        isDrop = Physics.CheckSphere(spherePosition, GroundedRadius, 1, QueryTriggerInteraction.Ignore);

        if (!isDrop && !oneAction)
        {
            pushAbility.StopAbilityFunction();
            oneAction = true;
            Invoke("ThisObjDestroy", 2f);
        }

        if (rigid.isKinematic && oneAction)
        {
            rigid.isKinematic = false;
        }
    }

    private void ThisObjDestroy()
    {
        // 막아놓은 벽 없애기
        transform.parent.GetComponent<BoxCollider>().enabled = false;

        // 카메라 전환
        swapCamArea.SetActive(true);

        Destroy(this.transform.gameObject,1f);
    }

    /*
    private void OnDrawGizmos()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - GroundedOffset);

        Gizmos.color = isDrop ? Color.green : Color.red;

        Gizmos.DrawWireSphere(spherePosition, GroundedRadius);
    }
    */
}
