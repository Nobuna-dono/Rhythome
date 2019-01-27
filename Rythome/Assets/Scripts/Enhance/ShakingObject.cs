using UnityEngine;

public class ShakingObject : MonoBehaviour
{
	public float Speed = 1f;

	[SerializeField]
	private Vector2 ShakeIntensity = Vector2.one;
	private Vector3 origin = Vector3.zero;

	private void OnEnable()
	{
		origin = transform.position;
	}

	private void OnDisable()
	{
		transform.position = origin;
	}

	private void Update()
	{
		Vector2 offset = Mathf.Sin(Time.time * Speed) * ShakeIntensity;
		transform.position = origin + new Vector3(offset.x, offset.y, 0f);
	}
}
