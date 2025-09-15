using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace App.Distrbute.Common.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum AudiencePersona
{
    Default = 0,

    // Students
    [EnumMember(Value = "SHS Students")] SHSStudents,

    [EnumMember(Value = "University Students")]
    UniversityStudents,

    [EnumMember(Value = "Postgrad Students")]
    PostgradStudents,

    // Professionals
    [EnumMember(Value = "Health Professionals")]
    HealthProfessionals, // Doctors, Nurses, Pharmacists

    [EnumMember(Value = "Legal Professionals")]
    LegalProfessionals, // Lawyers, Paralegals

    [EnumMember(Value = "Tech Professionals")]
    TechProfessionals, // Developers, Engineers, IT Specialists

    [EnumMember(Value = "Finance Professionals")]
    FinanceProfessionals, // Accountants, Bankers, Traders

    [EnumMember(Value = "Creative Professionals")]
    CreativeProfessionals, // Artists, Designers, Musicians, DJs

    [EnumMember(Value = "Media And Communications")]
    MediaAndCommunications, // Journalists, Influencers, PR
    [EnumMember(Value = "Educators")] Educators, // Teachers, Lecturers, Tutors

    [EnumMember(Value = "Skilled Trades People")]
    SkilledTradesPeople, // Masons, Electricians, Mechanics, Tailors
    [EnumMember(Value = "Civil Servants")] CivilServants, // Government employees
    [EnumMember(Value = "Entrepreneurs")] Entrepreneurs,

    // General Segments
    [EnumMember(Value = "Religious Leaders")]
    ReligiousLeaders,

    [EnumMember(Value = "Stay At Home Parents")]
    StayAtHomeParents,

    [EnumMember(Value = "Corporate Workers")]
    CorporateWorkers, // Office professionals, HR, Admin, etc.
    [EnumMember(Value = "Others")] Others
}