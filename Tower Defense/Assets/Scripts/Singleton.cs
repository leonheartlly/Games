using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null) //se o game object nao existir irá setar esta instancia.
            {
                instance = FindObjectOfType<T>(); //busca uma instancia do objeto do tipo T
            }
            else if (instance != FindObjectOfType<T>())
            {
                Destroy(FindObjectOfType<T>());
            }
            //faz com que o gameobject nao seja destruido ao criar uma nova scene.(Faz co que nao seja destruido este objeto uma vez criado)
            DontDestroyOnLoad(FindObjectOfType<T>().transform.root.gameObject);
            return instance;
        }
    }

}
