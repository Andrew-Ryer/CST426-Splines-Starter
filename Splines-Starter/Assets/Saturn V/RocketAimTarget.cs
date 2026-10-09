using UnityEngine;


public class RocketAimTarget : MonoBehaviour
{
    public Transform rocketBase;
    public Transform cameraToFollow;
    public float rocketHeight = 115f;
    public float heightOffset = 0f;

    private void Update()
    {
        if (rocketBase == null || cameraToFollow == null) return;

        Vector3 targetPosition = rocketBase.position;
        targetPosition.y = Mathf.Clamp(cameraToFollow.position.y + heightOffset, rocketBase.position.y, rocketBase.position.y + rocketHeight);
        
        transform.position = targetPosition;
    }
}
