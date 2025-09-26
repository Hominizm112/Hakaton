using UnityEngine;
using System.Collections.Generic;
public class CompanyIssuer
{
    public static Dictionary<string, Company> CompanyInfo = new Dictionary<string, Company>() {
    // Финансовый сектор
    {"GDC", new Company { Name = "Golden Dragon Capital", Sector = CompanySector.Financial, Country=CompanyCountry.China, Rating=RatingIssuer.AAA }},
    {"SRV", new Company { Name = "Silk Road Ventures", Sector = CompanySector.Financial,Country=CompanyCountry.Germany, Rating=RatingIssuer.BBB }},

    // Промышленный сектор
    {"GWI", new Company { Name = "Great Wall Industries", Sector = CompanySector.Industrial,Country=CompanyCountry.South_Korea, Rating=RatingIssuer.AAA}},
    {"TSW", new Company { Name = "Tiger Steel Works", Sector = CompanySector.Industrial, Country=CompanyCountry.China, Rating=RatingIssuer.BBB }},

    // Энергетика
    {"VRD", new Company { Name = "Veridian Power", Sector = CompanySector.Energy,Country=CompanyCountry.USA,Rating=RatingIssuer.AAA}},
    {"AEON", new Company { Name = "Aeon Power Grid", Sector = CompanySector.Energy,Country=CompanyCountry.Japan,Rating=RatingIssuer.BBB }},

    // Сектор потребительских товаров
    { "EGC", new Company { Name = "Evergreen Goods Co.",Sector = CompanySector.Consumer_Goods,Country=CompanyCountry.USA,Rating=RatingIssuer.AAA } },

    // Недвижимость
    { "CSD", new Company { Name = "Cityscape Development", Sector = CompanySector.Real_Estate,Country=CompanyCountry.China,Rating=RatingIssuer.CCC} },
    { "RRT", new Company { Name = "Regency Realty Trust", Sector = CompanySector.Real_Estate,Country=CompanyCountry.China,Rating=RatingIssuer.AAA} },

    //Tech
    { "RRT", new Company { Name = "Lumen Networks",Sector = CompanySector.Technology,Country=CompanyCountry.France,Rating=RatingIssuer.CCC } },
    { "CHAI", new Company { Name = "Chronos AI",Sector = CompanySector.Technology,Country=CompanyCountry.Canada, Rating=RatingIssuer.D } },
    //Health
    { "GNM", new Company { Name = "Genomica",Sector = CompanySector.Healthcare,Country=CompanyCountry.Germany,Rating=RatingIssuer.BBB } },
    };
}
public class Company
{
    public string Name { get; set; }
    public CompanySector Sector { get; set; }
    public CompanyCountry Country { get; set; }
    public RatingIssuer Rating { get; set; }
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

