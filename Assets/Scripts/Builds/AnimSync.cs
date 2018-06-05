using UnityEngine;

public class AnimSync : MonoBehaviour
{
    public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget target, MatchTargetWeightMask weightMask, float normalisedStartTime, float normalisedEndTime)
    {
        var animator = GetComponent<Animator>();

        if (animator.isMatchingTarget)
            return;

        float normalizeTime = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f);

        if (normalizeTime > normalisedEndTime)
            return;

        animator.MatchTarget(matchPosition, matchRotation, target, weightMask, normalisedStartTime, normalisedEndTime);
    }
}
