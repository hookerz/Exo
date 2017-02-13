using UnityEngine;
using System.Collections;

public enum RigidBodyAnimationMethod
{
    Kinematic,
    Forces,
}

public enum RigidBodyAttachMethod
{
    None,
    RigidBodyAttachScript,
    FixedJoint,
    Parent,
}

public static class Utils
{
    public static Quaternion LocalToGlobalRotation(Quaternion localRot, Transform transform)
    {
        Vector3 newForward = transform.TransformVector(localRot * Vector3.forward);
        Vector3 newUp = transform.TransformVector(localRot * Vector3.up);
        Quaternion quat = Quaternion.LookRotation(newForward.normalized, newUp.normalized);
        return quat;
    }

    public static IEnumerator DelayedAction(System.Action func, float secondToWait)
    {
        yield return new WaitForSeconds(secondToWait);

        func();
    }

    public static Quaternion ShortestRelativeRotation(Quaternion from, Quaternion to, bool keepAnglePositive=true)
    {
        Quaternion relativeRotation = to * Quaternion.Inverse(from);

        Vector3 rotationAxis = Vector3.zero;
        float angle = 0.0f;
        relativeRotation.ToAngleAxis(out angle, out rotationAxis);
        // we need the shortest rotation here, so if the angle is too big, flip it
        if (angle > 179.999999999999999f)
        {
            if (keepAnglePositive)
            {
                angle = 360.0f - angle;
                rotationAxis = -rotationAxis;
            }
            else
            {
                angle = -360.0f - angle;
            }
        }

        return Quaternion.AngleAxis(angle, rotationAxis);
    }

    public static void ShortestRelativeRotation(Quaternion from, Quaternion to, out float angle, out Vector3 rotationAxis, bool keepAnglePositive = true)
    {
        Quaternion relativeRotation = to * Quaternion.Inverse(from);
        relativeRotation.ToAngleAxis(out angle, out rotationAxis);
        // we need the shortest rotation here, so if the angle is too big, flip it
        if (angle > 179.999999999999999f)
        {
            angle = 360.0f - angle;
            if (keepAnglePositive)
            {
                rotationAxis = -rotationAxis;
            }
            else
            {
                angle = -angle;
            }
        }
    }

    public static WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    public static float SmoothStepEase(float t)
    {
        return Mathf.SmoothStep(0, 1, t);
    }

    public static float LinearEase(float t)
    {
        return t;
    }
    
    public static IEnumerator AnimateRigidBodyToTransformByEasingCoroutine(Transform destinationTransform, Rigidbody body, float time, System.Func<float,float> easing, 
        RigidBodyAnimationMethod animationMethod, RigidBodyAttachMethod attachMethod, Rigidbody attachToBody = null, System.Action callback = null)
    {
        bool previousIsKinematic = body.isKinematic;
        if (animationMethod == RigidBodyAnimationMethod.Kinematic && body.isKinematic == false)
            body.isKinematic = true;

        var a = body.GetComponent<RigidBodyAttach>();
        if (a != null)
        {
            a.enabled = false; // disable attachment while animating
        }

        Vector3 startPosition = body.position;
        Quaternion startRotation = body.rotation;
        float startTime = Time.time;

        while (true)
        {
            float t = Mathf.Clamp01((Time.time - startTime) / time);
            float te = easing(t);

            Vector3 pos = Vector3.Lerp(startPosition, destinationTransform.position, te);
            Quaternion quat = Quaternion.Slerp(startRotation, destinationTransform.rotation, te);

            if (animationMethod == RigidBodyAnimationMethod.Kinematic)
            {
                body.MovePosition(pos);
                body.MoveRotation(quat);
            }
            else
            {
                body.velocity = (pos - body.position) / Time.deltaTime;
                Vector3 axis;
                float angle;
                ShortestRelativeRotation(body.rotation, quat, out angle, out axis);

                float angleRads = angle * Mathf.Deg2Rad;
                float speed = angleRads / Time.fixedDeltaTime;
                Vector3 angularVelocity = axis * speed;
                body.angularVelocity = angularVelocity;
            }

            if (t == 1.0f)
            {
                break;
            }

            yield return waitForFixedUpdate;
        }

        if (animationMethod == RigidBodyAnimationMethod.Kinematic && body.isKinematic != previousIsKinematic)
            body.isKinematic = previousIsKinematic;

        if (attachMethod == RigidBodyAttachMethod.RigidBodyAttachScript)
        {
            // wait for 1 more frame to ensure the final MoveX was applied
            yield return waitForFixedUpdate;

            if (a == null)
            {
                a = body.gameObject.AddComponent<RigidBodyAttach>();
            }
            else
            {
                a.enabled = true;
            }
            a.body = body;
            a.attachTo = destinationTransform;
        }
        else if (attachMethod == RigidBodyAttachMethod.FixedJoint)
        {
            if (attachToBody == null)
                attachToBody = destinationTransform.GetComponent<Rigidbody>();

            if (attachToBody != null)
            {
                body.velocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                body.transform.position = destinationTransform.position;
                body.transform.rotation = destinationTransform.rotation;

                var joint = body.gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = attachToBody;
                joint.autoConfigureConnectedAnchor = false;
                joint.anchor = Vector3.zero;
                Vector3 localAttachPointOffset = attachToBody.transform.InverseTransformPoint(destinationTransform.position);
                joint.connectedAnchor = localAttachPointOffset;
            }
            else
            {
                throw new System.Exception("Excepted a RigidBody component on object: " + destinationTransform);
            }
        }
        else if (attachMethod == RigidBodyAttachMethod.Parent)
        {
            body.isKinematic = true;
            yield return null;
            body.transform.SetParent(destinationTransform);
            body.transform.localPosition = Vector3.zero;
            body.transform.localRotation = Quaternion.identity;
        }

        if (callback != null)
            callback();
    }

