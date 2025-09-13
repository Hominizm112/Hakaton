using UnityEngine;
using System.Collections.Generic;

public interface IPortfolioService
{
    
}

public class PortfolioSummary: MonoService
{
    public float TotalValue { get; }
    public float StocksValue { get; }
    public float BondsValue { get; }
    public float TotalGainLoss { get; }
    public float DayGainLoss { get; }
}