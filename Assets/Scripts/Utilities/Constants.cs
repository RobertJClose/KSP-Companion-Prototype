using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static readonly int KeplerEquationMaxIterations = 100; 
    public static readonly double KeplerEquationPrecision = 1e-15;

    public static readonly int LambertSolverMaxIterations = 5;

    public static readonly float PlotRescaleFactor = 1.0f / 600_000.0f;
    public static readonly float OrbitPlotThickness = 1.0f;

    public static readonly Angle OrbitDefaultStepRad = new Angle(0.05f);

    public static readonly float MaximumPlotDistance = 1_000.0f;
}