using UnityEngine;
using System.Collections.Generic;
public static class CompanyInfo
{
    public static Dictionary<Ticker, string> ActiveName = new Dictionary<Ticker, string>() {
        // Финансовый сектор
        { Ticker.GDC, "Golden Dragon Capital" },
        { Ticker.SRV, "Silk Road Ventures" },

        // Промышленный сектор
        { Ticker.GWI, "Great Wall Industries" },
        { Ticker.TSW, "Tiger Steel Works" },

        // Энергетика
        { Ticker.VRD, "Veridian Power" },
        { Ticker.AEON, "Aeon Power Grid" },

        // Сектор потребительских товаров
        { Ticker.EGC, "Evergreen Goods Co." },

        // Недвижимость
        { Ticker.CSD, "Cityscape Development" },
        { Ticker.RRT, "Regency Realty Trust" },

        // Tech
        { Ticker.LMN, "Lumen Networks" },
        { Ticker.CHAI, "Chronos AI" },

        // Health
        { Ticker.GNM, "Genomica" },

        { Ticker.UA0000F8132, "Gold. D. Capital-1832"},
        { Ticker.UA0000С8000, "Gold. D. Capital-8000C"},

        { Ticker.CH0000E9010, "Silk Road Ventures-E9010"},
        { Ticker.CH0000I0001, "Silk Road Ventures-I0001"},
        { Ticker.CH0000H0004, "Silk Road Ventures-H0004"},
        { Ticker.CH0000F7777, "Silk Road Ventures-F7777"},

        { Ticker.GE0000T0009, "Great Wall Industries-T0009"},
        { Ticker.GE0000I9090, "Great Wall Industries-I9090"},

        { Ticker.JA0000H5645, "Tiger Steel Works-H5645"},
        { Ticker.JA0000T7566, "Tiger Steel Works-T7566"},
        { Ticker.JA0000T0001, "Tiger Steel Works-T0001"},

        { Ticker.FR0000F8880, "Veridian Power-F8880"},
        { Ticker.CA0001E7655, "Evergreen Goods Co.E7655"},

        { Ticker.CA0002E7890, "Aeon Power Grid-E7890"},
        { Ticker.SK2100RS981, "Cityscape Development-RS981"},
        { Ticker.SK2400E9999, "Cityscape Development-E9999"},

    };
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

