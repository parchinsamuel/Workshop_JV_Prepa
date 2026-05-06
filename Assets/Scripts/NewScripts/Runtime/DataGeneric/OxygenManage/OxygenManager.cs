using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OxygenManager : MonoBehaviour
{
    [Header("References")]

    [SerializeField] private Slider oxygenSlider;

    [Header("Parameters")]

    [SerializeField] private float maxOxygenValue;
    [SerializeField] private float decreaseOxygen;
    [SerializeField] private float decreaseTimer;
    private float oxygenValue;

    private void Awake()
    {
        oxygenValue = maxOxygenValue;
        oxygenSlider.minValue = 0;
        oxygenSlider.maxValue = oxygenValue;
        UpdateSlider();
    }
    void Start ()
    {
        DecreaseOxygen();
    }
    
    public void DecreaseOxygen()
    {
        StartCoroutine(DecreaseOxygenTimer());
    }
    public void UpdateSlider()
    {
        oxygenSlider.value = oxygenValue;
    }

    public IEnumerator DecreaseOxygenTimer()
    {
       yield return new WaitForSeconds(decreaseTimer);

        oxygenValue = oxygenValue - decreaseOxygen;

        UpdateSlider();

        if (oxygenValue <= 0)
        {
            Debug.Log("Game Over");
        }
        else
        {
            StartCoroutine(DecreaseOxygenTimer());
        }
    }

    public void AddOxygen()
    {
        oxygenValue = maxOxygenValue;

        UpdateSlider();
    }

    
}
