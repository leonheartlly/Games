using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Controla ações da torre.
 * 
 * 
 * */
public class Tower : MonoBehaviour
{

    //Tempo entre um ataque e outro.
    [SerializeField]
    private float timeBetweenAttacks;

    //distancia em que a torre irá atacar.
    [SerializeField]
    private float attackRadius;

    //projetil da torre.
    [SerializeField]
    private Projectile projectile;


    private Enemy targetEnemy = null;
    private float attackCounter;
    private bool isAttacking = false;


    void Start()
    {

    }

    void Update()
    {
        //time.deltaTime regula a velocidade uma vez que cada computador tem uma velocidade de processamento, isto fará com que a velocidade seja a mesma emq ualquer pc
        attackCounter -= Time.deltaTime;
        if (targetEnemy == null || targetEnemy.IsDead)
        {
            Enemy nearestEnemy = getNearestEnemyInRange();
            //verifica se a lista de inimigos não está vazia e se o inimigo encontrado está no alcance da torre.
            if (getNearestEnemyInRange() != null && Vector2.Distance(transform.localPosition, nearestEnemy.transform.localPosition) <= attackRadius)
            {
                //seta o inimigo que será alvo.
                targetEnemy = nearestEnemy;
            }
        }
        else
        {
            //sempre que o tempo entre os ataques se passar(timeBetweenAttacks)
            if (attackCounter <= 0)
            {
                isAttacking = true;
                //reseta o attackcounter
                attackCounter = timeBetweenAttacks;
            }
            else
            {
                isAttacking = false;
            }

            if (Vector2.Distance(transform.localPosition, targetEnemy.transform.localPosition) > attackRadius)
            {
                targetEnemy = null;
            }
        }
        }

    void FixedUpdate()
    {
        if (isAttacking)
        {
            attack();
        }
    }


    public void attack()
    {
        //seta a permissao para atacar para falso para nao atacar novamente.
        isAttacking = false;
        //instancia um projétil
        Projectile newProjectile = Instantiate(projectile) as Projectile;
        //seta a posição inicial do projetil para a posição da torre.
        newProjectile.transform.localPosition = transform.localPosition;

        //som dos projeteis
        if (newProjectile.ProjectileType == proType.ARROW)
        {
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Arrow);
        }
        else if (newProjectile.ProjectileType == proType.FIREBALL)
        {
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Fireball);
        }
        else if (newProjectile.ProjectileType == proType.ROCK)
        {
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Rock);
        }

        if (targetEnemy == null)
        {
            Destroy(newProjectile);
        }
        else
        {
            //move o projetil para o inimigo.
            StartCoroutine(moveProjectile(newProjectile));
        }
    }

    /** IEnumerator é usado toda vez que for necessário um courotine.
     * */
    IEnumerator moveProjectile(Projectile projectile)
    {
        //entender o 0.20f aula 47 em 17:48
        while (getTargetDistance(targetEnemy) > 0.20f && projectile != null && targetEnemy != null)
        {
            //o unity pode lançar a flexa na na posição errada fazendo com que o contato com o inimigo seja com o corpo da flexa, nao a ponta, o codigo abaixo corrige isto
            //obtém a direção do inimigo.
            var dir = targetEnemy.transform.localPosition - transform.localPosition;

            //neste caso a variável dir será uma posição com angulo y e x //rad2deg apenas converte radian para degree
            var angleDirection = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            //apenas cuida da rotação da flexa
            projectile.transform.rotation = Quaternion.AngleAxis(angleDirection, Vector3.forward);

            //localposition é a posição do projetil, nao a posição global do mesmo
            //movetoward fará com que o projetil se mova do local onde está, até o local onde o inimigo está
            //o valor mágico é a velocidade de movimento do projétil
            projectile.transform.localPosition = Vector2.MoveTowards(projectile.transform.localPosition, targetEnemy.transform.localPosition, 5f * Time.deltaTime);

            yield return null;
        }

        //caso não encontre inimigo  e o projétil tenha sido lançado, o projétil é destruido
        if (projectile != null || targetEnemy == null)
        {
            Destroy(projectile);
        }
    }

    /// <summary>
    /// Obtém a distância do alvo.
    /// </summary>
    /// <param name="thisEnemy">Objeto alvo</param>
    /// <returns></returns>
    private float getTargetDistance(Enemy thisEnemy)
    {
        if (thisEnemy == null)
        {
            thisEnemy = getNearestEnemyInRange();
            //se a lista de inimigos também for nula, não há inimigos
            if (thisEnemy == null)
            {
                return 0f;
            }
        }

        //mathf.absolute indpendente se o número for negativo ou positivo, irá retornar um valor positivo.
        return Mathf.Abs(Vector2.Distance(transform.localPosition, thisEnemy.transform.localPosition));
    }

    /** Busca inimigos na area de alcance.
     * */
    private List<Enemy> getEnemiesInRange()
    {
        List<Enemy> enemiesInRange = new List<Enemy>();
        foreach (Enemy enemy in GameManager.Instance.enemyList)
        {
            //obtem a distancia entre a torre e o inimigo e verifica se está dentro do alcance da torre.
            if (Vector2.Distance(transform.localPosition, enemy.transform.localPosition) <= attackRadius)
            {
                enemiesInRange.Add(enemy);
            }
        }
        return enemiesInRange;
    }


    /**Devolve o inimigo de menor distancia em comparação com a torre.
     * */
    private Enemy getNearestEnemyInRange()
    {
        Enemy nearestEnemy = null;
        //obtém um valor positivo de comparação infinito.
        float smallestDistance = float.PositiveInfinity;

        //obtem a lista de inimigos que estão no alcance
        foreach (Enemy enemy in getEnemiesInRange())
        {
            //compara a posição da torre com a posição do inimigo
            if (Vector2.Distance(transform.localPosition, enemy.transform.localPosition) <= smallestDistance)
            {
                //iguala a menor posição encontrada garantindo buscar o inimigo de menor posição
                smallestDistance = Vector2.Distance(transform.localPosition, enemy.transform.localPosition);
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
    }
}
