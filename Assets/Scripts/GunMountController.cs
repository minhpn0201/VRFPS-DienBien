using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GunMountController : MonoBehaviour
{
    [Header("References")]
    public Transform yawPivot;   // trục xoay ngang (thường là chỗ gắn với đế)
    public Transform pitchPivot; // trục xoay dọc (nòng súng)

    [Header("Pitch Settings")]
    public float minPitch = -10f; // hạ xuống tối đa
    public float maxPitch = 45f;  // ngửa lên tối đa

    [Header("Sensitivity")]
    public float rotationSpeed = 100f;

    private XRGrabInteractable grabInteractable;
    private Vector3 lastHandPosition;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrab);
            grabInteractable.selectExited.AddListener(OnRelease);
        }
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        lastHandPosition = args.interactorObject.transform.position;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // reset nếu cần
    }

    void Update()
    {
        if (grabInteractable != null && grabInteractable.isSelected)
        {
            // Lấy vị trí tay cầm hiện tại
            Transform hand = grabInteractable.interactorsSelecting[0].transform;
            Vector3 handDelta = hand.position - lastHandPosition;

            // Xoay quanh trục Y (yaw)
            float yaw = handDelta.x * rotationSpeed * Time.deltaTime;
            yawPivot.Rotate(Vector3.up, yaw, Space.World);

            // Xoay quanh trục X (pitch) có giới hạn
            float pitch = -handDelta.y * rotationSpeed * Time.deltaTime;
            pitchPivot.Rotate(Vector3.right, pitch, Space.Self);

            // Clamp pitch
            Vector3 euler = pitchPivot.localEulerAngles;
            if (euler.x > 180) euler.x -= 360; // đổi về [-180,180]
            euler.x = Mathf.Clamp(euler.x, minPitch, maxPitch);
            pitchPivot.localEulerAngles = new Vector3(euler.x, 0, 0);

            lastHandPosition = hand.position;
        }
    }
}
