using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TurretHandleController : MonoBehaviour
{
    [Header("Pivots")]
    public Transform yawPivot;    // gán yawPivot (world Y)
    public Transform pitchPivot;  // gán pitchPivot (local X)

    [Header("Settings")]
    public float rotationSpeed = 80f; // điều chỉnh sensitivity
    public float minPitch = -10f;
    public float maxPitch = 45f;

    // internal
    private XRGrabInteractable grab;
    private bool isGrabbed = false;
    private Transform interactorTransform;
    private Vector3 lastInteractorPos;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        if (grab != null)
        {
            grab.selectEntered.AddListener(OnGrab);
            grab.selectExited.AddListener(OnRelease);
        }
    }

    void OnDisable()
    {
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnGrab);
            grab.selectExited.RemoveListener(OnRelease);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        interactorTransform = args.interactorObject.transform;
        lastInteractorPos = interactorTransform.position;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        interactorTransform = null;
    }

    void Update()
    {
        if (!isGrabbed || interactorTransform == null) return;

        // delta tay giữa frame
        Vector3 delta = interactorTransform.position - lastInteractorPos;

        // yaw theo delta.x (ngang)
        float yaw = delta.x * rotationSpeed * Time.deltaTime;
        yawPivot.Rotate(Vector3.up, yaw, Space.World);

        // pitch theo delta.y (lên/xuống) - dấu âm để phù hợp tay kéo lên->ngửa lên
        float pitchDelta = -delta.y * rotationSpeed * Time.deltaTime;
        pitchPivot.Rotate(Vector3.right, pitchDelta, Space.Self);

        // clamp pitchPivot.localEulerAngles.x into [-180,180] then clamp
        Vector3 e = pitchPivot.localEulerAngles;
        if (e.x > 180f) e.x -= 360f;
        e.x = Mathf.Clamp(e.x, minPitch, maxPitch);
        pitchPivot.localEulerAngles = new Vector3(e.x, 0f, 0f);

        lastInteractorPos = interactorTransform.position;
    }
}
