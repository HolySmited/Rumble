using System.Collections;
using UnityEngine;

/// <summary>
/// This class represents the projectiles fired by the shotgun. While
/// each projectile uses hitscan functionality, because the shotgun shoots
/// pellets in a spread, it is considered a projectile weapon.
/// </summary>

[RequireComponent (typeof (LineRenderer))]
public class ShotgunProjectile : ProjectileBehavior
{
    //Angle of the spread on either side of transform.forward
    public float spreadInDegrees;
    //Range of the pellets
    public float range;
    //The LineRenderer on the pellet
    private LineRenderer lineRenderer;

    private void Awake()
    {
        //Get the LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
    }

    //Called when the object is activated
    public override void Activate(LayerMask collisionMask, PlayerStats firingPlayer)
    {
        //Activate the object
        gameObject.SetActive(true);

        //Rotate the pellet to a random angle within the spread range
        Vector3 randomAngle = new Vector3(transform.rotation.eulerAngles.x + Random.Range(-spreadInDegrees, spreadInDegrees), 
            transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        transform.eulerAngles = randomAngle;

        //Activate the line renderer
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, transform.position);

        //Create the shootRay and shootHit data
        Ray shootRay = new Ray();
        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;
        RaycastHit shootHit;

        //If the shootRay hits something within range of the weapon on the shootable layer
        if (Physics.Raycast(shootRay, out shootHit, range, collisionMask))
        {
            //Set the end position of the line renderer to whatever was hit
            lineRenderer.SetPosition(1, shootHit.point);
            //Find the object that was hit
            GameObject objectHit = shootHit.collider.gameObject;
            //If it was another player
            if (objectHit.tag == "Player")
            {
                //Tell the player to take damage
                PlayerStats enemy = objectHit.GetComponent<PlayerStats>();
                enemy.TakeDamage(damage, firingPlayer);
            }
        }
        //If the shootRay did not hit anything within range
        else
            //Set the end position of the line renderer to the maximum range of the weapon
            lineRenderer.SetPosition(1, shootRay.origin + shootRay.direction * range);

        //Set the object to destroy (a coroutine is used instead of Destroy() because the object
        //is only being set to inactive, not actually destroyed
        StartCoroutine("Destroy");
    }
}
