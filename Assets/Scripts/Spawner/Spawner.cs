using UnityEngine;

public abstract class Spawner : MonoBehaviour
{
    [SerializeField] protected Transform _instancePrefab;
    [SerializeField] protected float _periodicity;
    [SerializeField] protected float _moveLength;
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected Vector3 _moveDirection;
    public float Speed { get => _moveSpeed; }
    public float Length { get => _moveLength; }
    public float Periodicity { get => _periodicity; }
    public virtual void SetSpeed(float value)
    {
        this._moveSpeed = value;
    }
    public virtual void SetLength(float value)
    {
        this._moveLength = value;
    }
    public virtual void SetPeriodicity(float value)
    {
        this._periodicity = value;
    }
}
