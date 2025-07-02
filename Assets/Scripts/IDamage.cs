using UnityEngine;

public interface IDamage
{
    float dmg { get; set; }
    bool type { get; set; }
    Vector3 offset { get; set; }
    PlayerController player { get; set; }
}