using UnityEngine;
using UnityEngine.Events;

#if XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit;
#endif

/// <summary>
/// A universal knob that can be rotated and mapped to a value [0,1].
/// Works in normal Unity projects (via mouse drag) and XR projects (via XR Grab).
/// </summary>
public class UniversalXRKnob : MonoBehaviour
{
    [Header("Knob Settings")]
    public Transform handle;             // phần mesh để xoay
    [Range(0f, 1f)] public float value = 0.5f;
    public float minAngle = -90f;
    public float maxAngle = 90f;
    public bool clamped = true;
    public float angleStep = 0f;         // nấc nhảy, 0 = mượt

    [System.Serializable]
    public class ValueEvent : UnityEvent<float> { }
    public ValueEvent onValueChanged;

    float currentAngle;

#if XR_INTERACTION_TOOLKIT
    XRBaseInteractor interactor;
    bool isGrabbed = false;
#endif

    void Start()
    {
        SetKnob(value);
    }

    void Update()
    {
#if XR_INTERACTION_TOOLKIT
        if (isGrabbed && interactor != null)
        {
            // Lấy forward vector của controller và chuyển thành góc
            Vector3 localForward = transform.InverseTransformDirection(interactor.transform.forward);
            float angle = Mathf.Atan2(localForward.x, localForward.z) * Mathf.Rad2Deg;
            ApplyRotation(angle);
        }
#else
        // Test trong Editor bằng chuột
        if (Input.GetMouseButton(0))
        {
            float delta = Input.GetAxis("Mouse X");
            value += delta * 0.01f;
            if (clamped) value = Mathf.Clamp01(value);
            SetKnob(value);
        }
#endif
    }

    void ApplyRotation(float rawAngle)
    {
        // Map raw angle thành value 0-1
        float mappedValue = Mathf.InverseLerp(minAngle, maxAngle, rawAngle);
        if (clamped) mappedValue = Mathf.Clamp01(mappedValue);
        SetKnob(mappedValue);
    }

    void SetKnob(float newValue)
    {
        value = newValue;
        currentAngle = Mathf.Lerp(minAngle, maxAngle, value);

        if (angleStep > 0)
        {
            currentAngle = Mathf.Round(currentAngle / angleStep) * angleStep;
            value = Mathf.InverseLerp(minAngle, maxAngle, currentAngle);
        }

        if (handle != null)
            handle.localEulerAngles = new Vector3(0, currentAngle, 0);

        onValueChanged.Invoke(value);
    }

#if XR_INTERACTION_TOOLKIT
    public void OnGrab(XRBaseInteractor interactor)
    {
        this.interactor = interactor;
        isGrabbed = true;
    }

    public void OnRelease(XRBaseInteractor interactor)
    {
        isGrabbed = false;
        this.interactor = null;
    }
#endif
}