    public static IEnumerator ActivateInOutAnimatedCoroutine(IInOutAnimated a, System.Action callback = null)
    {
        return ActivateInOutAnimatedCoroutine(a, a.DefaultInTime, callback);
    }

    public static IEnumerator ActivateInOutAnimatedCoroutine(IInOutAnimated a, float duration, System.Action callback=null)
    {

        float startTime = Time.time;

        a.StartActivation();
        while (true)
        {
            float t = Mathf.Clamp01((Time.time - startTime) / duration);
            a.ActivationProgress(t);

            if (t == 1.0f)
                break;

            yield return null;
        }

        a.Activated();

        if (callback != null)
            callback();
    }

    public static IEnumerator DeactivateInOutAnimatedCoroutine(IInOutAnimated a, System.Action callback = null)
    {
        return DeactivateInOutAnimatedCoroutine(a, a.DefaultOutTime, callback);
    }

    public static IEnumerator DeactivateInOutAnimatedCoroutine(IInOutAnimated a, float duration, System.Action callback = null)
    {
        float startTime = Time.time;

        a.StartDeactivation();
        while (true)
        {
            float t = Mathf.Clamp01((Time.time - startTime) / duration);
            a.DeactivationProgress(t);

            if (t == 1.0f)
                break;

            yield return null;
        }

        a.Deactivated();

        if (callback != null)
            callback();
    }

    public static Quaternion LookRotationUpPriority(Vector3 forward, Vector3 up)
    {
        Vector3 x = Vector3.Cross(up, forward).normalized;
        Vector3 newForward = Vector3.Cross(x, up).normalized;
        return Quaternion.LookRotation(newForward, up);
    }

    public static void SetActive(GameObject[] objects, bool active)
    {
        if (objects == null || objects.Length == 0)
            return;

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null)
                objects[i].SetActive(active);
        }
    }

    public static void SetBehavioursEnabled(MonoBehaviour[] behaviours, bool enabled)
    {
        if (behaviours == null || behaviours.Length == 0)
            return;

        for (int i = 0; i < behaviours.Length; i++)
        {
            if (behaviours[i] != null)
                behaviours[i].enabled = enabled;
        }
    }


    public static IEnumerator FadeOutAudioSource(GvrAudioSource source, float duration)
    {
        float startVolume = source.volume;
        float startTime = Time.time;
        while (true)
        {
            float t = Mathf.Clamp01((Time.time - startTime) / duration);
            float v = Mathf.SmoothStep(startVolume, 0, t);
            source.volume = v;

            if (t == 1.0f)
                break;

            yield return null;
        }
        source.Stop();
    }

    public static IEnumerator SmoothStepAudioSourceVolume(GvrAudioSource source, float duration, float to, bool stop = false)
    {
        float startVolume = source.volume;
        float startTime = Time.time;
        while (true)
        {
            float t = Mathf.Clamp01((Time.time - startTime) / duration);
            float v = Mathf.SmoothStep(startVolume, to, t);
            source.volume = v;

            if (t == 1.0f)
                break;

            yield return null;
        }
        if(stop)
            source.Stop();
    }

    public static IEnumerator SmoothStepAudioSourceVolume(AudioSource source, float duration, float to, bool stop = false)
    {
        float startVolume = source.volume;
        float startTime = Time.time;
        while (true)
        {
            float t = Mathf.Clamp01((Time.time - startTime) / duration);
            float v = Mathf.SmoothStep(startVolume, to, t);
            source.volume = v;

            if (t == 1.0f)
                break;

            yield return null;
        }
        if (stop)
            source.Stop();
    }

    public static IEnumerator SmoothStepAudioSourceVolumeUnscaled(AudioSource source, float duration, float to, bool stop = false)
    {
        float startVolume = source.volume;
        float startTime = Time.unscaledTime;
        while (true)
        {
            float t = Mathf.Clamp01((Time.unscaledTime - startTime) / duration);
            float v = Mathf.SmoothStep(startVolume, to, t);
            source.volume = v;

            if (t == 1.0f)
                break;

            yield return null;
        }
        if (stop)
            source.Stop();
    }

    public static IEnumerator SequenceIntroLoopClips(GvrAudioSource source, AudioClip intro, AudioClip loop)
    {
        source.clip = intro;
        source.loop = false;
        source.Play();

        yield return new WaitForSecondsRealtime(intro.length);

        source.clip = loop;
        source.loop = true;
        source.Play();
    }

}
