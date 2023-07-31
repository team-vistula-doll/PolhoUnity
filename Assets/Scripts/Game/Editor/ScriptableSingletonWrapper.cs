using UnityEngine;
using UnityEditor;

public abstract class ScriptableSingletonWrapper<T> : ScriptableSingleton<ScriptableSingletonWrapper<T>> where T : ScriptableObject
{
    private new static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = CreateInstance<T>();
            }
            return instance;
        }
    }
}
