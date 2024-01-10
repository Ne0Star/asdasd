using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Спавнер с динамическим пулом, возможностью приостоновления процесса
/// </summary>
public class AdvancedSpawner : Spawner
{
    private int instantiateCount = 0;
    private List<Transform> instances;
    private List<Transform> activeList;
    private float currentSpawnTime = 0f;
    private bool isStop = false;
    private bool isPaused = true;
    private List<Vector3> pausedPositions;
    private void Awake()
    {
        currentSpawnTime = _periodicity;
        instantiateCount = 0;
        instances = new List<Transform>();
        activeList = new List<Transform>();
        //Paused();
        ReCalculateInstaces();
        isStop = false;
        isPaused = false;
    }
    public override void SetPeriodicity(float value)
    {
        base.SetPeriodicity(value);
        ReCalculateInstaces();
    }
    public override void SetSpeed(float value)
    {
        base.SetSpeed(value);
        ReCalculateInstaces();
    }
    public override void SetLength(float value)
    {
        base.SetLength(value);
        ReCalculateInstaces();
    }
    private void Update()
    {
        if (isPaused || isStop)
        {
            return;
        }
        if (currentSpawnTime >= _periodicity)
        {
            if (GetInactiveInstance(out Transform result))
            {
                activeList.Add(result);
                result.gameObject.SetActive(true);
                currentSpawnTime = 0f;
            }
        }
        for (int i = 0; i < activeList.Count; i++)
        {
            Transform instance = activeList[i];
            float distance = Vector3.Distance(transform.position, instance.position);
            if (distance < _moveLength)
            {
                instance.transform.position += _moveDirection * _moveSpeed * Time.deltaTime;
            }
            else
            {
                activeList.Remove(instance);
                instance.gameObject.SetActive(false);
                instance.position = transform.position;
            }
        }
        currentSpawnTime += Time.deltaTime;
    }
    private void ReCalculateInstaces()
    {
        int currentInstantiateCount = CalculateInstanceCount(_periodicity, _moveSpeed, _moveLength);
        if (instantiateCount < currentInstantiateCount)
        {
            for (int i = instantiateCount; i < currentInstantiateCount; i++)
            {
                Transform instance = Instantiate(_instancePrefab, transform);
                instance.position = transform.position;
                instance.gameObject.SetActive(false);
                instances.Add(instance);
            }
        }
#if UNITY_EDITOR
        // Уменьшение пула не всегда нужно
        else
        {
            for (int i = instantiateCount - currentInstantiateCount; i > 0; i--)
            {
                for (int j = 0; j < instances.Count; j++) // Удалить только незадействованные объекты
                {
                    Transform instance = instances[j];
                    if (!instance.gameObject.activeInHierarchy)
                    {
                        instances.Remove(instance);
                        Destroy(instance.gameObject);
                        break;
                    }
                }
            }
        }
#endif
        instantiateCount = currentInstantiateCount;
    }
    [ContextMenu("Пауза")]
    public void Paused()
    {
        isPaused = true;
        pausedPositions = new List<Vector3>();
        foreach (Transform instance in activeList)
        {
            pausedPositions.Add(instance.position);
        }
    }
    [ContextMenu("Продолжить")]
    public void Resume()
    {
        isStop = false;
        isPaused = false;
        if (pausedPositions != null)
        {
            for (int i = 0; i < activeList.Count; i++)
            {
                activeList[i].position = pausedPositions[i];
            }
            pausedPositions.Clear();
        }
    }
    [ContextMenu("Стоп")]
    public void Stop()
    {
        isStop = true;
        currentSpawnTime = 0f;
        for (int i = 0; i < activeList.Count; i++)
        {
            activeList[i].gameObject.SetActive(false);
            activeList[i].transform.position = transform.position;
        }
        activeList.Clear();
    }
    private bool GetInactiveInstance(out Transform inactiveObject) // В это варианте решил использовать out для упрощения
    {
        inactiveObject = null;
        for (int i = 0; i < instances.Count; i++)
        {
            if (!instances[i].gameObject.activeInHierarchy)
            {
                inactiveObject = instances[i];
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Возвращает максимальное число объектов, которые могут быть активны в указанной конфигурации
    /// </summary>
    /// <param name="spawnPeriodicity"></param>
    /// <param name="moveSpeed"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    private int CalculateInstanceCount(float spawnPeriodicity, float moveSpeed, float length)
    {
        float distancePerSpawn = spawnPeriodicity * moveSpeed; // Расстояние, которое пройдет объект за промежуток времени спавна
        return Mathf.FloorToInt(length / distancePerSpawn) + 1; // Вычисление количества объектов, учитывая расстояние и скорость
    }
}
