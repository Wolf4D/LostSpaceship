using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipProperties : MonoBehaviour
{
    public enum BattleSides
    {
        Neutral = 0,
        Earth = 1,
        Asura = 2,
        Heretic = 3
    };

    public BattleSides side = BattleSides.Earth;

    public int HP;
    public int attackDistance;
    public int attackPower;
    public int attackCooldown;
    public int attackCooldownLeft = 0;
    public int moveSpeed;
    public bool hasMoved = false;

    public int specialPropertyCooldownLeft = 0;

    // Start is called before the first frame update
    void Start()
    {                 
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
