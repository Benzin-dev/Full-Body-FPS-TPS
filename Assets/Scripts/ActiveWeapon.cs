using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class ActiveWeapon : MonoBehaviour
{
    public UnityEngine.Animations.Rigging.Rig handIk;
    public Transform crossHairTarget;
    public Transform weaponParent;
    public Animator rigController;
    public bool isHolstered;

    private RaycastWeapon weapon;
    private CharacterAnimation characterAnimation;
    private CharacterAiming characterAiming;
    private bool hasWeapon;

    void Start()
    {
        characterAnimation = GetComponent<CharacterAnimation>();
        characterAiming = GetComponent<CharacterAiming>();

        RaycastWeapon existingWeapon = GetComponentInChildren<RaycastWeapon>();
        if (existingWeapon)
        {
            Equip(existingWeapon);
            AlignCamerasToWeapon(existingWeapon);
            hasWeapon = true;
        }
        else
        {
            hasWeapon = false;
        }
    }


    void Update()
    {
        FireWeapon();

        if (Input.GetKeyDown(KeyCode.X))
        {
            isHolstered = rigController.GetBool("holster_weapon");
            rigController.SetBool("holster_weapon", !isHolstered);
        }

        if (!hasWeapon)
        {
            rigController.SetBool("holster_weapon", true);
        }
    }

    private void FireWeapon()
    {
        if (weapon)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (characterAnimation.pose == CharacterAnimation.Pose.Idle)
                {
                    weapon.fireIsAllowed = false;
                }
                else
                {
                    weapon.fireIsAllowed = true;
                    // weapon.StartFiring();
                    weapon.isFiring = true;
                }
            }

            if (weapon.isFiring)
            {
                weapon.UpdateBullets(Time.deltaTime);
            }
            if (Input.GetButtonUp("Fire1"))
            {
                //weapon.StopFiring();
                weapon.isFiring = false;
            }
        }
    }


    private void AlignCamerasToWeapon(RaycastWeapon wpn)
    {
        Transform sight = wpn.transform.Find("Sight");
        if (!sight)
        {
            Debug.LogWarning("No 'Sight' object inside of weapon!");
        }
        else
        {
            characterAiming.aimFarVirtualCamera.Follow = sight;
            characterAiming.aimCloseVirtualCamera.Follow = sight;
            print("Cameras attached");
        }
    }

    public void Equip(RaycastWeapon newWeapon)
    {
        weapon = newWeapon;
        weapon.raycastDestination = crossHairTarget;
        weapon.transform.parent = weaponParent;
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        rigController.Play("equip_" + weapon.weaponName);
        hasWeapon = true;
        AlignCamerasToWeapon(newWeapon);
    }

}
