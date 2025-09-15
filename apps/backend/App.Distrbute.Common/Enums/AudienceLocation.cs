using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace App.Distrbute.Common.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum AudienceLocation
{
    Default = 0,

    // Greater Accra Region
    [EnumMember(Value = "Accra, Ghana")] AccraGhana,
    [EnumMember(Value = "Tema, Ghana")] TemaGhana,
    [EnumMember(Value = "Madina, Ghana")] MadinaGhana,

    [EnumMember(Value = "Ashaiman, Ghana")]
    AshaimanGhana,

    // Ashanti Region
    [EnumMember(Value = "Kumasi, Ghana")] KumasiGhana,
    [EnumMember(Value = "Ejisu, Ghana")] EjisuGhana,
    [EnumMember(Value = "Obuasi, Ghana")] ObuasiGhana,

    // Bono Region
    [EnumMember(Value = "Sunyani, Ghana")] SunyaniGhana,
    [EnumMember(Value = "Berekum, Ghana")] BerekumGhana,

    // Eastern Region
    [EnumMember(Value = "Koforidua, Ghana")]
    KoforiduaGhana,
    [EnumMember(Value = "Nkawkaw, Ghana")] NkawkawGhana,

    // Central Region
    [EnumMember(Value = "Cape Coast, Ghana")]
    CapeCoastGhana,
    [EnumMember(Value = "Kasoa, Ghana")] KasoaGhana,

    // Western Region
    [EnumMember(Value = "Takoradi, Ghana")]
    TakoradiGhana,
    [EnumMember(Value = "Tarkwa, Ghana")] TarkwaGhana,

    // Northern Region
    [EnumMember(Value = "Tamale, Ghana")] TamaleGhana,
    [EnumMember(Value = "Yendi, Ghana")] YendiGhana,

    // Volta Region
    [EnumMember(Value = "Ho, Ghana")] HoGhana,
    [EnumMember(Value = "Keta, Ghana")] KetaGhana,

    // Upper West
    [EnumMember(Value = "Wa, Ghana")] WaGhana,

    // Upper East
    [EnumMember(Value = "Bolgatanga, Ghana")]
    BolgatangaGhana,

    // University Towns (cross-region targeting)
    [EnumMember(Value = "Legon Campus, Accra")]
    LegonCampus,

    [EnumMember(Value = "KNUST Campus, Kumasi")]
    KnustCampus,

    [EnumMember(Value = "UCC Campus, Cape Coast")]
    UccCampus,

    [EnumMember(Value = "Sunyani Technical Campus")]
    SunyaniTechCampus
}