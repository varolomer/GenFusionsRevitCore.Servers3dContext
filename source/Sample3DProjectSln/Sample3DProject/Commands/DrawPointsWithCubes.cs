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
    /// Creates a sequence of points with cube servers
    /// </summary>
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class DrawPointsWithCubes : ExternalCommand
    {
        public override void Execute()
        {
            int POINT_ROW_COUNT = 10;
            int POINT_ROW_Z_COUNT = 10;
            double OFFSET = (2.0).FromMeters(); // 200 cm
            double SIZE = (0.30).FromMeters(); // 30cm

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

            ExAp.appInstance.ServerStateMachine.DrawPointsCube(Document, points, SIZE, SimpleColors.Orange, SimpleColors.White);
        }
    }

    /// <summary>
    /// Creates a cube point at zero point with 5m size.
    /// </summary>
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class DrawPointWithCube : ExternalCommand
    {
        public override void Execute()
        {
            double SIZE = (5.0).FromMeters();
            XYZ point = new XYZ(0, 0, 0);
            ExAp.appInstance.ServerStateMachine.DrawPointCube(Document, point, SIZE, SimpleColors.Orange, SimpleColors.White);
        }
    }
}
