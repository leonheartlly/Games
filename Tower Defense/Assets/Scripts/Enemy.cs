using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour
{
    #region Variaveis serialized
    [SerializeField]
    private Transform exitPoint;
    [SerializeField]
    private Transform[] waypoints;
    [SerializeField]
    private float navigationUpdate;//testar para entender melhor
    [SerializeField]
    private int healthPoint;
    [SerializeField]
    private int moneyReward;
    #endregion

    #region variaveis locais
    private int target = 0;
    private Transform enemy;//localizaçao do inimigo
    private Animator anim;
    private Collider2D enemyCollider;
    private float navigationTime = 0;
    private bool isDead = false;
    #endregion

    #region atributos de defesa

    [SerializeField]
    private EnemyElement element;
    [SerializeField]
    private int fireDefense;
    [SerializeField]
    private string fireBinary;
    [SerializeField]
    private int iceDefense;
    [SerializeField]
    private string iceBinary;
    [SerializeField]
    private int earthDefense;
    [SerializeField]
    private string earthBinary;
    [SerializeField]
    private int windDefense;
    [SerializeField]
    private string windBinary;
    [SerializeField]
    private int lightningDefense;
    [SerializeField]
    private string lightningBinary;

    [SerializeField]
    private int fitness;

    [SerializeField]
    private EnemyTraits status;

    [SerializeField]
    private List<SpecialTraitsEnum> specialTraits = new List<SpecialTraitsEnum>();

    #endregion

    #region Getters e Setters

    public EnemyElement Element
    {
        get
        {
            return element;
        }
        set
        {
            element = value;
        }
    }

    public int FireDefense
    {
        get
        {
            return this.fireDefense;
        }
        set
        {
            this.fireDefense = value;
        }
    }

    public string FireBinary
    {
        get
        {
            return this.fireBinary;
        }

        set
        {
            this.fireBinary = value;
        }
    }

    public int IceDefense
    {
        get
        {
            return this.iceDefense;
        }
        set
        {
            this.iceDefense = value;
        }
    }

    public string IceBinary
    {
        get
        {
            return this.iceBinary;
        }
        set
        {
            this.iceBinary = value;
        }
    }

    public int EarthDefense
    {
        get
        {
            return this.earthDefense;
        }
        set
        {
            this.earthDefense = value;
        }
    }

    public string EarthBinary
    {
        get
        {
            return this.EarthBinary;
        }
        set
        {
            this.earthBinary = value;
        }
    }

    public int WindDefense
    {
        get
        {
            return this.windDefense;
        }
        set
        {
            this.windDefense = value;
        }
    }

    public string WindBinary
    {
        get
        {
            return this.windBinary;
        }
        set
        {
            this.windBinary = value;
        }
    }

    public int LightningDefense
    {
        get
        {
            return this.lightningDefense;
        }

        set
        {
            this.lightningDefense = value;
        }
    }

    public string LightningBinary
    {
        get
        {
            return this.lightningBinary;
        }

        set
        {
            this.lightningBinary = value;
        }
    }

    public int Fitness
    {
        get
        {
            return this.fitness;
        }
        set
        {
            this.fitness = value;
        }
    }

    public int HealthPoints
    {
        get
        {
            return this.healthPoint;
        }
        set
        {
            this.healthPoint = value;
        }
    }

    public int MoneyReward
    {
        get
        {
            return this.moneyReward;
        }
        set
        {
            this.moneyReward = value;
        }
    }

    public EnemyTraits Status
    {
        get
        {
            return status;
        }
        set
        {
            status = value;
        }
    }

    public List<SpecialTraitsEnum> SpecialTraits
    {
        get
        {
            return specialTraits;
        }
        set
        {
            specialTraits = value;
        }
    }

    public bool IsDead
    {
        get
        {
            return isDead;
        }
    }

    #endregion

    void Start()
    {
        //obtem o componente transform do inimigo
        enemy = GetComponent<Transform>();

        //obtém o colisor do inimigo.
        enemyCollider = GetComponent<Collider2D>();

        //registra um inimigo criado
        GameManager.Instance.registerEnemy(this);

        //obtem o componente de animação do inimigo.
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (waypoints != null && !isDead)
        {
            //obtem o tempo passado
            navigationTime += Time.deltaTime;
            if (navigationTime > navigationUpdate)
            {
                if (target < waypoints.Length)
                {
                    //significa que pode mover a posicao do inimigo
                    enemy.position = Vector2.MoveTowards(enemy.position, waypoints[target].position, navigationTime); //atualiza a posição do inimigo
                }
                else
                {
                    enemy.position = Vector2.MoveTowards(enemy.position, exitPoint.position, navigationTime);
                }
                navigationTime = 0;
            }
        }

       /* if (Input.GetMouseButtonDown(0))//0 é o botado da esquerda, 1 o botado da direita
        {
            //a camera pega a posição do mundo. //input sabe onde é a posição do mouse
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit ht;
            //encontra o ponto clicado
         /*   RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero);

            if (Physics.Raycast(ray, ht))
            {
                enemyHeal(200);
            }
        }*/
    }
  

    ///<summary>Quando on trigger do colisor é marcado, este método é usado para checar caso um objeto se colida com outro.</summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Checkpoint")
        {
            target += 1;
        }
        else if (other.tag == "Finish")
        {
            GameManager.Instance.RoundEscaped += 1;
            GameManager.Instance.TotalEscaped += 1;
            GameManager.Instance.unregisterEnemy(this);
            //verifica se acabaram os inimigos da wave e se o jogo deverá continuar.
            GameManager.Instance.isWaveOver();
        }
        else if (other.tag == "projectile")
        {
            //busca o componente projétil
            Projectile otherProjectile = other.gameObject.GetComponent<Projectile>();

            //obtem o dano do projétil e subtrai do life do inimigo.
            if (otherProjectile != null)
            {
                enemyHit(otherProjectile.AttackStrength);
                Destroy(other.gameObject);
            }
        }
        else if (other.tag == "FireProjectile") //ELEMENTO FOGO
        {
            //busca o componente projétil
            Projectile otherProjectile = other.gameObject.GetComponent<Projectile>();

            //obtem o dano do projétil e subtrai do life do inimigo.
            if (otherProjectile != null)
            {
                calculateElementalDamage(50, EnemyElement.FIRE);

                Destroy(other.gameObject);
            }
        }
        else if (other.tag == "WaterProjectile") //ELEMENTO AGUA
        {
            Projectile otherProjectile = other.gameObject.GetComponent<Projectile>();

            if (otherProjectile != null)
            {
                calculateElementalDamage(50, EnemyElement.ICE);

                Destroy(other.gameObject);
            }
        }
        else if (other.tag == "EarthProjectile") //ELEMENTO TERRA
        {
            Projectile otherProjectile = other.gameObject.GetComponent<Projectile>();

            if (otherProjectile != null)
            {
                calculateElementalDamage(50, EnemyElement.EARTH);

                Destroy(other.gameObject);
            }
        }
        else if (other.tag == "WindProjectile") //ELEMENTO VENTO
        {
            Projectile otherProjectile = other.gameObject.GetComponent<Projectile>();

            if (otherProjectile != null)
            {
                calculateElementalDamage(50, EnemyElement.WIND);

                Destroy(other.gameObject);
            }
        }
        else if (other.tag == "LightningProjectile") //ELEMENTO RAIO
        {
            Projectile otherProjectile = other.gameObject.GetComponent<Projectile>();

            if (otherProjectile != null)
            {
                calculateElementalDamage(50, EnemyElement.LIGHTNING);

                Destroy(other.gameObject);
            }
        }
    }

    /// <summary>Obtém a porcentagem do valor elemental obtido pelo inimigo.</summary>
    /// <param name="elementalDefense">Atributo de defesa elemental</param>
    /// <returns>porcentagem</returns>
    private int findElementalDefensePercent(int elementalDefense)
    {
        int element = elementalDefense * 100;
        return element / byte.MaxValue;
    }

    /// <summary>Obtém o dano elemental através da porcentagem.</summary>
    /// <param name="percent"> porcentagem</param>
    /// <param name="projectileStrength"> força do projétil</param>
    /// <returns>dano</returns>
    private int findDamageByPercent(int percent, int projectileStrength)
    {
        return (projectileStrength * percent) / 100;
    }

    ///<summary>Obtém o dano levado pelo inimigo.</summary>
    ///<param name="hitPoints"> pontos de vida do inimigo</param>
    public void enemyHit(int hitPoints)
    {
        if (healthPoint - hitPoints > 0)
        {
            healthPoint -= hitPoints;

            //som de hit
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Hit);

            //chama animação de levar dano
            anim.Play("Hurt");
        }
        else
        {
            //busca a trigger da animação de morrer
            anim.SetTrigger("didDie");
            die();
        }
    }

    /// <summary>Adiciona pontos de vida ao inimigo.</summary>
    /// <param name="heal">quantidade de cura</param>
    public void enemyHeal(int heal)
    {
        if (healthPoint > 0)
        {
            healthPoint += heal;

            //som de hit
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Hit);

            //chama animação de levar dano
            anim.Play("Hurt");
        }
        if (healthPoint > 255)
        {
            this.gameObject.transform.localScale += new Vector3(0.2f, 0.2f, 1);
        }
    }

    /// <summary>Efetua operação para a morte de um inimigo.</summary>
    public void die()
    {
        isDead = true;
        enemyCollider.enabled = false;
        //adiciona um a quantidade de inimigos mortos
        GameManager.Instance.TotalKilled += 1;
        //som
        GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Death);

        //adiciona a quantidade de dinheiro do monstro ao total do player
        GameManager.Instance.addMoney(moneyReward);
        //verifica se a wave acabou 
        GameManager.Instance.isWaveOver();
    }

    /// <summary>Calcula o dano elemental ao inimigo</summary>
    /// <param name="damage">dano a ser aplicado</param>
    /// <param name="damageType">tipo do dano</param>
    public void calculateElementalDamage(int damage, EnemyElement damageType)
    {
        //elemento do inimigo
        switch (this.element)
        {
            case EnemyElement.FIRE:
                #region FIRE DAMAGE TAKEN
                if (EnemyElement.FIRE == damageType)
                {
                    int percent = findElementalDefensePercent(fireDefense);
                    damage += findDamageByPercent(percent, damage);

                    int mitigatedDamage = damage / 2;

                    enemyHeal(mitigatedDamage);
                }
                else if (EnemyElement.ICE == damageType)
                {
                    int percent = findElementalDefensePercent(iceDefense);
                    damage += findDamageByPercent(percent, damage);
                    int doubleDamage = damage * 2;
                    enemyHit(doubleDamage);
                }
                else if (EnemyElement.EARTH == damageType)
                {
                    int percent = findElementalDefensePercent(earthDefense);
                    damage += findDamageByPercent(percent, damage);

                    enemyHit(damage);
                }
                else if (EnemyElement.WIND == damageType)
                {
                    int percent = findElementalDefensePercent(windDefense);
                    damage += findDamageByPercent(percent, damage);
                    damage = (int)(damage * 1.25);
                    enemyHit(damage);
                }
                else if (EnemyElement.LIGHTNING == damageType)
                {
                    int percent = findElementalDefensePercent(windDefense);
                    damage += findDamageByPercent(percent, damage);
                    damage = damage / 2;
                    enemyHit(damage);
                }

                #endregion
                break;
            case EnemyElement.ICE:
                #region ICE DAMAGE TAKEN
                if (EnemyElement.FIRE == damageType)
                {
                    int percent = findElementalDefensePercent(fireDefense);
                    damage += findDamageByPercent(percent, damage);

                    int doubleDamage = damage * 2;
                    enemyHit(doubleDamage);
                }
                else if (EnemyElement.ICE == damageType)
                {
                    int percent = findElementalDefensePercent(iceDefense);
                    damage += findDamageByPercent(percent, damage);

                    int mitigatedDamage = damage / 2;
                    enemyHeal(mitigatedDamage);
                }
                else if (EnemyElement.EARTH == damageType)
                {
                    int percent = findElementalDefensePercent(earthDefense);
                    damage += findDamageByPercent(percent, damage);

                    damage = (int)(damage * 1.25);
                    enemyHit(damage);
                }
                else if (EnemyElement.WIND == damageType)
                {
                    int percent = findElementalDefensePercent(windDefense);
                    damage += findDamageByPercent(percent, damage);
                    enemyHit(damage);
                }
                else if (EnemyElement.LIGHTNING == damageType)
                {
                    int percent = findElementalDefensePercent(windDefense);
                    damage += findDamageByPercent(percent, damage);
                    damage = damage / 2;
                    enemyHit(damage);
                }

                #endregion
                break;
            case EnemyElement.EARTH:
                #region EARTH DAMAGE TAKEN
                if (EnemyElement.FIRE == damageType)
                {
                    int percent = findElementalDefensePercent(fireDefense);
                    damage += findDamageByPercent(percent, damage);

                    enemyHit(damage);
                }
                else if (EnemyElement.ICE == damageType)
                {
                    int percent = findElementalDefensePercent(iceDefense);
                    damage += findDamageByPercent(percent, damage);

                    damage = (int)(damage * 1.25);
                    enemyHit(damage);
                }
                else if (EnemyElement.EARTH == damageType)
                {
                    int percent = findElementalDefensePercent(earthDefense);
                    damage += findDamageByPercent(percent, damage);

                    damage = damage / 2;
                    enemyHeal(damage);
                }
                else if (EnemyElement.WIND == damageType)
                {
                    int percent = findElementalDefensePercent(windDefense);
                    damage += findDamageByPercent(percent, damage);
                    enemyHit(damage * 2);
                }
                else if (EnemyElement.LIGHTNING == damageType)
                {
                    int percent = findElementalDefensePercent(windDefense);
                    damage += findDamageByPercent(percent, damage);
                    damage = damage / 2;
                    enemyHit(damage);
                }

                #endregion
                break;
            case EnemyElement.WIND:
                #region WIND DAMAGE TAKEN
                if (EnemyElement.FIRE == damageType)
                {
                    int percent = findElementalDefensePercent(fireDefense);
                    damage += findDamageByPercent(percent, damage);

                    damage = (int)(damage * 1.25);
                    enemyHit(damage);
                }
                else if (EnemyElement.ICE == damageType)
                {
                    int percent = findElementalDefensePercent(iceDefense);
                    damage += findDamageByPercent(percent, damage);
                    enemyHit(damage);
                }
                else if (EnemyElement.EARTH == damageType)
                {
                    int percent = findElementalDefensePercent(earthDefense);
                    damage += findDamageByPercent(percent, damage);

                    enemyHit(damage * 2);
                }
                else if (EnemyElement.WIND == damageType)
                {
                    int percent = findElementalDefensePercent(windDefense);
                    damage += findDamageByPercent(percent, damage);
                    enemyHeal(damage / 2);
                }
                else if (EnemyElement.LIGHTNING == damageType)
                {
                    int percent = findElementalDefensePercent(windDefense);
                    damage += findDamageByPercent(percent, damage);

                    enemyHit(damage / 2);
                }

                #endregion
                break;
            case EnemyElement.LIGHTNING:
                #region LIGHTNING DAMAGE TAKEN
                if (EnemyElement.LIGHTNING == damageType)
                {
                    int percent = findElementalDefensePercent(fireDefense);
                    damage += findDamageByPercent(percent, damage);

                    enemyHit(damage * 2);
                }
                else
                {
                    int percent = findElementalDefensePercent(windDefense);
                    damage += findDamageByPercent(percent, damage);
                    damage = damage / 2;
                    enemyHit(damage);
                }


                #endregion
                break;

        }
    }

    public void testMouseClick()
    {
       /* if (EventSystem.current.IsPointerOverGameObject())
        {

        }*/
    }
}
