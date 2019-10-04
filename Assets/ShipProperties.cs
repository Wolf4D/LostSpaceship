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
    public int MaxHP;

    public int speed;
    public int attack;
    public int attackCooldown;
    public int attackCooldownLeft = 0;
    public int range;
    public bool hasMoved = false;
    public string unitName;

    public int abilityCooldownLeft = 0;

    private Vector3 target;

    public enum Commands
    {
        Stand = 0,
        Move = 1
    };

    public Commands command = Commands.Stand;

    // Start is called before the first frame update
    void Start()
    {                 
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (command)
        {
            case (Commands.Stand): break;
            case (Commands.Move):
                {
                    Vector3 direction = target - transform.position;
                    float brake = direction.magnitude / 10.0f;
                    if (brake > 1.0f) brake = 1.0f;

                    Quaternion toRotation = Quaternion.LookRotation(direction);
                    transform.rotation =  Quaternion.Lerp(transform.rotation, toRotation, speed * Time.deltaTime / brake);
                
                    transform.Translate(Vector3.forward * brake * 8.0f * speed * Time.deltaTime);

                    if (direction.magnitude < 2.35f)
                    {
                        command = Commands.Stand;
                        return;
                    }
                }
                break;
        }
    }

    public void Move(Vector3 ctarget)
    {
        target = new Vector3(ctarget.x, this.transform.position.y, ctarget.z);
        command = Commands.Move;
    }
}
