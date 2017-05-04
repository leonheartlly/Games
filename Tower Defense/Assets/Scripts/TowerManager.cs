using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class TowerManager : Singleton<TowerManager>
{

    //botao usado no gui.
    // public towerbtn towerbtnpresssed{get;set;} geraria o getter e setter do botao automatico
    private TowerBtn towerBtnPressed;

    private SpriteRenderer spriteRenderer;

    private List<Tower> towerList = new List<Tower>();
    private List<Collider2D> buildList = new List<Collider2D>();
    private Collider2D buildTile;

    public TowerBtn TowerBtnPressed
    {
        get
        {
            return towerBtnPressed;
        }
        set
        {
            towerBtnPressed = value;
        }
    }


    // Use this for initialization
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        buildTile = GetComponent<Collider2D>();
        spriteRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Este é o unico local onde é possível obter o click do jogador
        if (Input.GetMouseButtonDown(0))//0 é o botado da esquerda, 1 o botado da direita
        {
            //a camera pega a posição do mundo. //input sabe onde é a posição do mouse
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //encontra o ponto clicado
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            //permite que a torre seja colocada apenas onde existe a tag buildsite 
            //OBS : neste ponto verá que a torre está sendo posta abaixo do local, deverá setar o pivot de center para o botton (sprite)
            if (hit.collider.tag == "BuildSite")
            {
                buildTile = hit.collider;
                //renomeia a tag para que nao permita duas torres no mesmo local
                buildTile.tag = "buildSiteFull";
                //registra o buildtile selecionado para a lista
                registerBuildSite(buildTile);
                placeTower(hit);
            }
        }

        //cria o objeto do tipo da torre para seguir o mouse
        if (spriteRenderer.enabled)
        {
            followMouse();
        }

    }

    public void registerBuildSite(Collider2D buildTag)
    {
        buildList.Add(buildTag);
    }

    public void registerTower(Tower tower)
    {
        towerList.Add(tower);
    }

    public void renameTagsBuildSites()
    {
        foreach (Collider2D buildTag in buildList)
        {
            buildTag.tag = "BuildSite";
        }
        buildList.Clear();
    }

    public void DestroyAllTowers()
    {
        foreach (Tower tower in towerList)
        {
            Destroy(tower.gameObject);
        }
        towerList.Clear();
    }

    public void placeTower(RaycastHit2D hit)
    {
        if (!EventSystem.current.IsPointerOverGameObject() && towerBtnPressed != null && GameManager.Instance.TotalMoney >= 10)
        {
            //cria um objeto do objeto filho do botão correspondente.(ou seja uma torre correspondente ao botao)
            Tower newTower = Instantiate(towerBtnPressed.TowerObject) as Tower;

            //informa a posição em que o objeto sera posicionado.
            newTower.transform.position = hit.transform.position;

            //som de por torre
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.TowerBuilt);

            buyTower(towerBtnPressed.TowerPrice);
            registerTower(newTower);

            //desativa o sprite assim que a torre for colocada
            disableDragSprite();
        }
    }

    public void buyTower(int price)
    {
        GameManager.Instance.subtractMoney(price);
    }

    public void selectedTower(TowerBtn towerSelected) //obtém o botão selecionado
    {
        if (towerSelected.TowerPrice <= GameManager.Instance.TotalMoney)
        {
            towerBtnPressed = towerSelected;
            // Debug.Log("Pressed: " + towerBtnPressed.gameObject);

            enableDragSprite(towerSelected.DragSprite);
        }
    }

    public void followMouse()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(transform.position.x, transform.position.y);
    }

    /** habilita o sprite da torre no mouse ao clicar no botão.
     * 
     * 
     * */
    public void enableDragSprite(Sprite sprite)
    {
        //neste ponto lembre-se de setar um sprite renderer no botao
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 10;
        //se não funcionar, lembre-se de ir ao tower manager e setar o layer do sprite para aparecer
    }

    /** Desabilita o sprite da torre no mouse após gerar ou cancelar a torre.
     * 
     * */
    public void disableDragSprite()
    {
        spriteRenderer.enabled = false;
    }
}
