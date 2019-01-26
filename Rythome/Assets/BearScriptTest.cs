using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearScriptTest : MonoBehaviour
{
	public Animator animator = null;

	private void Update()
	{
		if (animator)
			animator.SetBool("IsRunning", Input.GetKey(KeyCode.Z));
	}
}
