using System;
using System.Collections.Generic;
using UnityEngine;

public static class JsonHelper
{
    /// <summary>
    /// JSON 데이터를 받아 target 리스트를 추출하는 함수
    /// </summary>
    public static List<string> ExtractTargets(string json)
    {
        try
        {
            // JSON 데이터를 ParsedData 객체로 변환
            ParsedData parsedData = JsonUtility.FromJson<ParsedData>(json);

            // target 리스트 반환
            if (parsedData != null && parsedData.target != null)
            {
                return new List<string>(parsedData.target);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error parsing JSON: {e.Message}");
        }

        return new List<string>(); // 오류 발생 시 빈 리스트 반환
    }

    // JSON 데이터 구조 정의
    [Serializable]
    private class ParsedData
    {
        public string sentence; // JSON의 "sentence" 필드
        public string[] target; // JSON의 "target" 배열
    }
}
