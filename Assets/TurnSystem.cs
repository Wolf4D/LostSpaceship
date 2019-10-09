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
    public int turnCount=0;
    public SidesStats sides;

    public bool catchUp = false;
    public bool wasAnyPlayersMove = false; // делал ли плеер что-либо с последнего хода?

    public AI AsuraAI;
    public AI HereticAI;

    bool DeadlockGuardIsRunning = false;

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

        if (currentSide==ShipProperties.BattleSides.Earth)
            wasAnyPlayersMove = false;

        foreach (ShipProperties shp in allShips)
        {
            if (shp.side == currentSide)
            {
                shp.hasMoved = false;
            }
        }

        if (catchUp)
            if (sides.MoneyOfSide[(int)(currentSide)] > sides.MoneyOfSide[(int)(ShipProperties.BattleSides.Earth)])
                sides.MoneyOfSide[(int)(currentSide)] = sides.MoneyOfSide[(int)(ShipProperties.BattleSides.Earth)];

                    for (int i=0; i<4; i++)
            turnIndicators[i].GetComponent<Image>().color = Color.white;

        turnBeginEffect[(int)(currentSide)-1].SetActive(false);
        turnBeginEffect[(int)(currentSide)-1].SetActive(true);

        sideLogoBanner[(int)(currentSide) - 1].SetActive(false);
        sideLogoBanner[(int)(currentSide) - 1].SetActive(true);

        actionsCounterForSide = 4;
        turnCount++;

        StartCoroutine(DeadlockGuard());

        if (AsuraAI!=null)
        if (currentSide == ShipProperties.BattleSides.Asura)
            {
                //AsuraAI.solutionTries += 15;
                if (wasAnyPlayersMove)
                    AsuraAI.LaunchTurn(); // isMyTurn = true;
                else
                    StartNextTurn();
                //return;
            }

        
        if (HereticAI != null)
            if (currentSide == ShipProperties.BattleSides.Heretic)
            {
                //HereticAI.solutionTries += 15;
                if (wasAnyPlayersMove)
                    HereticAI.LaunchTurn(); // isMyTurn = true;
                else
                    StartNextTurn();

                //return;
            }

    }

    IEnumerator DeadlockGuard()
    {
        DeadlockGuardIsRunning = true;
        yield return new WaitForSeconds(5.5f);
        if (actionsCounterForSide == 4)
        switch (currentSide)
        {
            case (ShipProperties.BattleSides.Asura):
                    {
                        if (AsuraAI != null)
                            if (AsuraAI.isMyTurn == false)
                            {
                                currentSide = ShipProperties.BattleSides.Earth;
                                //AsuraAI.LaunchTurn(); //isMyTurn = true;
                                //OneActionMade(); // NextTurn();
                                                 //StartNextTurn();
                            }
                    } break;
            case (ShipProperties.BattleSides.Heretic):
                    {
                        if (HereticAI != null)
                            if (HereticAI.isMyTurn == false)
                                currentSide = ShipProperties.BattleSides.Earth;
                        //OneActionMade();
                        //NextTurn();
                        //HereticAI.LaunchTurn();//isMyTurn = true;
                        //StartNextTurn();
                    } break;
        }

        DeadlockGuardIsRunning = false;
    }

        // Update is called once per frame
        void Update()
    {
        if ((DeadlockGuardIsRunning == false) && (currentSide != ShipProperties.BattleSides.Earth))
            StartCoroutine(DeadlockGuard());

    }
}
