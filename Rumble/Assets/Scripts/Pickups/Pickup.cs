using UnityEngine;

/// <summary>
/// This class is a basic pickup class that stores the amount the
/// pickup is worth and returns it, also destroying the pickup.
/// </summary>

public class Pickup : MonoBehaviour
{
    //Amount returned when used
    public int amount;

    //Returns the amount and destroys the pickup
    public int Use()
    {
        Destroy(gameObject, 0.1f);
        return amount;
    }
}
