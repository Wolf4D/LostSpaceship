using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InformationPanel : MonoBehaviour
{
    public Text UnitName;
    public Text PrepareState;
    public Text Health;
    public Text Attack;
    public Text AttackCoolDown;
    public Text AttackRange;
    public Text Speed;
    public Text AbilityDesc;
    public Button AbilityButton;
    public Text AbilityButtonText;


    public ShipProperties currentUnit;


    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }
    

    // Update is called once per frame
    void LateUpdate()
    {
        if (currentUnit == null) return;
        UnitName.text = currentUnit.unitName;
        if (!currentUnit.hasMoved)
        { 
            PrepareState.text = "Ready to go!";
        }
        else
        {
            PrepareState.text = "Already moved";
        }
        Health.text = "Health:   " + currentUnit.HP + " / " + currentUnit.MaxHP;
        Attack.text = "Attack:   " + currentUnit.attack;

        if (currentUnit.attackCooldownLeft==0)
        {
            if (currentUnit.attack>0)
                AttackCoolDown.text = "Armed";
            else
                AttackCoolDown.text = "Non-combat";
        }
        else
            AttackCoolDown.text = "Reload " + currentUnit.attackCooldownLeft + " turns";

        AttackRange.text = "Range:   " + currentUnit.range;
        Speed.text = "Speed:   " + currentUnit.speed;

        if (currentUnit.abilityCooldownLeft > 0)
        {
            AbilityButton.interactable = false;
            AbilityButtonText.text = "Turns: " + currentUnit.abilityCooldownLeft;
        }
        else
        {
            AbilityButton.interactable = true;
            AbilityButtonText.text = "USE";
        }


    }
}
