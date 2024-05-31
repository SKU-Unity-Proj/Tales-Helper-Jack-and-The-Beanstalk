using DiasGames.Abilities;
using UnityEngine;

public class DropFrame_Livingroom : MonoBehaviour
{
    private Rigidbody rigid;

    public bool isDrop = true;
    public float GroundedOffset;
    private float GroundedRadius = 0.1f;

    public GameObject player;
    private PushHavyObjectAbility pushAbility;

    public LayerMask groundLayer;

    private bool oneAction = false;

    void Start()
    {
        pushAbility = player.GetComponent<PushHavyObjectAbility>();
        rigid = GetComponent<Rigidbody>();
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
        Destroy(this.transform.parent.gameObject);
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
