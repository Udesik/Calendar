using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class HolidayLoader
{
    [Serializable]
    private class RawHolidayItem
    {
        public string date;
        public string localName;
        public string name;
    }

    [Serializable]
    private class JsonWrapper
    {
        public RawHolidayItem[] items;
    }
    
    public static IEnumerator FetchYearHolidays(int year, string countryCode, Action<Dictionary<DateTime, string>> onSuccess)
    {
        string url = $"https://date.nager.at/api/v3/PublicHolidays/{year}/{countryCode}";

        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        BypassCertificate certificateHandler = new BypassCertificate();
        
        // Связываем их вместе, чтобы Windows пропустила защищенное соединение
        webRequest.certificateHandler = certificateHandler;

        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed connection to holiday server: " + webRequest.error);
            
            certificateHandler.Dispose();
            webRequest.Dispose();
            yield break;
        }

        string jsonResponse = webRequest.downloadHandler.text;
        string wrappedJson = "{ \"items\": " + jsonResponse + "}";
        JsonWrapper wrapper;

        try
        {
            wrapper = JsonUtility.FromJson<JsonWrapper>(wrappedJson);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[HolidayLoader] Не удалось расшифровать JSON: {ex.Message}");
            
            certificateHandler.Dispose();
            webRequest.Dispose();
            yield break;
        }

        Dictionary<DateTime, string> holidayResult = new Dictionary<DateTime, string>();

        if (wrapper != null && wrapper.items != null)
        {
            foreach (var rawItem in wrapper.items)
            {
                if (DateTime.TryParse(rawItem.date, out DateTime parsedDate))
                {
                    holidayResult[parsedDate.Date] = rawItem.localName;
                }
            }
        }

        Debug.Log($"[HolidayLoader] Успешно скачано и расшифровано праздников для {countryCode} на {year} год: {holidayResult.Count} шт.");
        onSuccess?.Invoke(holidayResult);

        certificateHandler.Dispose();
        webRequest.Dispose();
    }
}

public class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true; 
    }
}
