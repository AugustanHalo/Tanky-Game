using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 20f;

    [SerializeField] private GameObject explosion;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            return;
        }
        // Create an explosion effect
        GameObject explo = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(explo, 0.75f);
        Destroy(gameObject);
    }
}
