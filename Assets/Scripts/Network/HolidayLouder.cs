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

    /// <summary>
    /// Корутина для скачивания и расшифровки праздников за конкретный год.
    /// </summary>
    /// <param name="year">Год, который нужно скачать (например, 2026)</param>
    /// <param name="countryCode">Код страны (например, "RU", "BY", "KZ")</param>
    /// <param name="onSuccess">Действие, которое выполнится после успешной расшифровки. Передаст готовый словарь.</param>
    public static IEnumerator FetchYearHolidays(int year, string countryCode, Action<Dictionary<DateTime, string>> onSuccess)
    {
        string url = $"https://date.nager.at/api/v3/PublicHolidays/{year}/{countryCode}";

        // ПРАВИЛЬНОЕ СОЗДАНИЕ ЗАПРОСА: Создаем объекты явно без оператора using
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        BypassCertificate certificateHandler = new BypassCertificate();
        
        // Связываем их вместе, чтобы Windows пропустила защищенное соединение
        webRequest.certificateHandler = certificateHandler;

        // Отправляем запрос в сеть
        yield return webRequest.SendWebRequest();

        // Проверка на успешность запроса
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed connection to holiday server: " + webRequest.error);
            
            // ВАЖНО: При ошибке обязательно очищаем память перед выходом!
            certificateHandler.Dispose();
            webRequest.Dispose();
            yield break;
        }

        string jsonResponse = webRequest.downloadHandler.text;
        string wrappedJson = "{ \"items\": " + jsonResponse + "}";
        JsonWrapper wrapper;

        // Расшифровываем JSON
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
                // Превращаем строку "2026-01-01" в настоящий DateTime объект
                if (DateTime.TryParse(rawItem.date, out DateTime parsedDate))
                {
                    // Сохраняем в словарь: Ключ = Дата, Значение = Название праздника
                    holidayResult[parsedDate.Date] = rawItem.localName;
                }
            }
        }

        Debug.Log($"[HolidayLoader] Успешно скачано и расшифровано праздников для {countryCode} на {year} год: {holidayResult.Count} шт.");
        onSuccess?.Invoke(holidayResult);

        // ФИНАЛЬНАЯ ОЧИСТКА: Вручную закрываем соединение и освобождаем ресурсы
        certificateHandler.Dispose();
        webRequest.Dispose();
    }
}

// Вспомогательный класс-обходчик. Находится строго за пределами HolidayLoader
public class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // Разрешаем любые SSL-сертификаты. Запрос выполнится и на Linux, и на Windows-сборке!
        return true; 
    }
}
