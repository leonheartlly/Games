
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [SerializeField]
    private int attackStrength;

    [SerializeField]
    private proType projectileType;

    /// <summary>
    /// Dano do ataque.
    /// </summary>
    public int AttackStrength
    {
        get
        {
            return attackStrength;
        }
    }

    /// <summary>
    /// Tipo do projétil
    /// </summary>
    public proType ProjectileType
    {
        get
        {
            return projectileType;
        }
    }
}

public enum proType
{
    ROCK, ARROW, FIREBALL
};
