using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CurrencyApp : MonoBehaviour
{
    public Text currencyText;
    public Text timerText; 
    public Button refreshButton; 
    private const string apiUrl = "https://api.currencylayer.com/live"; //API
    private const string accessKey = "3cd885f669e2d8b92c86251c2d0307a8"; //API key
    private float refreshInterval = 300f; // 5 min
    private float timer;

    void Start()
    {
        timer = refreshInterval;
        refreshButton.onClick.AddListener(UpdateCurrencyManually);
        StartCoroutine(UpdateCurrency()); // start game with async func
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            StartCoroutine(UpdateCurrency());
            timer = refreshInterval;
        }
        UpdateTimerUI();
    }

    IEnumerator UpdateCurrency() //async func for update currency
    {
        string url = $"{apiUrl}?access_key={accessKey}&currencies=EUR&source=USD&format=1";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) //error checker
            {
                Debug.LogError("Ошибка: " + request.error);
                currencyText.text = "Ошибка загрузки данных";
            }
            else
            {
                ProcessCurrencyData(request.downloadHandler.text);
            }
        }
    }

    void ProcessCurrencyData(string json)
    {
        try
        {
            var data = JsonUtility.FromJson<CurrencyData>(json); 
            float usdToEur = data.quotes.USDEUR;
            currencyText.text = $"USD/EUR: {usdToEur:F2}";
        }
        catch
        {
            currencyText.text = "Ошибка обработки данных";
        }
    }

    void UpdateCurrencyManually()
    {
        StartCoroutine(UpdateCurrency());
        timer = refreshInterval;
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        timerText.text = $"Следующее обновление через: {minutes:00}:{seconds:00}";
    }
}

//classes for currensy

[System.Serializable]
public class CurrencyData
{
    public Quotes quotes;
}

[System.Serializable]
public class Quotes
{
    public float USDEUR;
}
