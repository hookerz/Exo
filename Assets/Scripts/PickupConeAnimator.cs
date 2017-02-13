using UnityEngine;
using System.Collections.Generic;

public class PickupConeAnimator : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public ScaleInOutNonUniform animator;
    public float animationTime = 0.3f;
    private float targetTime = 0.0f;
    private float target = 0.0f;
    private float current = 0.0f;

    private bool allowed = true;
    [HideInInspector]
    public bool on;

    public LayerMask collisionMask;

    private List<Collider> activeColliders = new List<Collider>();

    void OnTriggerEnter(Collider collider)
    {
        int collisionColliderLayer = 1 << collider.gameObject.layer;
        if ((collisionColliderLayer & collisionMask.value) == 0)
            return;

        if (!activeColliders.Contains(collider))
        {
            if (activeColliders.Count == 0 && allowed)
                SetTargetScannerState(true);
            activeColliders.Add(collider);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        int collisionColliderLayer = 1 << collider.gameObject.layer;
            if ((collisionColliderLayer & collisionMask.value) == 0)
                return;

        if (activeColliders.Contains(collider))
        {
            activeColliders.Remove(collider);
        }

        if (activeColliders.Count == 0) {
            SetTargetScannerState(false);
        }
    }

    private void SetTargetScannerState(bool on)
    {
        this.on = on;
        float t = on ? 1.0f : 0.0f;
        if (t != target)
        {
            meshRenderer.enabled = true;
            target = t;
            targetTime = Time.time;
        }
    }

    public void SetScannerAllowed(bool allowed)
    {
        if (this.allowed == allowed)
            return;

        bool scannableInVolume = activeColliders.Count != 0;

        if (allowed)
        {
            if (scannableInVolume)
            {
                current = 0.0f;
                SetTargetScannerState(true);
            }
        }
        else
        {
            SetTargetScannerState(false);
        }

        this.allowed = allowed;
    }

    void Update()
    {
        if (target != current)
        {
            meshRenderer.enabled = true;
            float p = Mathf.Clamp01((Time.time - targetTime) / animationTime);
            if (target == 1.0f) // target is on
            {
                animator.ActivationProgress(p);
                current = p;
            }
            else
            {
                animator.DeactivationProgress(p);
                current = 1 - p;
                if (p == 1.0f)
                    meshRenderer.enabled = false;
            }
        }
    }
}
