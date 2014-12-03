using UnityEngine;
using System.Collections;

public class NonHide : MonoSingleton<NonHide>
{
	protected override void Awake()
	{
		base.Awake();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}
}
