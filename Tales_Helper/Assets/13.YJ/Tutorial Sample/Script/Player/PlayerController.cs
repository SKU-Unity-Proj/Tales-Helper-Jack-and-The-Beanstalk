using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField]
	private	KeyCode		keyCodeFire = KeyCode.Space;
	[SerializeField]
	private	GameObject	projectilePrefab;
	private	float		moveSpeed = 3;
	private	Vector3		lastMoveDirection = Vector3.right;

	public	bool		IsMoved		{ set; get; } = false;	// 이동 가능 여부
	public	bool		IsAttacked	{ set; get; } = false;	// 공격 가능 여부

	private void Update()
	{
		if ( IsMoved == true )
		{
			// 플레이어 오브젝트 이동
			float x = Input.GetAxisRaw("Horizontal");
			float y = Input.GetAxisRaw("Vertical");

			transform.position += new Vector3(x, y, 0) * moveSpeed * Time.deltaTime;

			// 마지막에 입력된 방향키의 방향을 총알의 발사 방향으로 활용
			if ( x != 0 || y != 0 )
			{
				lastMoveDirection = new Vector3(x, y, 0);
			}
		}

		if ( IsAttacked == true )
		{
			// 스페이스 키를 눌러 발사체 생성
			if ( Input.GetKeyDown(keyCodeFire) )
			{
				GameObject clone = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
				clone.GetComponent<Projectile>().Setup(lastMoveDirection);
			}
		}
	}
}

