using UnityEngine;

public interface IDamage
{
    float dmg { get; }
    bool type { get; }
    Vector3 offset { get; }
}