using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Não possui start ou update pois esta classe sera responsável apenas por garantir uma unica instanica de gamemanager.
 * 
 * */
public class Loader : MonoBehaviour {

    public GameObject gameManager;


    /*
     * Singleton.
     * 
     * */
    void Awake()
    {

        KeyValuePair<int, string> course = new KeyValuePair<int, string>(1, "developr");
        course.print();
    }
        
        
    }


public class KeyValuePair<TKey, Tvalue> {

    public TKey key;
    public Tvalue value;

    public KeyValuePair(TKey key, Tvalue value)
    {
        this.key = key;
        this.value = value;
    }

    public void print()
    {
        Debug.Log("Key: " + key.ToString());
        Debug.Log("Value: " + value.ToString());
    }

}

    /** Explicação generics
     *  printAny<int>(45);
     *  printAny<string>("This is cool");
    public void printAny<T> (T value)
    {
        Debug.Log("Value" + value.ToString());
    }
	*/

