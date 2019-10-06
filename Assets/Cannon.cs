using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    //public Projectile projectile;
    public GameObject partSystemShoot;
    [HideInInspector]
    public int damage;
    public float bulletSpeed = 5.5f;
    public float effectTime = 8.5f;

    public ShipProperties target;
    public GameObject partSystemHit;
    float delayLeftToWait;

    // Start is called before the first frame update
    void Start()
    {
        //projectile = Instantiate(projectile, this.transform);
        //projectile.gameObject.SetActive(false);
    }

    public void Fire()
    {
        delayLeftToWait = (this.transform.position - target.transform.position).magnitude / bulletSpeed;
        partSystemShoot.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        delayLeftToWait -= Time.deltaTime;
        if (target != null)
        {
            if (delayLeftToWait <= 0)
            {
                if (!target.ReceiveDamage(damage))
                {
                    GameObject tmp = Instantiate(partSystemHit, target.transform);
                    tmp.transform.position = target.transform.position;
                    tmp.transform.LookAt(this.transform);
                    tmp.SetActive(true);
                    delayLeftToWait = effectTime;
                }
                else
                    delayLeftToWait = target.dieTime / 4.0f;

                target = null;
                //delayLeftToWait = effectTime;
            }
        }
        else
        {
            if (delayLeftToWait <= 0)
               this.gameObject.SetActive(false);


        }
    }
}
