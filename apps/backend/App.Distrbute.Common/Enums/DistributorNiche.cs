using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace App.Distrbute.Common.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum DistributorNiche
{
    Default = 0,

    [EnumMember(Value = "Entertainment & Pop Culture")]
    EntertainmentAndPopCulture,

    [EnumMember(Value = "Beauty & Fashion")]
    BeautyAndFashion,

    [EnumMember(Value = "Food & Drink")]
    FoodAndDrink,

    [EnumMember(Value = "Technology")]
    Technology,

    [EnumMember(Value = "Gaming")]
    Gaming,

    [EnumMember(Value = "Education & Careers")]
    EducationAndCareers,

    [EnumMember(Value = "Sports & Fitness")]
    SportsAndFitness,

    [EnumMember(Value = "Music & Arts")]
    MusicAndArts,

    [EnumMember(Value = "Travel & Culture")]
    TravelAndCulture,

    [EnumMember(Value = "Social Good & Advocacy")]
    SocialGoodAndAdvocacy,

    [EnumMember(Value = "Business & Finance")]
    BusinessAndFinance,

    [EnumMember(Value = "Parenting & Family")]
    ParentingAndFamily,

    [EnumMember(Value = "Home & DIY")]
    HomeAndDIY,

    [EnumMember(Value = "Health & Wellness")]
    HealthAndWellness
}