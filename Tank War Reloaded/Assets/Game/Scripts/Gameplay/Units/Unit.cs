using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Hit Point")]
    [SerializeField] protected int maxHp;
    [SerializeField] protected int currentHp;
    [SerializeField] protected int maxArmor;
    [SerializeField] protected int currentArmor;

    protected bool killed = false;

    public int MaxHp => maxHp;
    public int CurrentHp => currentHp;
    public bool Killed => killed;

    public virtual void TakeDamage(int damage) {
        currentHp -= damage;
        if (currentHp <= 0) {
            killed = true;
            gameObject.SetActive(false);
        }
    }

    public virtual void Reset() {
        currentHp = maxHp;
        killed = false;
    }
}

public enum HullType {
    Light = 0,
    Medium = 1,
    Heavy = 2,
    Super = 3,
}
