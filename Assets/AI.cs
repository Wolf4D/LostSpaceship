using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    private SidesStats stats;
    public ShipProperties.BattleSides mySide = ShipProperties.BattleSides.Asura;

    public BattleField battlefield;
    public TurnSystem turnSystem;

    public List<ShipProperties> myShips = new List<ShipProperties>();
    public List<ShipProperties> enemyShips = new List<ShipProperties>();

    public List<Beacon> myBeacons = new List<Beacon>();
    public List<Beacon> enemyBeacons = new List<Beacon>();

    public ShipProperties[] currentTaskForce = new ShipProperties[4];
    public int TaskForceCoreMinimum = 10;     // минимальная сила ядра группы
    public float TaskForcePowerMinimum = 50.0f; // Только для проверок сплочённости
    public UnitSpawner Spawner;

    public float enemyDistanceRage = 10000.0f;

    public ShipProperties nearestEnemy = null;
    public float nearestDistance = 9999999;
    public int solutionTries = 5;

    public bool isMyTurn = false;

    public enum AITasks
    {
        BuildForces = 0,
        HuntBeacons = 1,
        AttackNearest = 2
    };

    public AITasks currentTask = AITasks.AttackNearest;

    bool PerformAttack(ShipProperties ship)
    {
        if (ship.hasMoved) return false;

        Vector2 myPos = battlefield.CalcCoordsFromXYZ(ship.transform.localPosition);
        Vector2 targetPos = battlefield.CalcCoordsFromXYZ(nearestEnemy.transform.localPosition);

        Vector2 resultDirection = targetPos - myPos;

        //Debug.Log(myPos);
        //Debug.Log(targetPos);

        if (resultDirection.magnitude > ship.range)
        {
            resultDirection = resultDirection.normalized * (resultDirection.magnitude - ship.range+1); //(Random.Range(1, ship.speed-ship.range));
            Debug.Log(resultDirection);
            Vector2 newPos = myPos + resultDirection;

            if (newPos.x > battlefield.x) newPos.x = battlefield.x - 1;
            if (newPos.y > battlefield.y) newPos.y = battlefield.y - 1;
            if (newPos.x < 0) newPos.x = 0;
            if (newPos.y < 0) newPos.y = 0;

            if (battlefield.GetObjectAtCoords(newPos) != null)
            {
                newPos = myPos + resultDirection.normalized* Random.Range(1, ship.speed);
                if (battlefield.GetObjectAtCoords(newPos) != null)
                                return false;
            }

            if (newPos.x > battlefield.x) newPos.x = battlefield.x - 1;
            if (newPos.y > battlefield.y) newPos.y = battlefield.y - 1;
            if (newPos.x < 0) newPos.x = 0;
            if (newPos.y < 0) newPos.y = 0;

            battlefield.MoveObject(myPos, newPos);
            ship.Move(battlefield.transform.TransformPoint(battlefield.CalcXYZfromCoords((int)(newPos.x), (int)(newPos.y))));
            turnSystem.OneActionMade();
            return true;
        }
        else
        {
            ship.Attack(nearestEnemy);
            turnSystem.OneActionMade();
            return true;
        }
        //Debug.Log(resultDirection.magnitude);

        //Debug.Log(ship.name);
    }

    void CaptureBeacon(ShipProperties ship)
    {
        float nearDist = 999999;
        Beacon nearestEnemyBeacon = null;
        foreach (ShipProperties shp in myShips)
            {
                // Близость к маякам
                foreach (Beacon bc in enemyBeacons)
                {
                    float dist = Vector3.Distance(shp.transform.localPosition, bc.transform.localPosition);
                    if (dist < nearDist)
                    {
                        nearDist = dist;
                        nearestEnemyBeacon = bc;
                    }
                }
            }



        Vector2 myPos = battlefield.CalcCoordsFromXYZ(ship.transform.localPosition);
        Vector2 targetPos = battlefield.CalcCoordsFromXYZ(nearestEnemyBeacon.transform.localPosition);

        Vector2 resultDirection = targetPos - myPos;


            resultDirection = resultDirection.normalized * (ship.speed);
            Vector2 newPos = myPos + resultDirection;
    
            if (battlefield.GetObjectAtCoords(newPos) != null) return;


            battlefield.MoveObject(myPos, newPos);
            ship.Move(battlefield.transform.TransformPoint(battlefield.CalcXYZfromCoords((int)(newPos.x), (int)(newPos.y))));
            turnSystem.OneActionMade();
            return;
 


    }

    public void BuildUnit()
    {
        // TODO: Если негде построить - просто ход пропустим!!!
        float nearestDist=999999;
        Beacon nearestBeacon = null;
        if (nearestEnemy!=null)
        foreach (Beacon bc in myBeacons)
        {
            float dist = Vector3.Distance(nearestEnemy.transform.localPosition, bc.transform.localPosition);
            if (dist < nearestDist)
            {
                    nearestDist = dist;
                    nearestBeacon = bc;
            }
        }

        if (nearestBeacon == null)
        {
            turnSystem.OneActionMade();
            return;
        }


        int ShipNumber = Random.Range(0, 13);

        Vector2 coords = battlefield.CalcCoordsFromXYZ(nearestBeacon.transform.localPosition);
        if (battlefield.GetObjectAtCoords(coords) == null)
            Spawner.SpawnUnit(mySide, Spawner.SpawnablePrefabs[ShipNumber].ShipToBuy,
                Spawner.SpawnablePrefabs[ShipNumber].Cost, coords);

        //turnSystem.OneActionMade();

    }

    void ExecuteTask()
    {
        if (nearestEnemy == null)
            solutionTries = -1;

        if (solutionTries <= 0)
            BuildUnit();
            // построим юнита и выйдем
            // или просто выйдем

            // while (turnSystem.actionsCounterForSide>1)
            //for (int i=0; i< 4; i++)
        switch (currentTask)
        {
            case (AITasks.AttackNearest):
                {
                    if (FindTaskForce(nearestEnemy.transform.localPosition))
                    {
                        // Нашли кем атаковать!
                        int attackerNum = Random.Range(0, 4);
                        //Debug.Log(attackerNum);
                        //if (attackerNum<=3)
                        if (!currentTaskForce[attackerNum].hasMoved)
                            PerformAttack(currentTaskForce[attackerNum]);
                    }
                    else
                    {
                        if ((myShips.Count >0) && (myBeacons.Count==0))
                            PerformAttack(myShips[Random.Range(0, myShips.Count)]);
                        else
                            BuildUnit();
                    }
                } break;

            case (AITasks.HuntBeacons):
                {
                    if (nearestEnemy!=null)
                    { 
                    if (FindTaskForce(nearestEnemy.transform.localPosition))
                    {
                        // Нашли кем атаковать!
                        int attackerNum = Random.Range(0, 4);
                        if (!currentTaskForce[attackerNum].hasMoved)
                            CaptureBeacon(currentTaskForce[attackerNum]);
                    }
                    else
                        {
                            if ((myShips.Count > 0) && (myBeacons.Count == 0))
                                CaptureBeacon(myShips[Random.Range(0, myShips.Count)]);
                            else
                                BuildUnit();
                    }
                    }
                } break;
        }
        solutionTries--;

    }

    void ChoseTask()
    {
        int[] weight = new int[3];

        // Если враг близко к нашим силам или маякам - атакуем врага (2)
        // Если враг далеко, но маяков мало - отбиваем маяки (1) но это переходит в (2) по мере приближения к врагу
        // В противном случае - стоим на месте (0)

        weight[0] = 5;

        nearestEnemy = null;
        nearestDistance = 999999;

        foreach (ShipProperties shp in enemyShips)
            if (shp.isAlive==true)
        {
            // Близость к маякам
            foreach (Beacon bc in myBeacons)
            {
                float dist = Vector3.Distance(shp.transform.localPosition, bc.transform.localPosition);
                if (dist < nearestDistance)
                {
                    nearestDistance = dist;
                    nearestEnemy = shp;
                }
            }
            // Близость к нашим силам
            foreach (ShipProperties sp in myShips)
            {
                float dist = Vector3.Distance(shp.transform.localPosition, sp.transform.localPosition);
                if (dist < nearestDistance)
                {
                    nearestDistance = dist;
                    nearestEnemy = shp;
                }
            }
        }


        weight[2] = Mathf.RoundToInt(enemyDistanceRage / nearestDistance); // обратно пропорционально дистанции

        weight[1] =  5 * (enemyBeacons.Count + 1) / (myBeacons.Count + 1) ;


      //  Debug.Log(weight[0]);
      // Debug.Log(weight[1]);
     //   Debug.Log(weight[2]);

        if ((weight[0] > weight[1]) && (weight[0] > weight[2]))
            currentTask = AITasks.BuildForces;

        if ((weight[1] > weight[2]) && (weight[1] > weight[0]))
            currentTask = AITasks.HuntBeacons;

        if ((weight[2] > weight[1]) && (weight[2] > weight[0]))
            currentTask = AITasks.AttackNearest;

        /*
        // Важность каждого из решений
        int[] weight= new int[5];

        // Вес защитных и атакующих стратегий
        int defenceWeight = 0;
        int offenceWeight = 0;

        // Критерии, по которым выбирается задача
        if (myShips.Count / 2 <= enemyShips.Count)
            defenceWeight += 5;

        if (myShips.Count / 3 <= enemyShips.Count)
            defenceWeight += 10;

        if (myShips.Count / 4 <= enemyShips.Count)
        { 
            defenceWeight += 15;
            offenceWeight -= 5;
        }

        if (enemyShips.Count / 4 <= myShips.Count)
            offenceWeight += 10;

        if (enemyShips.Count / 2 <= myShips.Count)
            offenceWeight += 5;

        if (enemyShips.Count / 1.5f <= myShips.Count)
            offenceWeight += 3;

        // Взвесили сторону, теперь выберем стратегию
        */

    }


    bool FindTaskForce(Vector3 targetCoords)
    {
        // Выберем ядро ударной группы - самый тяжёлый корабль, наиболее близкий от цели.
        // Ударная группа будет строиться вокруг ядра. 

        List<ShipProperties> cores = new List<ShipProperties>();

        foreach (ShipProperties shp in myShips)
        {
            if (shp.attack >= TaskForceCoreMinimum)
                cores.Add(shp);
        }

        // Может, такого и нет Надо строить!
        if (cores.Count == 0) return false;

        // Сила ударной группы - мощь атаки всех её членов, делённая на расстояние от ядра
        float bestTaskForcePower = 0;
        List<ShipProperties> bestTaskForce = new List<ShipProperties>();

        // Теперь поищем, из чего можно собрать ударную группу поблизости
        foreach (ShipProperties core in cores)
        {
            // Найдём 3 ближайших к ядру корабля и его самого
            List<ShipProperties> newTaskForce = Find4NearestShips(myShips, core.transform.localPosition);
            //newTaskForce.Add(core);
            float newTaskForcePower = 0;

            //Debug.Log("New TaskForce is: " + newTaskForce + " with core " + core.name);

            // Оценим силу новой ударной группы
            foreach (ShipProperties tf in newTaskForce)
                newTaskForcePower += tf.attack / 
                    (1.0f + Vector3.Distance(core.transform.localPosition, 
                    tf.transform.localPosition));
            //Debug.Log("New TaskForce power by common is " + newTaskForcePower);

            // Сравним с минимальной силой, чтобы дважды не вставать
            if (newTaskForcePower < TaskForcePowerMinimum)
                newTaskForcePower = 0;

            // Это мы нашли, насколько группа компактна.
            // А теперь найдём, насколько группа далеко от цели
            newTaskForcePower /= 0.01f * Vector3.Distance(core.transform.localPosition, targetCoords);
            //Debug.Log("New TaskForce power is " + newTaskForcePower);

            if (newTaskForcePower > bestTaskForcePower)
            {
                bestTaskForcePower = newTaskForcePower;
                bestTaskForce = newTaskForce;
            }
        }

        //Debug.Log("Best TaskForce is: " + bestTaskForce);

        if (bestTaskForcePower > 0.001f)
        {
            currentTaskForce = bestTaskForce.ToArray();
            //Debug.Log("TaskForce is: ");
            //foreach (ShipProperties sp in bestTaskForce)
           //     Debug.Log(sp.name);
            return true;
        }
        else
            return false;


    }

    List<ShipProperties> Find4NearestShips(List<ShipProperties> fromWhat, Vector3 toWhat)
    {
        List<ShipProperties> result = new List<ShipProperties>();
        if (fromWhat.Count < 4) return result;

        int[] distances = new int[fromWhat.Count];
        for (int i = 0; i < fromWhat.Count; i++)
            //if (fromWhat[i]!=toWhat)
                distances[i] = Mathf.RoundToInt(1000.0f*Vector3.Distance(
                    fromWhat[i].transform.localPosition, toWhat));

        for (int i = 0; i < 4; i++)
        {
            int minValue = Mathf.Min(distances);
            int index = System.Array.IndexOf(distances, minValue);
            result.Add(fromWhat[index]);
            distances[index] = 999999;
        }

        return result;
    }

    // Start is called before the first frame update
    void Start()
    {
        stats = FindObjectOfType<SidesStats>();
        battlefield = FindObjectOfType<BattleField>();
        turnSystem = FindObjectOfType<TurnSystem>();
        Spawner = FindObjectOfType<UnitSpawner>();
    }

    public void MakeTurn()
    {
        ShipProperties[] allShips = FindObjectsOfType<ShipProperties>();
        Beacon[] allBeacons = stats.Beacons;

        myShips.Clear();
        enemyBeacons.Clear();
        myBeacons.Clear();
        enemyShips.Clear();

        foreach (ShipProperties shp in allShips)
        {
            // Кто не с нами - тот против нас...
            if (shp.side == mySide)
                myShips.Add(shp);
            else
                enemyShips.Add(shp);
        }

        foreach (Beacon bc in allBeacons)
        {
            if (bc.currentSide == mySide)
                myBeacons.Add(bc);
            else
                enemyBeacons.Add(bc);
        }

        // Пусть AI выберет задачу
        ChoseTask();
        // И выполнит её, ха-ха!
        ExecuteTask();
        //FindTaskForce(new Vector2(0, 0));
    }

    // Update is called once per frame
    void Update()
    {
//      
 //           StartCoroutine(StartTurn());
            //MakeTurn();
    }

    public void LaunchTurn()
    {
        solutionTries = 5;
        isMyTurn = true;
        StartCoroutine(StartTurn());
    }

    IEnumerator StartTurn()
    {
        //print(Time.time);
        while (isMyTurn)
        { 
            yield return new WaitForSeconds(0.5f);
            MakeTurn();
        }
        //print(Time.time);
    }
}
