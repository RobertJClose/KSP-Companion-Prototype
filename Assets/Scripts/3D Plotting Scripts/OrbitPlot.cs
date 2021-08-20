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

    public void SetPlotPoints(List<PolylinePoint> points, Vector3 periapsisPoint, Vector3? apoapsisPoint, Vector3? ascendingNodePoint, Vector3? descendingNodePoint)
    {
        mainPlot.SetPoints( points.ConvertAll( (PolylinePoint point) => new PolylinePoint(point.point * Constants.PlotRescaleFactor, point.color, point.thickness) ) );
        highlightPlot.SetPoints( points.ConvertAll( (PolylinePoint point) => new PolylinePoint(point.point * Constants.PlotRescaleFactor, highlightedColour, Constants.OrbitPlotThickness) ) );

        periapsisSphere.transform.position = (periapsisPoint) * Constants.PlotRescaleFactor;
        apoapsisSphere.transform.position = (apoapsisPoint ?? Vector3.zero) * Constants.PlotRescaleFactor;
        ascendingNodeSphere.transform.position = (ascendingNodePoint ?? Vector3.zero) * Constants.PlotRescaleFactor;
        descendingNodeSphere.transform.position = (descendingNodePoint ?? Vector3.zero) * Constants.PlotRescaleFactor;

        periapsisSphere.Radius = nodesRadius;
        apoapsisSphere.Radius = nodesRadius;
        ascendingNodeSphere.Radius = nodesRadius * 0.75f;
        descendingNodeSphere.Radius = nodesRadius * 0.75f;

        lineOfApsides.Start = periapsisSphere.transform.position;
        lineOfApsides.End = apoapsisSphere.transform.position;
        lineOfNodes.Start = ascendingNodeSphere.transform.position;
        lineOfNodes.End = descendingNodeSphere.transform.position;
    }

    public void SetClosedPlot(bool isClosed)
    {
        mainPlot.Closed = isClosed;
        highlightPlot.Closed = isClosed;
    }
}
