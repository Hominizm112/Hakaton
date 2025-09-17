using UnityEngine;
using System.Collections.Generic;
public class CompanyIssuer
{
    public static Dictionary<string, Company> CompanyInfo = new Dictionary<string, Company>() {
    // Финансовый сектор
    {"GDC", new Company { Name = "Golden Dragon Capital", Sector = CompanySector.Financial,Country=CompanyCountry.China }},
    {"SRV", new Company { Name = "Silk Road Ventures", Sector = CompanySector.Financial,Country=CompanyCountry.Germany }},

    // Промышленный сектор
    {"GWI", new Company { Name = "Great Wall Industries", Sector = CompanySector.Industrial,Country=CompanyCountry.South_Korea}},
    {"TSW", new Company { Name = "Tiger Steel Works", Sector = CompanySector.Industrial,Country=CompanyCountry.China }},

    // Энергетика
    {"VRD", new Company { Name = "Veridian Power", Sector = CompanySector.Energy,Country=CompanyCountry.USA }},
    {"AEON", new Company { Name = "Aeon Power Grid", Sector = CompanySector.Energy,Country=CompanyCountry.Japan }},

    // Сектор потребительских товаров
    { "EGC", new Company { Name = "Evergreen Goods Co.",Sector = CompanySector.Consumer_Goods,Country=CompanyCountry.USA } },

    // Недвижимость
    { "CSD", new Company { Name = "Cityscape Development", Sector = CompanySector.Real_Estate,Country=CompanyCountry.China} },
    { "RRT", new Company { Name = "Regency Realty Trust", Sector = CompanySector.Real_Estate,Country=CompanyCountry.China} },

    //Tech
    { "LUMN", new Company { Name = "Lumen Networks",Sector = CompanySector.Technology,Country=CompanyCountry.France } },
    { "CHAI", new Company { Name = "Chronos AI",Sector = CompanySector.Technology,Country=CompanyCountry.Canada } },
    //Health
    { "GNM", new Company { Name = "Genomica",Sector = CompanySector.Healthcare,Country=CompanyCountry.Germany } },
    };
}
public class Company
{
    public string Name { get; set; }
    public CompanySector Sector { get; set; }
    public CompanyCountry Country { get; set; }
}
public enum LevelStability
    {
        High,
        Normal,
        Low
    }

public enum CompanyCountry
{
    USA,
    China,
    Germany,
    France,
    Japan,
    South_Korea,
    Canada,
}
public enum CompanySector
{
    Financial,
    Energy,
    Technology,
    Healthcare,
    Consumer_Goods,
    Industrial,
    Real_Estate,
}

