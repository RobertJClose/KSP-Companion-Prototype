using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class OrbitPlot : InspectablePlot
{
    Polyline mainPlot;
    [SerializeField]
    Polyline highlightPlot;
    [SerializeField]
    Sphere periapsisSphere;
    [SerializeField]
    Sphere apoapsisSphere;
    [SerializeField]
    Sphere ascendingNodeSphere;
    [SerializeField]
    Sphere descendingNodeSphere;
    [SerializeField]
    Line lineOfApsides;
    [SerializeField]
    Line lineOfNodes;

    [SerializeField]
    Color highlightedColour;
    [SerializeField]
    float nodesRadius;

    Color notTravelledOnColor = Color.grey;

    private void Awake()
    {
        mainPlot = GetComponent<Polyline>();

        highlightPlot.gameObject.SetActive(false);
        periapsisSphere.gameObject.SetActive(false);
        apoapsisSphere.gameObject.SetActive(false);
        ascendingNodeSphere.gameObject.SetActive(false);
        descendingNodeSphere.gameObject.SetActive(false);
        lineOfApsides.gameObject.SetActive(false);
        lineOfNodes.gameObject.SetActive(false);
    }

    public override void HighlightPlot(bool isHighlighted)
    {
        highlightPlot.gameObject.SetActive(isHighlighted);
        periapsisSphere.gameObject.SetActive(isHighlighted);
        apoapsisSphere.gameObject.SetActive(isHighlighted);
        ascendingNodeSphere.gameObject.SetActive(isHighlighted);
        descendingNodeSphere.gameObject.SetActive(isHighlighted);
        lineOfApsides.gameObject.SetActive(isHighlighted);
        lineOfNodes.gameObject.SetActive(isHighlighted);
    }

    public void PlotTrajectory(Orbit orbit, Color color, double? startTime = null, double? endTime = null)
    {
        // This method creates the plot of a supplied orbital trajectory, while also positioning spheres/lines to represent the points
        // of periapsis, apoapsis, ascending node, descending node, and the lines of apsides and the line of nodes. 
        // The start/endTime arguments may be passed so that it may be determined if a particular point is actually travelled
        // on during the orbital trajectory; points that are not travelled on are coloured grey.

        // 1. Plot the orbital trajectory
        // First get a list of Vector3 points along the orbital trajectory.
        List<Vector3d> orbitPoints = orbit.OrbitalPoints(-orbit.MaxTrueAnomaly, orbit.MaxTrueAnomaly, out List<Angled> trueAnomalies, Constants.OrbitDefaultStepRad);

        // Now each one of these points must be turned into a PolylinePoint by being given a colour and thickness.
        PolylinePoint nextPoint;
        Angled nextPointTrueAnomaly;
        int pointIndex = 0;
        List<PolylinePoint> polylinePoints = new List<PolylinePoint>(orbitPoints.Count);
        foreach (Vector3d point in orbitPoints)
        {
            nextPointTrueAnomaly = trueAnomalies[pointIndex];
            Vector3 rescaledPoint = (Vector3)(point * Constants.PlotRescaleFactor);

            // If a point is too far away, don't add it to the plot.
            if (rescaledPoint.magnitude > Constants.MaximumPlotDistance)
            {
                pointIndex++;
                continue;
            }
            else
                nextPoint.point = rescaledPoint;

            // Colour the point according to whether it is actually travelled on during the orbital trajectory.
            if (DetermineIfPointIsTravelledOn(nextPointTrueAnomaly, orbit, startTime, endTime))
                nextPoint.color = color;
            else
                nextPoint.color = notTravelledOnColor;

            nextPoint.thickness = Constants.OrbitPlotThickness;

            polylinePoints.Add(nextPoint);
            pointIndex++;
        }

        polylinePoints.TrimExcess();

        mainPlot.SetPoints(polylinePoints);
        highlightPlot.SetPoints(polylinePoints.ConvertAll((PolylinePoint point) => new PolylinePoint(point.point, highlightedColour, Constants.OrbitPlotThickness)));

        if (orbit.OrbitType == Orbit.ConicSection.Elliptical)
            SetClosedPlot(true);
        else
            SetClosedPlot(false);

        // 2. Position the spheres for the periapsis and apoapsis, and position the line of apsides.
        Vector3 periapsisPlotPosition;
        Vector3 apoapsisPlotPosition;

        if (orbit.PeriapsisPoint.magnitude * Constants.PlotRescaleFactor < Constants.MaximumPlotDistance)
            periapsisPlotPosition = (Vector3)(orbit.PeriapsisPoint * Constants.PlotRescaleFactor);
        else
            periapsisPlotPosition = Vector3.zero;

        if (orbit.ApoapsisPoint.HasValue && 
            orbit.ApoapsisPoint.Value.magnitude * Constants.PlotRescaleFactor < Constants.MaximumPlotDistance)
            apoapsisPlotPosition = (Vector3)(orbit.ApoapsisPoint.Value * Constants.PlotRescaleFactor);
        else
            apoapsisPlotPosition = Vector3.zero;

        periapsisSphere.transform.position = periapsisPlotPosition;
        apoapsisSphere.transform.position = apoapsisPlotPosition;
        periapsisSphere.Radius = nodesRadius;
        apoapsisSphere.Radius = nodesRadius;
        lineOfApsides.Start = periapsisSphere.transform.position;
        lineOfApsides.End = apoapsisSphere.transform.position;

        // 3. Position the spheres for the ascending and descending nodes, and position the line of nodes.
        Vector3 ascendingNodePlotPosition;
        Vector3 descendingNodePlotPosition;

        if (orbit.AscendingNode.HasValue && 
            orbit.AscendingNode.Value.magnitude * Constants.PlotRescaleFactor < Constants.MaximumPlotDistance)
            ascendingNodePlotPosition = (Vector3)(orbit.AscendingNode * Constants.PlotRescaleFactor);
        else
            ascendingNodePlotPosition = Vector3.zero;

        if (orbit.DescendingNode.HasValue && 
            orbit.DescendingNode.Value.magnitude * Constants.PlotRescaleFactor < Constants.MaximumPlotDistance)
            descendingNodePlotPosition = (Vector3)(orbit.DescendingNode.Value * Constants.PlotRescaleFactor);
        else
            descendingNodePlotPosition = Vector3.zero;

        ascendingNodeSphere.transform.position = ascendingNodePlotPosition;
        descendingNodeSphere.transform.position = descendingNodePlotPosition;
        ascendingNodeSphere.Radius = nodesRadius * 0.75f;
        descendingNodeSphere.Radius = nodesRadius * 0.75f;
        lineOfNodes.Start = ascendingNodeSphere.transform.position;
        lineOfNodes.End = descendingNodeSphere.transform.position;
    }

    private bool DetermineIfPointIsTravelledOn(Angled pointTrueAnomaly, Orbit orbit, double? startTime, double? endTime)
    {
        Angled? startTrueAnomaly;
        Angled? endTrueAnomaly;

        if (startTime.HasValue)
            startTrueAnomaly = orbit.Time2TrueAnomaly(startTime.Value);
        else
            startTrueAnomaly = null;

        if (endTime.HasValue)
            endTrueAnomaly = orbit.Time2TrueAnomaly(endTime.Value);
        else
            endTrueAnomaly = null;

        if (orbit.OrbitType == Orbit.ConicSection.Elliptical)
        {
            // Elliptical orbit case.
            if ((startTrueAnomaly == null) ||
                (endTrueAnomaly == null) ||
                ((endTime - startTime) > orbit.Period) ||
                (pointTrueAnomaly.IsBetween(startTrueAnomaly, endTrueAnomaly)))
            {
                // Point is travelled on.
                return true;
            }
            else
            {
                // Point isn't travelled on.
                return false;
            }
        }
        else
        {
            // Open orbit case.
            if ((startTrueAnomaly == null && endTrueAnomaly == null) ||
                (startTrueAnomaly == null && pointTrueAnomaly.IsBetween(-orbit.MaxTrueAnomaly, endTrueAnomaly)) ||
                (pointTrueAnomaly.IsBetween(startTrueAnomaly, orbit.MaxTrueAnomaly) && endTrueAnomaly == null) ||
                (pointTrueAnomaly.IsBetween(startTrueAnomaly, endTrueAnomaly)))
            {
                // Point is travelled on.
                return true;
            }
            else
            {
                // Point isn't travelled on.
                return false;
            }
        }
    }

    public void SetClosedPlot(bool isClosed)
    {
        mainPlot.Closed = isClosed;
        highlightPlot.Closed = isClosed;
    }
}
