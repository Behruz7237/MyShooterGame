using UnityEngine;

public class FixMutantPosture : MonoBehaviour
{
    private Animator animator;
    private Transform neckBone;
    private Transform headBone;

    [Header("Adjust Posture")]
    [Tooltip("Tweak these numbers while the game is playing to find the perfect tilt!")]
    public Vector3 neckTilt = new Vector3(-30f, 0f, 0f);
    public Vector3 headTilt = new Vector3(-20f, 0f, 0f);

    void Start()
    {
        animator = GetComponent<Animator>();

        // Unity's Humanoid system automatically finds where the neck and head are!
        if (animator != null && animator.isHuman)
        {
            neckBone = animator.GetBoneTransform(HumanBodyBones.Neck);
            headBone = animator.GetBoneTransform(HumanBodyBones.Head);
        }
    }

    // LateUpdate runs exactly one millisecond AFTER the animation poses the body.
    // We let the animation do its thing, then we step in and bend the neck back!
    void LateUpdate()
    {
        if (neckBone != null)
        {
            // Bend the neck relative to whatever the animation is currently doing
            neckBone.localRotation *= Quaternion.Euler(neckTilt);
        }

        if (headBone != null)
        {
            // Bend the head relative to the neck
            headBone.localRotation *= Quaternion.Euler(headTilt);
        }
    }
}