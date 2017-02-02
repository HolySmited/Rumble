using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    //Script is for testing purposes only
    public int health = 100;
    public Material skin;
    public Material damage;
    public float damageTimer = 0.25f;
    public bool damaged = false;

    private void Update()
    {
        if (damaged)
            damageTimer -= Time.deltaTime;

        if (damageTimer <= 0)
        {
            gameObject.GetComponent<MeshRenderer>().material = skin;
            damageTimer = 0.25f;
            damaged = false;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;

        if (health > 0)
        {
            gameObject.GetComponent<MeshRenderer>().material = damage;
            damaged = true;
        }
        else
            Destroy(gameObject);
    }
}
