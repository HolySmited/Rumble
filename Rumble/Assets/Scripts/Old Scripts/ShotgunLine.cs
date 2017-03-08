using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (LineRenderer))]
public class ShotgunLine : MonoBehaviour
{
    private float lifetime;

    public void OnActive(float _lifetime, Ray shootRay, RaycastHit shootHit, float range, LayerMask collisionMask, int damage)
    {
        lifetime = _lifetime;
        LineRenderer renderer = GetComponent<LineRenderer>();

        renderer.SetPosition(0, transform.position);

        //Create the shootRay data
        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        //If the shootRay hits something within range of the weapon on the shootable layer
        if (Physics.Raycast(shootRay, out shootHit, range, collisionMask))
        {
            //Set the end position of the line renderer to whatever was hit
            renderer.SetPosition(1, shootHit.point);
            //Find the object that was hit
            GameObject objectHit = shootHit.collider.gameObject;
            //If it was another player
            if (objectHit.tag == "Player")
            {
                //Tell the player to take damage
                PlayerStats enemy = objectHit.GetComponent<PlayerStats>();
                enemy.TakeDamage(damage);
            }
        }
        //If the shootRay did not hit anything within range
        else
            //Set the end position of the line renderer to the maximum range of the weapon
            renderer.SetPosition(1, shootRay.origin + shootRay.direction * range);

        StartCoroutine("Destroy");
    }

    IEnumerator Destroy()
    {
        float timeAlive = 0f;

        while (timeAlive < lifetime)
        {
            timeAlive += Time.deltaTime;
            yield return null;
        }

        gameObject.GetComponent<LineRenderer>().enabled = false;
        gameObject.SetActive(false);
    }
}
