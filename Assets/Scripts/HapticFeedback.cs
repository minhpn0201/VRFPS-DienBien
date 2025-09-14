// HapticFeedback.cs
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HapticFeedback : MonoBehaviour
{
    [Range(0f, 1f)] public float amplitude = 0.5f;
    public float duration = 0.1f;

    public void PlayHaptic(XRGrabInteractable grabbable)
    {
        foreach (var interactor in grabbable.interactorsSelecting)
        {
            if (interactor is XRBaseControllerInteractor controllerInteractor)
            {
                controllerInteractor.SendHapticImpulse(amplitude, duration);
            }
        }
    }
}
