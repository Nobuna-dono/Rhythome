using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	[SerializeField]
	private Transform Target = null;
	private Vector3 Offset = Vector3.back * 10f;

	public float Speed = 1f;

	private void Awake()
	{
		if (!Target)
			return;

		Offset = transform.position - Target.position;
	}

	private void LateUpdate()
	{
		if (!Target)
			return;

		transform.position = Vector3.Lerp(transform.position, Target.position + Offset, Speed * Time.deltaTime);
	}
}
