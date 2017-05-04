using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyElement
{
    FIRE = 1,
    ICE = 2,
    EARTH = 3,
    WIND = 4,
    LIGHTNING = 5
}

public class EnemyTraits
{

    [SerializeField]
    private EnemyElement element;

    [SerializeField]
    private byte fireDefense;
   
    [SerializeField]
    private string fireDefenseBinary;

    [SerializeField]
    private byte iceDefense;
    [SerializeField]
    private string iceDefenseBinary;

    [SerializeField]
    private byte earthDefense;
    [SerializeField]
    private string earthDefenseBinary;

    [SerializeField]
    private byte windDefense;
    [SerializeField]
    private string windDefenseBinary;

    [SerializeField]
    private byte lightningDefense;
    [SerializeField]
    private string lightningDefenseBinary;

    [SerializeField]
    private int healthPoints;
    [SerializeField]
    private string healthPointsBinary;

    [SerializeField]
    private int fitness;

    [SerializeField]
    private int moneyReward;

    private List<SpecialTraitsEnum> special = new List<SpecialTraitsEnum>();

    private const string max_byte_value = "11111111";
    private const string half_byte_value = "01111101";
    private const string zero = "00000000";

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

    public byte FireDefense
    {
        get
        {
            return fireDefense;
        }
    }

    public string FireDefenseBinary
    {
        get
        {
            return fireDefenseBinary;
        }
        set
        {
            fireDefenseBinary = value;
            fireDefense = findDecimalValue(fireDefenseBinary);
        }
    }

    public byte IceDefense
    {
        get
        {
            return iceDefense;
        }
    }

    public string IceDefenseBinary
    {
        get
        {
            return iceDefenseBinary;
        }
        set
        {
            iceDefenseBinary = value;
            iceDefense = findDecimalValue(iceDefenseBinary);
        }
    }

    public byte EarthDefense
    {
        get
        {
            return earthDefense;
        }
    }

    public string EarthDefenseBinary
    {
        get
        {
            return earthDefenseBinary;
        }
        set
        {
            earthDefenseBinary = value;
            earthDefense = findDecimalValue(earthDefenseBinary);
        }
    }

    public byte WindDefense
    {
        get
        {
            return windDefense;
        }
    }

    public string WindDefenseBinary
    {
        get
        {
            return windDefenseBinary;
        }
        set
        {
            windDefenseBinary = value;
            windDefense = findDecimalValue(windDefenseBinary);
        }
    }

    public byte LightningDefense
    {
        get
        {
            return lightningDefense;
        }
    }

    public string LightningDefenseBinary
    {
        get
        {
            return lightningDefenseBinary;
        }
        set
        {
            lightningDefenseBinary = value;
            lightningDefense = findDecimalValue(lightningDefenseBinary);
        }
    }

    public int HealthPoints
    {
        get
        {
            return healthPoints;
        }
    }

    public string HealthPointsBinary
    {
        get
        {
            return healthPointsBinary;
        }
        set
        {
            healthPointsBinary = value;
            healthPoints = findIntDecimalValue(healthPointsBinary);
        }
    }

    public int Aptidao
    {
        get
        {
            return fitness;
        }
        set
        {
            fitness = value;
        }
    }

    public int MoneyReward
    {
        get
        {
            return moneyReward;
        }
        set
        {
            moneyReward = value;
        }
    }

    public List<SpecialTraitsEnum> Special
    {
        get
        {
            return special;
        }
        set
        {
            special = value;
        }
    }

    #endregion


    /// <summary>Obtém a aptidão do inimigo através da soma de todos os seus atributos.</summary>
    public void calculateAptitude()
    {
        fitness = fireDefense + iceDefense + windDefense + earthDefense + lightningDefense + healthPoints;
    }

    /// <summary>Calcula a recompensa oferecida pelo inimigo baseado em sua aptidão.</summary>
    public void calculateReward()
    {
        float money = (float)(fitness * 0.005);
        moneyReward = (int)Mathf.RoundToInt(money);
    }

    /// <summary>Obtem a representação binária de um valor short.</summary>
    /// <param name="defense"></param>
    /// <returns></returns>
    public string findBinaryRepresentation(short defense)
    {
        string binaryValue = System.Convert.ToString(defense, 2);
        return roundStringBinaryCases(binaryValue);
    }

    /// <summary>Obtém a representação binária de um valor inteiro.</summary>
    /// <param name="defense">Representação decimal.</param>
    /// <returns>Representação binária de um valor inteiro.</returns>
    public string findBinaryRepresentation(int defense)
    {
        string binaryValue = System.Convert.ToString(defense, 2);
        return roundStringBinaryCases(binaryValue);
    }

