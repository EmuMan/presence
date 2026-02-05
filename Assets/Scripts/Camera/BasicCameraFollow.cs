using UnityEngine;

public class BasicCameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 5.0f;

    void Start()
    {
        transform.position = target.position + offset;
        transform.LookAt(target);
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;
    }
}
