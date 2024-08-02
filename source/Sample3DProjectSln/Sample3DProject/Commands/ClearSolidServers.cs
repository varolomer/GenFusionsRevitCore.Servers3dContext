using Autodesk.Revit.Attributes;
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
    public class ClearSolidServers : ExternalCommand
    {
        public override void Execute()
        {
            ExAp.appInstance.ServerStateMachine.ClearSolidServers();
        }
    }
}
