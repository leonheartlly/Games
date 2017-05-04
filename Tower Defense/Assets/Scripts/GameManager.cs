using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum gameStatus
{
    NEXT, PLAY, GAMEOVER, WIN
}

public class GameManager : Singleton<GameManager>
{

    #region Variaveis Serializadas

    //isto irá fazer com que o campo pareca publico apenas para esta classe, tendo o efeito privado para qualquer outra
    [SerializeField]
    private GameObject spawnPoint;
    [SerializeField]
    private Enemy[] enemies;

    //quantidade máxima de inimigos que poderá ser chamada no level
    [SerializeField]
    private int totalEnemies = 3;
    //inimigos invocados por wave
    [SerializeField]
    private int enemiesPerSpawn;
    [SerializeField]
    private int totalWaves = 10;
    [SerializeField]
    private Text totalMoneyLabel;
    [SerializeField]
    private Text currentWaveLabel;
    [SerializeField]
    private Text totalEscapedLbl;
    [SerializeField]
    private Text playBtnLbl;
    [SerializeField]
    private Button playBtn;

    #endregion

    #region Variaveis privadas 
    //usada antes de criar a lista
    // private int enemiesOnScreen = 0;
    private int waveNumber = 0;
    private int totalMoney = 10;
    private int totalEscaped = 0;
    private int roundEscaped = 0;
    private int totalKilled = 0;
    private int whichEnemiesToSpawn = 0;
    private gameStatus currentState = gameStatus.PLAY;
    private int enemiesToSpawn;

    private AudioSource audioSource;

    const float spawnDelay = 0.5f;
    public List<Enemy> enemyList = new List<Enemy>();
    private bool isInitial = true;

    private GeneticAlgorithm geneticAlgorithm;

    #endregion

    #region GETTERS E SETTERS

    public int TotalMoney
    {
        get
        {
            return totalMoney;
        }
        set
        {
            totalMoney = value;
            totalMoneyLabel.text = totalMoney.ToString();
        }
    }

    public int TotalEscaped
    {
        get
        {
            return totalEscaped;
        }
        set
        {
            totalEscaped = value;
        }
    }

    public int RoundEscaped
    {
        get
        {
            return roundEscaped;
        }
        set
        {
            roundEscaped = value;
        }
    }

    public int TotalKilled
    {
        get
        {
            return totalKilled;
        }
        set
        {
            totalKilled = value;
        }
    }

    public Enemy[] Enemies
    {
        get
        {
            return this.enemies;
        }
        set
        {
            this.enemies = value;
        }
    }

    public AudioSource AudioSource
    {
        get
        {
            return audioSource;
        }
    }

    #endregion

     /// <summary>Garantia de que o objeto criado não será apagado.</summary>
    void Awake()
    {

    }

    /// <summary>Método de inicialização.</summary>
    void Start()
    {
        //spawnEnemy() inicia chamando inimigo, removido após criação da ui.
        // StartCoroutine(spawn());

        //fará com que o botão desapareça da tela ao iniciar a partida.
        playBtn.gameObject.SetActive(false);

        geneticAlgorithm = new GeneticAlgorithm();


        audioSource = GetComponent<AudioSource>();

        showMenu();

    }

    /// <summary>Metodo update será realizado a cada ciclo.</summary>
    void Update()
    {
        //permite o jogador cancelar uma compra
        handleCancel();
    }

