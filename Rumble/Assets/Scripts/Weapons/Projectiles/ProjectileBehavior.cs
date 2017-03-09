using System.Collections;
using UnityEngine;

/// <summary>
/// This class is the parent class for all projectile types. It includes
/// stats and behavior common to all projectiles.
/// </summary>

public abstract class ProjectileBehavior : MonoBehaviour
{
    //Stats common to all projectiles
    public float lifetime;
    public int damage;

    public abstract void Activate(LayerMask collisionMask, PlayerStats firingPlayer);

    //Destroys the projectile after it has reached its lifetime
    protected IEnumerator Destroy()
    {
        float timeAlive = 0f;

        while (timeAlive < lifetime)
        {
            timeAlive += Time.deltaTime;
            yield return null;
        }

        //Set the object to false, since it's part of an object pool
        gameObject.SetActive(false);
    }
}
