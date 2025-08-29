using UnityEngine;
using System.Collections.Generic;

public class AnimalManager : MonoBehaviour
{
    private static AnimalManager _instance;
    public static AnimalManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AnimalManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<AnimalManager>();
                    singletonObject.name = typeof(AnimalManager).ToString() + " (Singleton)";
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    public List<Animal> activeAnimals = new List<Animal>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void AddAnimal(Animal animal)
    {
        if (!activeAnimals.Contains(animal))
        {
            activeAnimals.Add(animal);
        }
    }

    public void RemoveAnimal(Animal animal)
    {
        if (activeAnimals.Contains(animal))
        {
            activeAnimals.Remove(animal);
        }
    }
}