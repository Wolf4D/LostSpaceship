using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TurnSystem : MonoBehaviour
{
    public ShipProperties.BattleSides currentSide = ShipProperties.BattleSides.Earth;
    public int actionsCounterForSide = 4;
    public Color disableColor;
    public MouseControl mouse;

    public GameObject[] turnIndicators;

    public GameObject[] turnBeginEffect;
    public GameObject[] sideLogoBanner;

    public bool passTurn = false;
    public float timeForPassingTurn = -1;

    public SidesStats sides;

    public AI AsuraAI;
    public AI HereticAI;

    // Start is called before the first frame update
    void Start()
    {
        sides = FindObjectOfType<SidesStats>();
        mouse = FindObjectOfType<MouseControl>();
    }

    public void OneActionMade()
    {
        if (actionsCounterForSide > 0)
            actionsCounterForSide -= 1;
        turnIndicators[actionsCounterForSide].GetComponent<Image>().color = disableColor;
        if (actionsCounterForSide == 0)
        {
            //StartNextTurn();
            StartCoroutine(NextTurn());
        }
    }

    public void StartNextTurn()
    {
        if (currentSide==ShipProperties.BattleSides.Earth)
                StartCoroutine(NextTurn());

    }

    IEnumerator NextTurn()
    {
        mouse.DropSelections();

        if (AsuraAI != null)  AsuraAI.isMyTurn = false;
        if (HereticAI != null) HereticAI.isMyTurn = false;

        sides.CheckBeacons();
        sides.CheckUnits();
        sides.CollectMoney(currentSide);

        if (currentSide!= ShipProperties.BattleSides.Earth)
            yield return new WaitForSeconds(1.5f);
        else
            yield return new WaitForSeconds(0.00001f);

        ShipProperties[] allShips = FindObjectsOfType<ShipProperties>();

        switch (currentSide)
        {
            case (ShipProperties.BattleSides.Earth): { currentSide = ShipProperties.BattleSides.Asura; } break;
            case (ShipProperties.BattleSides.Asura): { currentSide = ShipProperties.BattleSides.Heretic; } break;
            case (ShipProperties.BattleSides.Heretic): { currentSide = ShipProperties.BattleSides.Earth; } break;
        }

        // Проверка того, есть ли эта сторона на карте
        if ((currentSide== ShipProperties.BattleSides.Asura) && (AsuraAI == null))
            currentSide = ShipProperties.BattleSides.Heretic;

        if ((currentSide == ShipProperties.BattleSides.Heretic) && (HereticAI == null))
            currentSide = ShipProperties.BattleSides.Earth;

        foreach (ShipProperties shp in allShips)
        {
            if (shp.side == currentSide)
            {
                shp.hasMoved = false;
            }
        }

        for (int i=0; i<4; i++)
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
                //return;
            }

        
        if (HereticAI != null)
            if (currentSide == ShipProperties.BattleSides.Heretic)
            {
                //HereticAI.solutionTries += 15;
                HereticAI.LaunchTurn(); // isMyTurn = true;
                //return;
            }
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
