using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitCard : MonoBehaviour
{
    public GameObject ShipToBuy;
    public int Cost;
    public Sprite UnitSprite;
    public Text UnitName;
    public Text UnitCost;
    public Image Picture;

    // Start is called before the first frame update
    void Start()
    {
        ShipProperties ship = ShipToBuy.GetComponent<ShipProperties>();
        //Text UnitName = GetComponentsInChildren<Text>()[1];
        //Text UnitCost = GetComponentsInChildren<Text>()[2];
        //Image Picture = GetComponentInChildren<Image>();

        Picture.sprite = UnitSprite;
        UnitCost.text = Cost + "$";
        UnitName.text = ship.unitName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
