using UnityEngine;
using UnityEngine.UI;

public class SpawnerUIController : MonoBehaviour
{
    [SerializeField] private Spawner spawner;
    [SerializeField] private InputField speedField;
    [SerializeField] private InputField lengthField;
    [SerializeField] private InputField periodicityField;

    private void Awake()
    {
        spawner = spawner == null ? FindObjectOfType<Spawner>(true) : spawner;
        speedField.onEndEdit?.AddListener((result) =>
        {
            spawner.SetSpeed(FormatValue(result));
        });
        lengthField.onEndEdit?.AddListener((result) =>
        {
            spawner.SetLength(FormatValue(result));
        });
        periodicityField.onEndEdit?.AddListener((result) =>
        {
            spawner.SetPeriodicity(FormatValue(result));
        });
        UpdateInputs();
    }
    private float FormatValue(string text)
    {
        text = text.Replace(".", ",");
        return float.Parse(text);
    }
    private void UpdateInputs()
    {
        speedField.text = spawner.Speed + "";
        lengthField.text = spawner.Length + "";
        periodicityField.text = spawner.Periodicity + "";
    }
}
