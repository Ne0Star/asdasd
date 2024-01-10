using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Неоптимизированный вариант спавнера, однако этот вариант делался во много раз быстрее чем Advanced
/// </summary>
public class StandartSpawner : Spawner
{
    // Я думал использовать корутину, однако как по мне это излишне для такой задачи
    private int maxCount = 300;
    private float currentSpawnTime;
    private List<Transform> inactiveInstance;
    private List<Transform> activeInstance;
    private void Awake()
    {
        currentSpawnTime = _periodicity;
        inactiveInstance = new List<Transform>();
        activeInstance = new List<Transform>();
        for (int i = 0; i < maxCount; i++)
        {
            Transform t = Instantiate(_instancePrefab, transform);
            t.gameObject.SetActive(false);
            inactiveInstance.Add(t);
        }
    }
    private void Update()
    {
        if (currentSpawnTime >= _periodicity)
        {
            Transform t = GetInactiveInstance();
            if (t != null)
            {
                activeInstance.Add(t);
                t.gameObject.SetActive(true);
            }
            currentSpawnTime = 0f;
        }
        for (int i = 0; i < activeInstance.Count; i++)
        {
            Transform instance = activeInstance[i];
            float distance = Vector3.Distance(transform.position, instance.position);
            if (distance >= _moveLength)
            {
                activeInstance.Remove(instance);
                instance.gameObject.SetActive(false);
                instance.position = transform.position;
            }
            else
            {
                instance.position += _moveDirection * _moveSpeed * Time.deltaTime;
            }
        }
        currentSpawnTime += Time.deltaTime;
    }
    private Transform GetInactiveInstance()
    {
        for (int i = 0; i < inactiveInstance.Count; i++)
        {
            Transform t = inactiveInstance[i];
            if (!t.gameObject.activeSelf)
            {
                return t;
            }
        }
        return null;
    }
}
