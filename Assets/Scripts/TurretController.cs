using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TurretController : MonoBehaviour
{
    [Header("References")]
    public Transform pivot;         // Empty đặt ở chân đế
    public Transform gunModel;      // Model súng (child của pivot)

    [Header("Settings")]
    public bool lockYRotation = true;   // Chỉ xoay ngang
    public float minYaw = -60f;
    public float maxYaw = 60f;
    public float minPitch = -10f;
    public float maxPitch = 30f;

    private XRBaseInteractor interactor;

    private Quaternion startPivotRot;       // Rotation của pivot lúc grab
    private Quaternion startHandRot;        // Rotation của tay lúc grab

    public void OnGrab(XRBaseInteractor interactor)
    {
        this.interactor = interactor;
        startPivotRot = pivot.rotation;
        startHandRot = interactor.transform.rotation;
    }

    public void OnRelease(XRBaseInteractor interactor)
    {
        this.interactor = null;
    }

    void Update()
    {
        if (interactor != null)
        {
            // Tính delta rotation từ lúc grab đến bây giờ
            Quaternion currentHandRot = interactor.transform.rotation;
            Quaternion delta = currentHandRot * Quaternion.Inverse(startHandRot);

            // Áp delta vào pivot
            Quaternion targetRot = delta * startPivotRot;

            if (lockYRotation)
            {
                // Lấy Euler để clamp
                Vector3 euler = targetRot.eulerAngles;
                float yaw = Mathf.DeltaAngle(0, euler.y);
                float pitch = Mathf.DeltaAngle(0, euler.x);

                yaw = Mathf.Clamp(yaw, minYaw, maxYaw);
                pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

                pivot.rotation = Quaternion.Euler(pitch, yaw, 0);
            }
            else
            {
                pivot.rotation = targetRot;
            }
        }
    }
}