    /// <summary>Invocação de monstros.</summary>
    /// <returns></returns>
    IEnumerator spawn()
    {
        //se a quantidade de inimigos por spawn for maior que 0 e a quantidade de inimigos na tela for menor que o total de inimigo
        if (enemiesPerSpawn > 0 && enemyList.Count < totalEnemies)
        {
            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                if (enemyList.Count < totalEnemies)
                {

                    geneticAlgorithm.instantiateEnemies(spawnPoint, enemies, enemiesPerSpawn, enemyList.Count);

                    // registerEnemy(newEnemy);
                    //(enemies[Random.Range(0, enemiesToSpawn)]) as Enemy; //as Gameobjet é apenas um cast (0 neste caso é o numero do modelo de inimigo do array)
                }
            }

            yield return new WaitForSeconds(spawnDelay); // segura o retorno por uma quantidade de segundos e em seguida inicia um coroutine
            StartCoroutine(spawn());
        }
    }

    #region Tratativas lista de inimigos

    ///<summary>
    ///Registra um inimigo na tela
    ///</summary>
    ///<param name="enemy">Objeto inimigo</param>
    public void registerEnemy(Enemy enemy)
    {
        enemyList.Add(enemy);
    }

    ///<summary>
    ///Retira um inimigo registrado da lista.
    ///</summary>
    ///<param name="enemy">Objeto inimigo</param>
    public void unregisterEnemy(Enemy enemy)
    {
        enemyList.Remove(enemy);
        //faz com que um inimigo que morreu ou escapou tenha seu objeto destruido.
        Destroy(enemy.gameObject);
    }

    /** Destroi todos os inimigos da lista.
     * */
    public void destroyAllEnemies()
    {
        foreach (Enemy enemy in enemyList)
        {
            Destroy(enemy.gameObject);
        }
        //limpa a lista após remover todos os inimigos.
        enemyList.Clear();
    }
    #endregion

    /// <summary>Adiciona dinheiro ao jogador</summary>
    /// <param name="amount">Quantidade a adicionar</param>
    public void addMoney(int amount)
    {
        TotalMoney += amount;
    }

    /// <summary>Retira dinheiro do jogador</summary>
    /// <param name="amount">Quantidade a subtrair</param>
    public void subtractMoney(int amount)
    {
        TotalMoney -= amount;
    }

    ///<summary>Verifica se a wave de inimigos está encerrada.</summary>
    public void isWaveOver()
    {
        totalEscapedLbl.text = "Escaped " + TotalEscaped + "/10";

        //se o total de inimigos que escaparam + o total de inimigos mortos forem igual ao total de inimigos gerados na wave.
        if ((RoundEscaped + TotalKilled) == totalEnemies)
        {
            if (waveNumber <= enemies.Length)
            {
                enemiesToSpawn = waveNumber;
            }
            //verifica o estado do jogo a cada iteração.
            setCurrentGameState();
            showMenu();
        }
    }

    /// <summary>Atribui o estado atual do jogo.</summary>
    public void setCurrentGameState()
    {
        if (TotalEscaped >= 10)
        {
            currentState = gameStatus.GAMEOVER;
        }
        else if (waveNumber == 0 && (TotalKilled + RoundEscaped) == 0)
        {
            currentState = gameStatus.PLAY;
        }
        else if (waveNumber >= totalWaves)
        {
            currentState = gameStatus.WIN;
        }
        else
        {
            currentState = gameStatus.NEXT;
        }
    }

    /// <summary>Mostra o menu inicial.</summary>
    public void showMenu()
    {
        switch (currentState)
        {
            case gameStatus.GAMEOVER:
                playBtnLbl.text = "Play Again!";

                //adiciona som de gameover
                AudioSource.PlayOneShot(SoundManager.Instance.GameOver);
                break;
            case gameStatus.NEXT:
                playBtnLbl.text = "Next Wave!";
                break;
            case gameStatus.PLAY:
                playBtnLbl.text = "Play";
                break;
            case gameStatus.WIN:
                playBtnLbl.text = "Play";
                break;
        }
        playBtn.gameObject.SetActive(true);
    }

    ///<summary>Efetua ação de chamar inimigos ao apertar o botão iniciar.</summary>
    public void playBtnPressed()
    {
        switch (currentState)
        {
            case gameStatus.NEXT:
                waveNumber += 1;
                totalEnemies += waveNumber;

                geneticAlgorithm.geneticAlgorithmManager(enemiesToSpawn, isInitial);
                break;
            default:
                totalEnemies = 3;
                totalEscaped = 0;
                TotalMoney = 10;
                enemiesToSpawn = 0;
                TowerManager.Instance.DestroyAllTowers();
                TowerManager.Instance.renameTagsBuildSites();
                totalMoneyLabel.text = TotalMoney.ToString();
                totalEscapedLbl.text = "Escaped " + totalEscaped + "/10";
                isInitial = true;
                //invoca o som
                //playoneshot ira tocar vários audios ao mesmo tempo
                audioSource.PlayOneShot(SoundManager.Instance.NewGame);
                isInitial = geneticAlgorithm.geneticAlgorithmManager(enemiesToSpawn, isInitial);
                break;
        }
        destroyAllEnemies();
        TotalKilled = 0;
        RoundEscaped = 0;
        currentWaveLabel.text = "Wave " + (waveNumber);
        //invoca inimigos após setado o botão
        StartCoroutine(spawn());
        playBtn.gameObject.SetActive(false);
    }

    ///<summary>Quando o player aperta esc ou o botão direito do mouse, cancela a compra de uma torre.</summary>
    public void handleCancel()
    {
        //tecla, inpu.getkeydown(keycode.escape) //esc nesse caso Input.GetMouseButton(1) || 
        if (Input.GetMouseButton(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            TowerManager.Instance.disableDragSprite();
            TowerManager.Instance.TowerBtnPressed = null;
        }
    }

}
