using System;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollowTarget : MonoBehaviour, IPoolObserver
{
    // The target we are following
    [SerializeField]
    private List<Transform> targets = new List<Transform>();

	// The distance in the x-z plane to the target
	[SerializeField]
	private float distance = 10.0f;
	// the height we want the camera to be above the target
	[SerializeField]
	private float height = 5.0f;

	[SerializeField]
	private float rotationDamping;
	[SerializeField]
	private float heightDamping;

    private const float targetDistanceHeightModifier = .25f;
    private const float maxHeight = 30f;

	// Use this for initialization
	void Awake()
    {
        GameObject.FindGameObjectWithTag("Player Pool").GetComponent<ObjectPool>().RegisterObserver(this);
    }

	// Update is called once per frame
	void LateUpdate()
	{
		// Early out if we don't have a target
		if (targets.Count == 0)
        {
            return;
        }

        float eulerAnglesY = targets[0].eulerAngles.y;
        Vector3 centerPosition = GetCenterBetweenTargets();

		// Calculate the current rotation angles
		var wantedRotationAngle = eulerAnglesY;
		var wantedHeight = Mathf.Clamp(centerPosition.y + height + (targets[0].position - centerPosition).sqrMagnitude * targetDistanceHeightModifier, 1, maxHeight);

		var currentRotationAngle = transform.eulerAngles.y;
		var currentHeight = transform.position.y;

		// Damp the rotation around the y-axis
		currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

		// Damp the height
		currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

		// Convert the angle into a rotation
		var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		transform.position = centerPosition;
		transform.position -= currentRotation * Vector3.forward * distance;

		// Set the height of the camera
		transform.position = new Vector3(transform.position.x ,currentHeight , transform.position.z);

		// Always look at the target
		transform.LookAt(centerPosition);
	}

    public void OnPoolInsert(Transform objectTransform, int objectIndex)
    {
        targets.Remove(objectTransform);
    }

    public void OnPoolPop(Transform objectTransform)
    {
        targets.Add(objectTransform);
    }

    private Vector3 GetCenterBetweenTargets()
    {
        Vector3 center = Vector3.zero;
        for(int i=0; i<targets.Count; i++)
        {
            center += targets[i].position;
        }

        return center / targets.Count;
    }
}