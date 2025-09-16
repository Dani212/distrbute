using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace App.Distrbute.Common.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum BrandNiche
{
    Default = 0,

    [EnumMember(Value = "Beauty & Fashion")]
    BeautyAndFashion,

    [EnumMember(Value = "Food & Drink")] FoodAndDrink,

    [EnumMember(Value = "Travel")] Travel,

    [EnumMember(Value = "Fitness")] Fitness,

    [EnumMember(Value = "Lifestyle")] Lifestyle,

    [EnumMember(Value = "Technology")] Technology,

    [EnumMember(Value = "Gaming")] Gaming,

    [EnumMember(Value = "Parenting")] Parenting,

    [EnumMember(Value = "Health")] Health,

    [EnumMember(Value = "Business")] Business,

    [EnumMember(Value = "Finance")] Finance,

    [EnumMember(Value = "Education")] Education,

    [EnumMember(Value = "Music")] Music,

    [EnumMember(Value = "Art")] Art,

    [EnumMember(Value = "Photography")] Photography,

    [EnumMember(Value = "Automotive")] Automotive,

    [EnumMember(Value = "Sports")] Sports,

    [EnumMember(Value = "Pets")] Pets,

    [EnumMember(Value = "DIY & Crafts")] DIY,

    [EnumMember(Value = "Books")] Books,

    [EnumMember(Value = "Movies & TV")] Movies,

    [EnumMember(Value = "Entrepreneurship")]
    Entrepreneurship,

    [EnumMember(Value = "Relationships & Dating")]
    Relationships,

    [EnumMember(Value = "Sustainability")] Sustainability
}