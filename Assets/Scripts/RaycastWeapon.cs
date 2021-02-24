using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FireMode { Semi, Auto, Burst }
public class RaycastWeapon : MonoBehaviour
{
    class Bullet
    {
        public float time;
        public Vector3 initialPosition;
        public Vector3 initialVelocity;
        public TrailRenderer tracer;
    }



    public bool isFiring = false;

    [Header("-Weapon Properties-")]
    [SerializeField] private bool hasAutoMode;
    [SerializeField] private bool hasBurstMode;
    [SerializeField] private byte bulletsPerBurst;
    [SerializeField] private float rateOfFire = 600f;
    [SerializeField] private FireMode fireMode = FireMode.Semi;

    [Header("-Bullet Settings-")] 
    [SerializeField] private float bulletSpeed = 600f;
    [SerializeField] private float bulletDrop;

    [Header("-VFX-")]
    [SerializeField] private ParticleSystem[] muzzleFlash;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private TrailRenderer tracerEffect;

    [Header("-Raycast Options-")]
    [SerializeField] private Transform raycastOrigin;
    public Transform raycastDestination;

    [Header("-Animations-")] 
    public string weaponName;
    

    private Ray ray;
    private RaycastHit hitInfo;
    private float nextFire;
    private List<Bullet> bullets = new List<Bullet>();
    private float maxLifetime = 3f;
    public bool fireIsAllowed = false;

    Vector3 GetPosition(Bullet bullet)
    {
        Vector3 gravity = Vector3.down * bulletDrop;
        return (bullet.initialPosition) + (bullet.initialVelocity * bullet.time) +
               (0.5f * gravity * bullet.time * bullet.time);
    }

    Bullet CreateBullet(Vector3 position, Vector3 velocity)
    {
        Bullet bullet = new Bullet();
        bullet.initialPosition = position;
        bullet.initialVelocity = velocity;
        bullet.time = 0.0f;
        bullet.tracer = Instantiate(tracerEffect, position, Quaternion.identity);
        bullet.tracer.AddPosition(position);
        return bullet;
    }

    private void Update()
    {
        SwitchFireMode();
        Fire();
    }

    public void StartFiring()
    {
        isFiring = true;
    }

    public void StopFiring()
    {
        isFiring = false;
    }

    public void UpdateBullets(float deltaTime)
    {
        SimulateBullets(deltaTime);
        DestroyBullets();
    }

    private void SimulateBullets(float deltaTime)
    {
        bullets.ForEach(bullet =>
        {
            Vector3 p0 = GetPosition(bullet);
            bullet.time += deltaTime;
            Vector3 p1 = GetPosition(bullet);
            RaycastSegment(p0, p1, bullet);
        });
    }

    private void DestroyBullets()
    {
        bullets.RemoveAll(bullet => bullet.time >= maxLifetime);
    }

    private void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        ray.origin = start;
        ray.direction = direction;
        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(20);

            bullet.tracer.transform.position = hitInfo.point;
            bullet.time = maxLifetime;
        }
        else
        {
            
            bullet.tracer.transform.position = end;
        }
    }

    private void Shoot()
    {
        foreach (var particle in muzzleFlash)
        {
            particle.Emit(1);
        }

        Vector3 velocity = (raycastDestination.position - raycastOrigin.position).normalized * bulletSpeed;
        var bullet = CreateBullet(raycastOrigin.position, velocity);
        bullets.Add(bullet);
    }

    

    public void SwitchFireMode()
    {
        if (Input.GetButtonDown("Change Firemode"))
        {
            if (hasAutoMode && hasBurstMode)
            {
                if ((int)fireMode < 2)
                    fireMode += 1;
                else
                    fireMode = 0;
            }

            if (hasAutoMode && !hasBurstMode)
            {
                if ((int)fireMode < 1)
                    fireMode += 1;
                else
                    fireMode = 0;
            }

            if (!hasAutoMode && hasBurstMode)
            {
                if ((int)fireMode < 2)
                    fireMode += 1;
                else if ((int)fireMode == 1)
                    fireMode = 0;
            }
        }
    }

    public void Fire()
    {
        if (fireIsAllowed)
        {
            if (fireMode == FireMode.Semi)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    Shoot();
                }
            }
            else if (fireMode == FireMode.Auto && Time.time >= nextFire)
            {
                if (Input.GetButton("Fire1"))
                {
                    nextFire = Time.time + (60f / rateOfFire);
                    Shoot();
                }
            }
            else if (fireMode == FireMode.Burst)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    StartCoroutine(BurstFire(60f / rateOfFire));
                }
            }
        }
    }

    IEnumerator BurstFire(float delay)
    {
        for (int i = 0; i < bulletsPerBurst; i++)
        {
            //yield return new WaitForEndOfFrame();
            Shoot();
            yield return new WaitForSeconds(delay);
        }
        yield return new WaitForSeconds(delay);
    }
}
