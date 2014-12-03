using System;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	static public T Instance;

	protected virtual void Awake()
	{
		Instance = this as T;
	}
	protected virtual void OnDestroy()
	{
		Instance = null;
	}
}