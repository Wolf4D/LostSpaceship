using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    private SidesStats stats;
    public ShipProperties.BattleSides mySide = ShipProperties.BattleSides.Asura;

    public List<ShipProperties> myShips = new List<ShipProperties>();
    public List<ShipProperties> enemyShips = new List<ShipProperties>();

    public List<Beacon> myBeacons = new List<Beacon>();
    public List<Beacon> enemyBeacons = new List<Beacon>();

    public ShipProperties[] currentTaskForce = new ShipProperties[4];
    public int TaskForceCoreMinimum = 10;     // минимальная сила ядра группы
    public float TaskForcePowerMinimum = 50.0f; // Только для проверок сплочённости

    public float enemyDistanceRage = 10000.0f;



    public enum AITasks
    {
        BuildForces = 0,
        //HuntBase = 1,
        HuntBeacons = 1,
        //DefendBeacons = 3,
        AttackNearest = 2
    };

    public AITasks currentTask = AITasks.AttackNearest;

    
    void ChoseTask()
    {
        int[] weight = new int[3];

        // Если враг близко к нашим силам или маякам - атакуем врага (2)
        // Если враг далеко, но маяков мало - отбиваем маяки (1)
        // В противном случае - стоим на месте (0)

        weight[0] = 5;

        ShipProperties nearestEnemy = null;
        float nearestDistance = 9999999;
        foreach (ShipProperties shp in enemyShips)
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
                    nearestEnemy = sp;
                }
            }
        }


        weight[1] = Mathf.RoundToInt(enemyDistanceRage / nearestDistance); // обратно пропорционально дистанции

        weight[2] =  10 * (enemyBeacons.Count + 1) / (myBeacons.Count + 1) ;


        Debug.Log(weight[0]);
        Debug.Log(weight[1]);
        Debug.Log(weight[2]);
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

            Debug.Log("New TaskForce is: " + newTaskForce + " with core " + core.name);

            // Оценим силу новой ударной группы
            foreach (ShipProperties tf in newTaskForce)
                newTaskForcePower += tf.attack / 
                    (1.0f + Vector3.Distance(core.transform.localPosition, 
                    tf.transform.localPosition));
            Debug.Log("New TaskForce power by common is " + newTaskForcePower);

            // Сравним с минимальной силой, чтобы дважды не вставать
            if (newTaskForcePower < TaskForcePowerMinimum)
                newTaskForcePower = 0;

            // Это мы нашли, насколько группа компактна.
            // А теперь найдём, насколько группа далеко от цели
            newTaskForcePower /= 0.01f * Vector3.Distance(core.transform.localPosition, targetCoords);
            Debug.Log("New TaskForce power is " + newTaskForcePower);

            if (newTaskForcePower > bestTaskForcePower)
            {
                bestTaskForcePower = newTaskForcePower;
                bestTaskForce = newTaskForce;
            }
        }

        Debug.Log("Best TaskForce is: " + bestTaskForce);

        if (bestTaskForcePower > 0.001f)
        {
            currentTaskForce = bestTaskForce.ToArray();
            Debug.Log("TaskForce is: ");
            foreach (ShipProperties sp in bestTaskForce)
                Debug.Log(sp.name);
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

        ChoseTask();
        //FindTaskForce(new Vector2(0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
