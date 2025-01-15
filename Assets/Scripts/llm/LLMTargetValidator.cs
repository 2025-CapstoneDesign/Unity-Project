using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class LLMTargetValidator
{
        // 각 Type에 속한 target들을 정의합니다.
    private static readonly Dictionary<string, List<string>> targetGroups = new Dictionary<string, List<string>>()
{
    { "현장안전확인", new List<string> { "현장", "안전", "확인" } },
    { "보호장비 착용", new List<string> { "착용" } },
    { "의료기록지 작성", new List<string> { "의무기록지", "작성" } },
    { "AED 요청", new List<string> { "AED", "요청", "119", "신고" } },
    { "통나무굴리기", new List<string> { "등", "검사", "통나무", "구령" } },
    { "PMS확인", new List<string> { "PMS", "확인", "감각" } },
    { "맥박 호흡 확인", new List<string> { "맥박", "호흡", "확인" } },
    { "기도개방 이물질확인", new List<string> { "기도", "개방", "이물질" } },
    { "통증여부확인", new List<string> { "어디", "불편" } },
    { "통증위치확인", new List<string> { "통증", "호소" } },
    { "환부지지", new List<string> { "환부", "노출", "지지", "보조요원" } },
    { "부목길이측정", new List<string> { "부목", "길이", "측정" } },
    { "고정끈 적용", new List<string> { "고정끈", "재조정" } },
    { "붕대적용", new List<string> { "기능적", "자세", "붕대" } },
    { "공기제거", new List<string> { "공기", "제거" } },
    { "고정끈 재조정", new List<string> { "고정끈", "조정" } },
    { "암실링 길이측정", new List<string> { "암실링", "측정" } },
    { "암실링 적용", new List<string> { "암실링", "적용" } },
    { "암실링 길이 재조정", new List<string> { "암실링", "조정" } },
    { "CMS 확인", new List<string> { "CMS", "확인", "감각" } },
    { "통증확인 및 인계", new List<string> { "환자분", "다리", "통증", "상태", "보조인원" } },
    { "길이측정", new List<string> { "부목", "길이", "측정" } },
    { "당김고리 통증확인", new List<string> { "당김고리", "통증", "확인" } },
    { "고정끈 상태확인", new List<string> { "고정끈", "확인" } },
    { "통나무법 긴척추", new List<string> { "통나무", "굴리기", "긴척추판", "환자" } },
    { "장비확인 및 흡인준비", new List<string> { "장비", "확인", "흡인기", "흡인관", "생리식염수", "압력조절기", "비재호흡마스크", "산소통", "흡인기", "전원" } },
    { "흡인압력 확인", new List<string> { "압력", "재조정", "흡인", "이상" } },
    { "흡인", new List<string> { "흡인" } },
    { "흡인관 세척", new List<string> { "흡인관", "세척" } },
    { "압력조절기 조립", new List<string> { "압력조절기", "조립" } },
    { "산소 압력확인", new List<string> { "산소", "개방", "압력" } },
    { "마스크 연결", new List<string> { "비재", "호흡", "마스크", "연결" } },
    { "산소 유량조절", new List<string> { "산소", "리터" } },
    { "산소 공급", new List<string> { "산소", "공급" } },
    { "마스크 끈 재조정", new List<string> { "마스크", "끈", "재조정" } },
    { "산소 공급 완료", new List<string> { "비재", "호흡", "마스크", "제거", "유량계", "산소통" } },
    { "기도개방", new List<string> { "기도", "개방" } },
    { "자세교정", new List<string> { "자세", "교정" } },
    { "기도재개방", new List<string> { "기도", "재개방" } },
    { "AVPU", new List<string> { "환자", "상태", "눈", "자극" } },
    { "머리 DCAP-BLS 및 TIC확인", new List<string> { "머리", "DCAP", "BLS", "TIC", "확인" } },
    { "목 DCAP-BLC 및 TIC 및 JVD TD확인", new List<string> { "목", "DCAP", "BLS", "TIC", "JVD", "TD", "확인" } },
    { "경추보호대 길이측정", new List<string> { "경추보호대", "길이", "측정" } },
    { "상의 제거", new List<string> { "상의", "제거" } },
    { "가슴 확인", new List<string> { "가슴", "DCAP", "BLS", "TIC", "확인" } },
    { "기이성 운동확인", new List<string> { "기이성", "운동", "확인" } },
    { "폐음 청진", new List<string> { "폐음", "청진" } },
    { "복부확인", new List<string> { "복부", "DCAP", "BLTS", "확인" } },
    { "골반 확인", new List<string> { "하의", "신발", "제거", "DCAP", "BLS", "TIC", "확인" } },
    { "하지 확인", new List<string> { "하지", "DCAP", "BLS", "TIC", "확인", "PMS", "발" } },
    { "상지 확인", new List<string> { "상지", "DCAP", "BLS", "TIC", "확인", "PMS", "손" } },
    { "등 확인", new List<string> { "등", "DCAP", "BLS", "TIC", "확인" } },
    { "맥박호흡결과", new List<string> { "맥박", "호흡" } },
    { "보조요원CPR", new List<string> { "보조요원", "CPR" } },
    { "제세동기가동", new List<string> { "충격", "준비" } },
    { "긴척추고정판 위치", new List<string> { "긴척추고정판", "환자", "위치", "패드", "신체", "고정판" } },
    { "몸통과 다리 고정", new List<string> { "긴척추고정판", "몸통", "다리", "고정" } },
    { "머리 고정", new List<string> { "머리", "고정", "안전" } },
    { "팔다리 PMS", new List<string> { "환자", "손", "고정", "팔다리", "PMS", "확인" } }
};

    public static bool ValidateLLMWords(string type, List<string> words){
        Debug.Log($"validateLLMWords type: {type}");
        if(!targetGroups.ContainsKey(type)){
            return false;
        }

        List<string> requiredWords = targetGroups[type];

        return requiredWords.All(word => words.Contains(word));
    }
}
