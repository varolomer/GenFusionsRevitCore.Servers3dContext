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
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class DrawDirection : ExternalCommand
    {
        public override void Execute()
        {
            double startSize = (0.4).FromMeters();
            double endSize = (0.1).FromMeters();
            double FiveMetersInInternal = (5.0).FromMeters();
            double offset = (0.2).FromMeters();
            XYZ sp1 = new XYZ(0, 0, 0);
            XYZ sp2 = new XYZ(0, 0, FiveMetersInInternal);

            XYZ sp3 = new XYZ(0, 0, FiveMetersInInternal);
            XYZ sp4 = new XYZ(FiveMetersInInternal, 0, 0);

            XYZ sp5 = new XYZ(FiveMetersInInternal, 0, 0);
            XYZ sp6 = new XYZ(0, 0, 0);

            ExAp.appInstance.ServerStateMachine.DrawBlend(Document, sp1, sp2, startSize, endSize, SimpleColors.Orange, SimpleColors.White);
            ExAp.appInstance.ServerStateMachine.DrawBlend(Document, sp3, sp4, startSize, endSize, SimpleColors.Magenta, SimpleColors.White);
            ExAp.appInstance.ServerStateMachine.DrawBlend(Document, sp5, sp6, startSize, endSize, SimpleColors.Blue, SimpleColors.White);
        }
    }
}
