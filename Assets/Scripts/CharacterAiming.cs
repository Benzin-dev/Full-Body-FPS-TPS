using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using UnityEngine.Rendering.HighDefinition;

public class CharacterAiming : MonoBehaviour
{
    [SerializeField] private float turnSpeed = 15f;

    [Header("-Camera Setup-")]
    [SerializeField] private CinemachineVirtualCamera mainVirtualCamera;
    public CinemachineVirtualCamera aimCloseVirtualCamera;
    public CinemachineVirtualCamera aimFarVirtualCamera;

    [SerializeField] private Animator rigLayerAnimator;

    private CharacterAnimation characterAnimation;
    private Camera mainCamera;
    private bool isAiming = false;
    private float scroll;


    void Start()
    {
        characterAnimation = GetComponent<CharacterAnimation>();
        mainCamera = Camera.main;
    }

    
    void FixedUpdate()
    {
        RotateCharacterFromCamera();

    }

    void Update()
    {
        Aim();
        CheckForPose();
    }

    void LateUpdate()
    {
        
        
    }

    private void CheckForPose()
    {
        switch (characterAnimation.pose)
        {
            case CharacterAnimation.Pose.Idle:
                SwitchCam(true, false, false);
                break;
            case CharacterAnimation.Pose.Hip:
                SwitchCam(true, false, false);
                break;
            case CharacterAnimation.Pose.Aim:
                SwitchCam(false, true, false);
                break;
            case CharacterAnimation.Pose.Ironsight:
                SwitchCam(false, false, true);
                break;
        }
    }

    private void Aim()
    {
        bool isHolstered = rigLayerAnimator.GetBool("holster_weapon");
        if (Input.GetButtonDown("Fire2") && !isAiming && !isHolstered)
        {
            characterAnimation.pose = CharacterAnimation.Pose.Aim;
            isAiming = true;
        }
        else if (Input.GetButtonDown("Fire2") && isAiming || isHolstered)
        {
            characterAnimation.pose = CharacterAnimation.Pose.Idle;
            isAiming = false;
        }

        float mWheelVal = Mathf.Clamp((Input.GetAxis("Mouse ScrollWheel") * 10f), -1f, 1f);
        if (mWheelVal != 0f && isAiming)
        {
            scroll = Mathf.Clamp((scroll + mWheelVal), 0f, 1f);


            if (scroll >= 1f)
            {
                characterAnimation.pose = CharacterAnimation.Pose.Ironsight;
            }
            else if (scroll <= 0f)
            {
                characterAnimation.pose = CharacterAnimation.Pose.Aim;
            }
        }

    }


    private void SwitchCam(bool mainCam, bool aimFarCam, bool aimCloseCam)
    {
        mainVirtualCamera.enabled = mainCam;
        aimCloseVirtualCamera.enabled = aimCloseCam;
        aimFarVirtualCamera.enabled = aimFarCam;
    }

    private void RotateCharacterFromCamera()
    {
        float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed * Time.fixedDeltaTime);
    }

    

    
}
