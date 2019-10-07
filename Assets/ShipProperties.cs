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

    public BattleField battleField;

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

    public Cannon cannon;
    public float dieTime = 3.0f;
    public GameObject dieEffect;
    public bool isAlive = true;

    private ProgressbarExample lifeMeter;

    public enum Commands
    {
        Stand = 0,
        Move = 1,
        Attack = 2,
        Die = 3
    };

    public Commands command = Commands.Stand;

    // Start is called before the first frame update
    void Start()
    {
        lifeMeter = GetComponentInChildren<ProgressbarExample>();
    }

    bool SmoothMove()
    {
        Vector3 direction = target - transform.position;
        float brake = direction.magnitude / 10.0f;
        if (brake > 1.0f) brake = 1.0f;

        Quaternion toRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, speed * Time.deltaTime / brake);

        transform.Translate(Vector3.forward * brake * 8.0f * speed * Time.deltaTime);

        if (direction.magnitude < 2.35f)
             return true;

        return false;
    }


    bool SmoothRotate()
    {
        Vector3 direction = target - transform.position;
        float brake = direction.magnitude / 5.0f;
        if (brake > 1.0f) brake = 1.0f;

        Quaternion toRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 3.0f * speed * Time.deltaTime / brake);

        float angle = Vector3.Angle(direction, transform.forward);
        if (angle < 0.35f)
            return true;

        return false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (command)
        {
            case (Commands.Stand): break;
            case (Commands.Move):
                {
                    if (SmoothMove())
                        command = Commands.Stand;
                }
                break;

            case (Commands.Attack):
                {
                    if (SmoothRotate())
                    {
                        cannon.gameObject.SetActive(true);
                        cannon.Fire();
                        command = Commands.Stand;
                    }
                }
                break;

            case (Commands.Die):
                {
                    lifeMeter.gameObject.SetActive(false);
                    dieTime -= Time.deltaTime;
                    if (!dieEffect.activeSelf)
                    {
                        dieEffect.SetActive(true);
                        dieEffect.transform.parent = transform.parent;
                        this.GetComponentInChildren<MeshRenderer>().enabled = false;
                    }
                    else
                    {
                        if (dieTime <= 0)
                            Destroy(this.gameObject);
                    }
                }
                break;
        }
    }

    public void Move(Vector3 ctarget)
    {
        target = new Vector3(ctarget.x, this.transform.position.y, ctarget.z);
        command = Commands.Move;
        hasMoved = true;
    }

    public void Attack(ShipProperties ctarget)
    {
        target = new Vector3(ctarget.transform.position.x, this.transform.position.y, ctarget.transform.position.z);
        // Все настройки пушки - тут
        cannon.target = ctarget;
        cannon.damage = attack;

        command = Commands.Attack;
        hasMoved = true;
    }

    public bool ReceiveDamage(int damage)
    {
        HP -= damage;
        lifeMeter.progressPercent = 100.0f * (float)(HP) / (float)(MaxHP);
        if (HP <= 0)
        {
            isAlive = false;
            command = Commands.Die;
            return true;
        }

        return false;
    }
}
