using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class TankShoot : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject ricochetBulletPrefab;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float fireRate = 0.5f;

    [Header("Fire Point")]
    [SerializeField] private Transform firePoint;

    private float nextFireTime = 0f;

    void Update()
    {
        // Check if fire button is pressed (Left mouse click or Space)
        if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space)) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
        
        if((Input.GetButtonDown("Fire2")  && Time.time >= nextFireTime))
        {
            //TripleShot();
            RicochetShot();
            nextFireTime = Time.time + fireRate;
        }

    }

    public void Shoot()
    {
        // Create the bullet at the fire point position and rotation
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Get the Rigidbody2D component from the bullet
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        
        // Add force to the bullet in the direction it's facing
        rb.velocity = firePoint.right * bulletSpeed;

        // Optional: Destroy the bullet after some time to prevent memory issues
        Destroy(bullet, 3f);
    }

    private void TripleShot()
    {
        float spreadAngle = 15f; // Total spread angle between bullets

        // Calculate angles for left, center, and right bullets
        float[] angles = { -spreadAngle, 0f, spreadAngle };

        for (int i = 0; i < 3; i++)
        {
            // Create bullet at fire point
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // Calculate the rotation for this bullet
            Quaternion rotation = firePoint.rotation * Quaternion.Euler(0, 0, angles[i]);
            bullet.transform.rotation = rotation;

            // Get rigidbody and apply velocity in the rotated direction
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = bullet.transform.right * bulletSpeed;

            Destroy(bullet, 3f);
        }
    }

    void RicochetShot()
    {
        GameObject bullet = Instantiate(ricochetBulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = firePoint.right * bulletSpeed;
    }
}