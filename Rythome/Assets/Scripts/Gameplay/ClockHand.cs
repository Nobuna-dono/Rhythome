using UnityEngine;

public class ClockHand : MonoBehaviour
{
	[SerializeField, Tooltip("In seconds")]
	private float TickFrequency = 1f;
	[Tooltip("1 = 60 bpm, 0.5 = 120 bpm")]
	public float RatioBPM = 1f;	// TODO remove, use service
	[Tooltip("Angle to increment at each tick (6° for minutes, 30° for hours)")]
	public float Angle = 1f;
	public Vector2 PivotOffset = Vector2.zero;

	private Vector3 Pivot = Vector3.zero;
	private float timer = 0f;

	private void Awake()
	{
		Pivot = transform.position + new Vector3(PivotOffset.x, PivotOffset.y, 0f);
	}

	private void Update()
	{
		timer += Time.deltaTime;

		while (timer >= TickFrequency * RatioBPM)
		{
			transform.RotateAround(Pivot, Vector3.back, Angle);
			timer -= TickFrequency * RatioBPM;
		}
	}
}
