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
        
    }
    

    // Update is called once per frame
    void OnEnable()
    {
        if (currentUnit == null) return;
        UnitName.text = currentUnit.name;
        if (!currentUnit.hasMoved)
        { 
            PrepareState.text = "Ready to go!";
        }
        else
        {
            PrepareState.text = "NOT READY!";
        }
        Health.text = "Health:   " + currentUnit.HP;
        Attack.text = "Attack:   " + currentUnit.attackPower;

        if (currentUnit.attackCooldownLeft==0)
            AttackCoolDown.text = "Armed";
        else
            AttackCoolDown.text = "Reload " + currentUnit.attackCooldownLeft + " turns";

        AttackRange.text = "Range:   " + currentUnit.attackDistance;
        Speed.text = "Speed:   " + currentUnit.moveSpeed;

        if (currentUnit.specialPropertyCooldownLeft > 0)
        {
            AbilityButton.interactable = false;
            AbilityButtonText.text = currentUnit.specialPropertyCooldownLeft + " turns";
        }
        else
        {
            AbilityButton.interactable = true;
            AbilityButtonText.text = "USE";
        }


    }
}
