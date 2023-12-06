using UnityEngine;

public class Projectile : MonoBehaviour
{
	private	Vector3	moveDirection;
	private	float	moveSpeed = 5.0f;

	public void Setup(Vector3 direction)
	{
		moveDirection = direction;
	}

	private void Update()
	{
		transform.position += moveDirection * moveSpeed * Time.deltaTime;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if ( collision.tag.Contains("Enemy") )
		{
			Destroy(gameObject);
			Destroy(collision.gameObject);
		}
	}
}

