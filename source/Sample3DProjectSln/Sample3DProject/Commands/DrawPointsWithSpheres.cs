using Autodesk.Revit.Attributes;
using GenFusionsRevitCore.Servers3dContext.Graphics;
using Nice3point.Revit.Toolkit.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample3DProject.Commands
{
    /// <summary>
    /// Creates a sequence of points
    /// </summary>
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class DrawPointsWithSpheres : ExternalCommand
    {
        public override void Execute()
        {
            int POINT_ROW_COUNT = 3;
            int POINT_ROW_Z_COUNT = 3;
            double OFFSET = (2.0).FromMeters(); // 200 cm
            double RADIUS = (0.30).FromMeters(); // 30cm

            List<XYZ> points = new List<XYZ>();

            for (int i = 0; i < POINT_ROW_COUNT; i++) 
            { 
                for (int j = 0; j < POINT_ROW_COUNT; j++)
                {
                    for (int w = 0; w < POINT_ROW_Z_COUNT; w++)
                    {
                        XYZ xyz = new XYZ(i * OFFSET, j * OFFSET, w * OFFSET);
                        points.Add(xyz);
                    }
                }
            }

            ExAp.appInstance.ServerStateMachine.DrawPointsSphere(Document, points, RADIUS, SimpleColors.Orange, SimpleColors.White);
        }
    }

    /// <summary>
    /// Creates a sphere point at zero point with 5m radius.
    /// </summary>
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class DrawPointWithSphere : ExternalCommand
    {
        public override void Execute()
        {
            double RADIUS = (5.0).FromMeters();
            XYZ point = new XYZ(0, 0, 0);
            ExAp.appInstance.ServerStateMachine.DrawPointSphere(Document, point, RADIUS, SimpleColors.Orange, SimpleColors.White);
        }
    }
}
