using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterAnimation : MonoBehaviour
{
    [SerializeField] private float aimDuration = 0.2f;

    [Header("-Left Hand Grip Setup-")]
    [SerializeField] private TwoBoneIKConstraint leftHandIKHip;
    [SerializeField] private TwoBoneIKConstraint leftHandIKAim;

    [Header("-Pose Setup-")]
    [SerializeField] private Rig weaponPoseIdle;
    [SerializeField] private Rig weaponPoseHip;
    [SerializeField] private Rig weaponPoseAim;
    public Pose pose = Pose.Idle;

    public enum Pose
    {
        Idle, Hip, Aim, Ironsight
    }

    Animator animator;


    void Start()
    {
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        CheckForPose();
    }

    private void CheckForPose()
    {
        switch (pose)
        {
            case Pose.Idle:
                //weaponPoseIdle.weight += Time.deltaTime / aimDuration;
                //weaponPoseHip.weight -= Time.deltaTime / aimDuration;
                weaponPoseAim.weight -= Time.deltaTime / aimDuration;
                //SwitchLeftHandtoHip(false);
                animator.SetBool("isArmed", false);
                break;
            case Pose.Hip:
                //weaponPoseHip.weight += Time.deltaTime / aimDuration;
                //weaponPoseIdle.weight -= Time.deltaTime / aimDuration;
                //weaponPoseAim.weight -= Time.deltaTime / aimDuration;
                //SwitchLeftHandtoHip(true);
                animator.SetBool("isArmed", false);
                break;
            case Pose.Aim:
                weaponPoseAim.weight += Time.deltaTime / aimDuration;
                //weaponPoseIdle.weight -= Time.deltaTime / aimDuration;
                //weaponPoseHip.weight -= Time.deltaTime / aimDuration;
                //SwitchLeftHandtoHip(false);
                animator.SetBool("isArmed", true);
                break;
            case Pose.Ironsight:
                //weaponPoseAim.weight += Time.deltaTime / aimDuration;
                //weaponPoseIdle.weight -= Time.deltaTime / aimDuration;
                //weaponPoseHip.weight -= Time.deltaTime / aimDuration;
                //SwitchLeftHandtoHip(false);
                animator.SetBool("isArmed", true);
                break;
        }
    }


    private void SwitchLeftHandtoHip(bool toHip)
    {
        if (!toHip)
        {
            leftHandIKAim.weight += Time.deltaTime / aimDuration;
            leftHandIKHip.weight -= Time.deltaTime / aimDuration;
        }
        else
        {
            leftHandIKAim.weight -= Time.deltaTime / aimDuration;
            leftHandIKHip.weight += Time.deltaTime / aimDuration;
        }
    }
}
