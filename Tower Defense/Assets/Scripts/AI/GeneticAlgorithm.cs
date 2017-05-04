
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GeneticAlgorithm : MonoBehaviour
{
    #region VARIAVEIS DE CONTROLE

    private List<EnemyTraits> lastGeneration = new List<EnemyTraits>();
    private List<EnemyTraits> newGeneration = new List<EnemyTraits>();
    private List<EnemyTraits> enemyTraitCreated = new List<EnemyTraits>();
    private int enemyQuantity;
    private const int generation_quantity = 40;
    private const int half_generation = 20;
    private const string elementSymbol = "enemyElement";
    

    #endregion

    public List<EnemyTraits> selectedTrait = new List<EnemyTraits>();

    #region INICIALIZAÇÃO

    /// <summary>Cria população inicial de características de indivíduos.</summary>
    public void generateInitialPopulation()
    {
        for (int i = 0; i < 40; i++)
        {
            EnemyTraits trait = new EnemyTraits();

            trait = createInitialRandomTrait(trait);


            lastGeneration.Add(trait);
        }
    }

    /// <summary>Organiza as chamadas do algorítmo genético.</summary>
    /// <param name="isInitial">caso seja a primeira vez em que o algoritmo é chamado, cria a população inicial.</param>
    /// <returns>false</returns>
    public bool geneticAlgorithmManager(int enemyQuantity, bool isInitial)
    {

        this.enemyQuantity = enemyQuantity;
        if (isInitial)
        {
            generateInitialPopulation();

            isInitial = false;
        }

        tournamentSelection();
        crossOver();
        mutation();

        //transforma a nova geração em velha geração, dando continuidado ao processo evolutivo
        lastGeneration.Clear();
        lastGeneration.AddRange(newGeneration);

        //limpa a lista de inimigos criados na wave passada, permitindo que o mesmo entre na proxima wave
        enemyTraitCreated.Clear();

        return isInitial;
    }

    /// <summary>Cria caracteristicas iniciais da população.</summary>
    /// <param name="status">caracteristicas iniciais do inimigo</param>
    /// <returns></returns>
    private EnemyTraits createInitialRandomTrait(EnemyTraits status)
    {
        //valor decimal sendo buscado duas vezes ao gerar, transforma em binário e busca novamente..
        status.FireDefenseBinary = status.findBinaryRepresentation((byte)Random.Range(0, 100));

        status.IceDefenseBinary = status.findBinaryRepresentation((byte)Random.Range(0, 100));

        status.EarthDefenseBinary = status.findBinaryRepresentation((byte)Random.Range(0, 100));

        status.WindDefenseBinary = status.findBinaryRepresentation((byte)Random.Range(0, 100));

        status.LightningDefenseBinary = status.findBinaryRepresentation((byte)Random.Range(0, 100));

        status.HealthPointsBinary = status.findBinaryRepresentation(Random.Range(0, 300));

        status.calculateAptitude();

        status.calculateReward();

        return status;
    }

    /// <summary>
    /// Obtém o elemento do inimigo.
    /// </summary>
    /// <param name="status">característica</param>
    public void findEnemyElemental(EnemyTraits status)
    {
        int element = 1;
        int value = status.FireDefense;

        if (value < status.IceDefense)
        {
            element = 2;
            value = status.IceDefense;
        }
        if (value < status.WindDefense)
        {
            element = 4;
            value = status.WindDefense;
        }
        if (value < status.EarthDefense)
        {
            element = 3;
            value = status.EarthDefense;
        }
        if (value < status.LightningDefense)
        {
            element = 5;
            value = status.LightningDefense;
        }

        status.Element = (EnemyElement)element;
    }

    #endregion

    #region SELEÇÃO

    /// <summary>Obtém os pais selecionados para a próxima geração através de torneio.</summary>
    private void tournamentSelection()
    {
        newGeneration.Clear();
        selectedTrait.Clear();

        for (int i = 0; i < half_generation; i++)
        {
            int randomStatus = Random.Range(0, generation_quantity);
            EnemyTraits trait = lastGeneration[randomStatus];

            //os pais sao automaticamente adicionados à proxima geração
            newGeneration.Add(trait);
            //os pais selecionados
            selectedTrait.Add(trait);
        }

        Debug.Log("Geração");
    }

    #endregion

    #region CRUZAMENTO

    /// <summary>Efetua a operação de cruzamento</summary>
    private void crossOver()
    {
        for (int i = 0; i < selectedTrait.Count; i++)
        {
            childStatusParametersOrchestration(selectedTrait[i], selectedTrait[++i]);
        }
    }

    ///<code>Orquestra a chamada de cada atributo do pai ao metodo de crossover uniforme</code>
    ///<param name="firstParent">Primeiro indivíduo para cruzamento selecionado</param>
    ///<param name="secondParent">Segundo indivíduo para cruzamento selecionado</param>
    private void childStatusParametersOrchestration(EnemyTraits firstParent, EnemyTraits secondParent)
    {
        EnemyTraits firstChild = new EnemyTraits();
        EnemyTraits secondChild = new EnemyTraits();

        string[] fireDefense = uniformCrossOver(firstParent.FireDefenseBinary, secondParent.FireDefenseBinary);
        firstChild.FireDefenseBinary = fireDefense[0];
        secondChild.FireDefenseBinary = fireDefense[1];

        string[] iceDefense = uniformCrossOver(firstParent.IceDefenseBinary, secondParent.IceDefenseBinary);
        firstChild.IceDefenseBinary = iceDefense[0];
        secondChild.IceDefenseBinary = iceDefense[1];

        string[] earthDefense = uniformCrossOver(firstParent.EarthDefenseBinary, secondParent.EarthDefenseBinary);
        firstChild.EarthDefenseBinary = earthDefense[0];
        secondChild.EarthDefenseBinary = earthDefense[1];

        string[] windDefense = uniformCrossOver(firstParent.WindDefenseBinary, secondParent.WindDefenseBinary);
        firstChild.WindDefenseBinary = windDefense[0];
        secondChild.WindDefenseBinary = windDefense[1];

        string[] physicalDefense = uniformCrossOver(firstParent.LightningDefenseBinary, secondParent.LightningDefenseBinary);
        firstChild.LightningDefenseBinary = physicalDefense[0];
        secondChild.LightningDefenseBinary = physicalDefense[1];

        string[] health = uniformCrossOver(firstParent.HealthPointsBinary, secondParent.HealthPointsBinary);
        firstChild.HealthPointsBinary = health[0];
        secondChild.HealthPointsBinary = health[1];

        firstChild.calculateAptitude();
        secondChild.calculateAptitude();

        firstChild.calculateReward();
        secondChild.calculateReward();

        if (firstParent.Special.Count > 0)
        {
            inheritParentSpecialTrait(firstParent, firstChild, secondChild);
        }
        if (secondParent.Special.Count > 0)
        {
            inheritParentSpecialTrait(secondParent, firstChild, secondChild);
        }

        //adiciona na lista de usuários da nova geração
        newGeneration.Add(firstChild);
        newGeneration.Add(secondChild);
    }

    /// <summary>Efetua teste para descobrir se o filho irá herdar atributos do pai </summary>
    /// <param name="parent">Pai</param>
    /// <param name="firstChild">Primeiro filho</param>
    /// <param name="secondChild">Segundo filho</param>
    private void inheritParentSpecialTrait(EnemyTraits parent, EnemyTraits firstChild, EnemyTraits secondChild)
    {
        foreach (SpecialTraitsEnum trait in parent.Special)
        {
            int firstChildTrait = Random.Range(0, 100);
            if (firstChildTrait < 50)
            {
                firstChild.Special.Add(trait);
            }
            int secondChildTrait = Random.Range(0, 100);
            if (secondChildTrait < 50)
            {
                secondChild.Special.Add(trait);
            }
        }
    }

    ///<summary> Operação de cruzamento usando metodologia uniforme</summary>
    ///<param name="parent1">Atributo de cruzamento genérico do pai 1</param>
    ///<param name="parent2">Atributo de cruzamento genérico do pai 2</param>
    ///<returns> array de duas posições contendo um status genérico</returns>
    private string[] uniformCrossOver(string parent1, string parent2)
    {
        string firstSon = "";
        string secondSon = "";
        int genIndex = 0;

        if (parent1.Length < parent2.Length)
        {
            int size = parent2.Length - parent1.Length;
            parent1 = arrangeBinaryHealthByPair(parent1, size);
        }
        else
        {
            int size = parent1.Length - parent2.Length;
            parent2 = arrangeBinaryHealthByPair(parent2, size);
        }

        foreach (char gen in parent1)
        {
            int selectedParent = Random.Range(0, 2);
            char parent1GenAtIndex = parent1[genIndex];
            char parent2GenAtIndex = parent2[genIndex];

            if (selectedParent == 0)
            {
                firstSon = firstSon + parent1GenAtIndex;
                secondSon = secondSon + parent2GenAtIndex;
            }
            else
            {
                firstSon = firstSon + parent2GenAtIndex;
                secondSon = secondSon + parent1GenAtIndex;
            }
            genIndex++;
        }

        string[] childStatus = new string[2];
        childStatus[0] = firstSon;
        childStatus[1] = secondSon;

        return childStatus;
    }

    /// <summary>Iguala o tamanho do binario do par selecionado para o atributo pontos de vida</summary>
    /// <param name="parent">Pai com o tamanho menor</param>
    /// <param name="size">Diferença de tamanho</param>
    /// <returns>Pai com tamanho igualado</returns>
    private string arrangeBinaryHealthByPair(string parent, int size)
    {
        for (int i = 0; i < size; i++)
        {
            parent = "0" + parent;
        }

        return parent;
    }

    #endregion

    #region MUTAÇÃO

    /// <summary>Efetua mutanção dos indivíduos.</summary>
    private void mutation()
    {
        foreach (EnemyTraits enemy in newGeneration)
        {
            mutationCheck(enemy.FireDefenseBinary);
            mutationCheck(enemy.IceDefenseBinary);
            mutationCheck(enemy.EarthDefenseBinary);
            mutationCheck(enemy.WindDefenseBinary);
            mutationCheck(enemy.LightningDefenseBinary);
            mutationCheck(enemy.HealthPointsBinary);

            int specialTraitChance = Random.Range(0, 100);

            //chance de conseguir uma mutação especial de 1%
            if (specialTraitChance <= 1)
            {
                int trait = Random.Range(1, 20);
                SpecialTraitsEnum mutationTrait = (SpecialTraitsEnum)trait;
                enemy.Special.Add(mutationTrait);
            }

            //caso o inimigo tenha uma mutação especial aplica seus efeitos.
            if (enemy.Special.Count > 0)
            {
                foreach (SpecialTraitsEnum mod in enemy.Special)
                {
                    enemy.specialTraits(mod);
                }
            }

            enemy.calculateAptitude();
        }
    }

    /// <summary>Verifica e aplica a mutação em um indivíduo gen a gen</summary>
    /// <param name="attribute">Código genético de um indivíduo </param>
    /// <returns>código genético mutado</returns>
    private string mutationCheck(string attribute)
    {
        string mutatedTrait = "";
        foreach (char gen in attribute)
        {
            int chance = Random.Range(0, 100);

            if (chance <= 1)
            {
                if (gen.Equals("0"))
                {
                    mutatedTrait += "1";
                }
                else
                {
                    mutatedTrait += "0";
                }
            }
            else
            {
                mutatedTrait += gen;
            }
        }
        return mutatedTrait;
    }

    #endregion

    #region FINALIZAÇÃO

    /// <summary>Efetua a instanciação de indivíduos a serem usados em tela.</summary>
    /// <param name="spawnPoint">Ponto de partida do inimigo</param>
    /// <param name="enemies">modelos de inimigos a instanciar</param>
    /// <param name="enemiesPerSpawn">quantidade de inimigos no round</param>
    /// <param name="enemyNumber"></param>
    /// <returns></returns>
    public Enemy instantiateEnemies(GameObject spawnPoint, Enemy[] enemies, int enemiesPerSpawn, int enemyNumber)
    {
        for (int i = 0; i < enemiesPerSpawn; i++)
        {
            Enemy enemy = Instantiate(enemies[Random.Range(0, enemyQuantity)]) as Enemy;
            enemy.transform.position = spawnPoint.transform.position;

            //Ordena a lista de inimigos selecionados através da aptidao
            selectedTrait = selectedTrait.OrderBy(o => o.Aptidao).ToList();

            checkGeminiEnemy(selectedTrait[enemyNumber], enemyNumber, enemy);

            return enemy;
        }
        return null;
    }

    /// <summary>Converte propriedades de cada indivíduo aplicando os valores gerados.</summary>
    /// <param name="enemy">indivíduo criado</param>
    /// <param name="status">status de conversão</param>
    private void enemyPropertiesConversor(Enemy enemy, EnemyTraits status)
    {
        enemy.FireDefense = status.FireDefense;
        enemy.FireBinary = status.FireDefenseBinary;

        enemy.IceDefense = status.IceDefense;
        enemy.IceBinary = status.IceDefenseBinary;

        enemy.EarthDefense = status.EarthDefense;
        enemy.EarthBinary = status.EarthDefenseBinary;

        enemy.WindDefense = status.WindDefense;
        enemy.WindBinary = status.WindDefenseBinary;

        enemy.LightningDefense = status.LightningDefense;
        enemy.LightningBinary = status.LightningDefenseBinary;

        enemy.HealthPoints = status.HealthPoints;

        enemy.SpecialTraits.AddRange(status.Special);
        findEnemyElemental(status);

        enemy.Element = status.Element;
        applyEnemyElement(enemy);

        enemy.Fitness = status.Aptidao;
        enemy.MoneyReward = status.MoneyReward;

    }

    /// <summary>Evita que inimigos duplicados sejam criados na mesma wave</summary>
    /// <param name="trait">característica selecionada</param>
    /// <param name="val">posição</param>
    /// <param name="enemy">figura do inimigo a ser criado</param>
    private void checkGeminiEnemy(EnemyTraits trait, int val, Enemy enemy)
    {
        bool gemini = false;
        if (enemyTraitCreated.Count > 0)
        {
            gemini = enemyTraitCreated.Contains(trait);
        }

        if (gemini)
        {
            while (gemini)
            {
                int traitPosition = Random.Range(0, selectedTrait.Count);

                trait = selectedTrait[traitPosition];
                gemini = enemyTraitCreated.Contains(trait);
            }
        }

        //evita que dois indivíduos com as mesmas caracteristicas sejam criados em tela
        enemyTraitCreated.Add(trait);

        enemyPropertiesConversor(enemy, trait);
    }

    /// <summary>Aplica o icone de elemento ao inimigo.</summary>
    /// <param name="enemy"></param>
    public void applyEnemyElement(Enemy enemy)
    {
        Transform childTransform = null;
        switch (enemy.Element)
        {

            case EnemyElement.FIRE:
                childTransform = enemy.gameObject.transform.Find(elementSymbol);
                childTransform.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("fogos");
                break;
            case EnemyElement.ICE:
                childTransform = enemy.gameObject.transform.Find(elementSymbol);
                childTransform.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("ices");
                break;
            case EnemyElement.EARTH:
                childTransform = enemy.gameObject.transform.Find(elementSymbol);
                childTransform.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("earths");
                break;
            case EnemyElement.WIND:
                childTransform = enemy.gameObject.transform.Find(elementSymbol);
                childTransform.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("ventos");
                break;
            default:
                childTransform = enemy.gameObject.transform.Find(elementSymbol);
                childTransform.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("raios");
                break;
        }
    }

    #endregion

}