    ///<c/> Efetua o arredondamento das casas binárias para 8 casas
    ///<param name="binary">Valor binário para arredondamento</param>
    ///<returns>Valor binario arredondado</returns>
    private string roundStringBinaryCases(string binary)
    {
        for (int i = binary.Length; i < 8; i++)
        {
            if (i < 8)
            {
                binary = 0 + binary;
            }
        }
        return binary;
    }

    /// <summary>
    ///  Busca o valor decimal de uma string contendo um valor binário.
    /// </summary>
    /// <param name="binary">Representação binária de um valor decimal.</param>
    /// <returns>Valor decimal em byte.</returns>
    private byte findDecimalValue(string binary)
    {
        return (byte)System.Convert.ToInt32(binary, 2);
    }

    /// <summary>
    /// Busca o valor decimal de uma string contendo uma representação binária.
    /// </summary>
    /// <param name="binary">Representação binária de um valor decimal.</param>
    /// <returns>Valor decimal em int</returns>
    private int findIntDecimalValue(string binary)
    {
        return System.Convert.ToInt32(binary, 2);
    }

    /// <summary>
    /// Aplica características especiais aos inimigos
    /// </summary>
    /// <param name="traits">características de mutação</param>
    public void specialTraits(SpecialTraitsEnum traits)
    {
        switch (traits)
        {
            case SpecialTraitsEnum.Big:
                //inserir modificador de velocidade positiva aqui 1.3
                break;
            case SpecialTraitsEnum.Small:
                //inserir modificador de velocidade negativa aqui 0.8
                break;
            case SpecialTraitsEnum.Fast:
                //inserir modificador de velocidade positiva aqui
                break;
            case SpecialTraitsEnum.Slow:
                //inserir modificador de velocidade negativa aqui
                break;
            case SpecialTraitsEnum.Hard:
                lightningDefense = (byte)(lightningDefense * 1.3);
                LightningDefenseBinary = findBinaryRepresentation(lightningDefense);
                break;
            case SpecialTraitsEnum.Soft:
                lightningDefense = (byte)(lightningDefense * 0.6);
                LightningDefenseBinary = findBinaryRepresentation(lightningDefense);
                break;
            case SpecialTraitsEnum.FireImmunity:
                FireDefenseBinary = max_byte_value;
                IceDefenseBinary = zero;
                break;
            case SpecialTraitsEnum.IceImmunity:
                IceDefenseBinary = max_byte_value;
                FireDefenseBinary = zero;
                break;
            case SpecialTraitsEnum.EarthImmunity:
                EarthDefenseBinary = max_byte_value;
                WindDefenseBinary = zero;
                break;
            case SpecialTraitsEnum.WindImmunity:
                WindDefenseBinary = max_byte_value;
                EarthDefenseBinary = zero;
                break;
            case SpecialTraitsEnum.LightningImmunity:
                LightningDefenseBinary = max_byte_value;
                break;
            case SpecialTraitsEnum.PrismaticDefense:
                FireDefenseBinary = half_byte_value;
                IceDefenseBinary = half_byte_value;
                EarthDefenseBinary = half_byte_value;
                WindDefenseBinary = half_byte_value;
                LightningDefenseBinary = half_byte_value;
                break;
            case SpecialTraitsEnum.TowerSick:
                healthPoints = (int)(healthPoints * 0.6);
                HealthPointsBinary = findBinaryRepresentation(healthPoints);
                break;
            case SpecialTraitsEnum.Fitness:
                healthPoints = (int)(healthPoints * 1.3);
                HealthPointsBinary = findBinaryRepresentation(healthPoints);
                break;
            case SpecialTraitsEnum.FireWeakness:
                fireDefense = (byte)(fireDefense * 0.6);
                FireDefenseBinary = findBinaryRepresentation(fireDefense);
                break;
            case SpecialTraitsEnum.IceWeakness:
                iceDefense = (byte)(iceDefense * 0.6);
                IceDefenseBinary = findBinaryRepresentation(iceDefense);
                break;
            case SpecialTraitsEnum.EarthWeakness:
                earthDefense = (byte)(earthDefense * 0.6);
                EarthDefenseBinary = findBinaryRepresentation(earthDefense);
                break;
            case SpecialTraitsEnum.WindWeakness:
                windDefense = (byte)(windDefense * 0.6);
                WindDefenseBinary = findBinaryRepresentation(windDefense);
                break;
            case SpecialTraitsEnum.LightningWeakness:
                lightningDefense = (byte)(lightningDefense * 0.6);
                LightningDefenseBinary = findBinaryRepresentation(lightningDefense);
                break;
            case SpecialTraitsEnum.None:
                break;
            default:
                break;
        }
    }

}
