﻿using System;
using UnityEngine;

public class BeltConfig : MonoBehaviour
{
	private const float Speed = 1.5f;
	public Quaternion target;
	public Transform endPoint;
	
	private void Update()
	{
		transform.rotation = Quaternion.Lerp(transform.rotation, target, Time.deltaTime * Speed);
	}

	private void OnTriggerStay(Collider other)
	{
		other.transform.position =
			Vector3.MoveTowards(other.transform.position, endPoint.position, Speed  * Time.deltaTime);
	}
	
}