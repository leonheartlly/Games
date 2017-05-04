using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBtn : MonoBehaviour {

    [SerializeField]
    private Tower towerObject;

    [SerializeField]
    private Sprite dragSprite;

    [SerializeField]
    private int towerPrice;

    //apenas gera um getter do botão da torre específica
    public Tower TowerObject 
    {
        get
        {
            return towerObject;
        }
    }

    public Sprite DragSprite
    {
        get
        {
            return dragSprite;
        }
    }

    public int TowerPrice
    {
        get
        {
            return towerPrice;
        }
    }
}
