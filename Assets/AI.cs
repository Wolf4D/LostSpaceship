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
    public int TaskForceCoreMinimum = 10;
    public float TaskForcePowerMinimum = 50.0f;


    public enum AITasks
    {
        BuildForces = 0,
        //HuntBase = 1,
        HuntBeacons = 2,
        //DefendBeacons = 3,
        AttackNearest = 4
    };

    public AITasks currentTask = AITasks.AttackNearest;

    
    void ChoseTask()
    {
        // Если враг близко - атакуем врага
        // Если враг далеко, но маяков мало - отбиваем маяки
        // В противном случае - стоим на месте


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
            List<ShipProperties> newTaskForce = Find4NearestShips(myShips, core);
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

                // Это мы нашли, насколько группа компактна. А теперь найдём, насколько группа далеко от цели
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

    List<ShipProperties> Find4NearestShips(List<ShipProperties> fromWhat, ShipProperties toWhat)
    {
        List<ShipProperties> result = new List<ShipProperties>();
        if (fromWhat.Count < 4) return result;

        int[] distances = new int[fromWhat.Count];
        for (int i = 0; i < fromWhat.Count; i++)
            //if (fromWhat[i]!=toWhat)
                distances[i] = Mathf.RoundToInt(1000.0f*Vector3.Distance(
                    fromWhat[i].transform.localPosition, toWhat.transform.localPosition));

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


        FindTaskForce(new Vector2(0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
