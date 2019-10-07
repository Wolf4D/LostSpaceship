using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TurnSystem : MonoBehaviour
{
    ShipProperties.BattleSides currentSide = ShipProperties.BattleSides.Earth;
    public int actionsCounterForSide = 4;
    public Color disableColor;

    public GameObject[] turnIndicators;

    public GameObject[] turnBeginEffect;
    public GameObject[] sideLogoBanner;

    public SidesStats sides;

    public AI AsuraAI;
    public AI HereticAI;

    // Start is called before the first frame update
    void Start()
    {
        sides = FindObjectOfType<SidesStats>();
    }

    public void OneActionMade()
    {
        actionsCounterForSide -= 1;
        turnIndicators[actionsCounterForSide].GetComponent<Image>().color = disableColor;
        if (actionsCounterForSide == 0)
        {
            NextTurn();
        }
    }

    public void NextTurn()
    {
        if (AsuraAI != null)  AsuraAI.isMyTurn = false;
        if (AsuraAI != null) AsuraAI.isMyTurn = false;

        sides.CheckBeacons();
        sides.CollectMoney(currentSide);

        ShipProperties[] allShips = FindObjectsOfType<ShipProperties>();

        foreach (ShipProperties shp in allShips)
        {
            if (shp.side == currentSide)
            {
                shp.hasMoved = false;
            }
        }

        switch (currentSide)
        {
            case (ShipProperties.BattleSides.Earth): { currentSide = ShipProperties.BattleSides.Asura; } break;
            case (ShipProperties.BattleSides.Asura): { currentSide = ShipProperties.BattleSides.Heretic; } break;
            case (ShipProperties.BattleSides.Heretic): { currentSide = ShipProperties.BattleSides.Earth; } break;
        }

        for(int i=0; i<4; i++)
            turnIndicators[i].GetComponent<Image>().color = Color.white;

        turnBeginEffect[(int)(currentSide)-1].SetActive(false);
        turnBeginEffect[(int)(currentSide)-1].SetActive(true);

        sideLogoBanner[(int)(currentSide) - 1].SetActive(false);
        sideLogoBanner[(int)(currentSide) - 1].SetActive(true);

        actionsCounterForSide = 4;


        if (AsuraAI!=null)
        if (currentSide == ShipProperties.BattleSides.Asura)
            {
                //AsuraAI.solutionTries += 15;
                AsuraAI.LaunchTurn(); // isMyTurn = true;
            }

        
        if (HereticAI != null)
            if (currentSide == ShipProperties.BattleSides.Heretic)
            {
                //HereticAI.solutionTries += 15;
                HereticAI.LaunchTurn(); // isMyTurn = true;

            }
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
