using Autodesk.Revit.DB;
using Autodesk.Revit.DB.DirectContext3D;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GenFusionsRevitCore.Servers3dContext.Utils
{
    internal class ServerUtilities
    {

        /// <summary>
        /// This function is written to avoid boilerplate code. It abstracts the repititive code
        /// while registering the different servers to Revit.
        /// </summary>
        /// <typeparam name="T">A type of a IDirectContext3DServer</typeparam>
        /// <param name="revitServer">The server instance that will be passed to server list.</param>
        /// <param name="serverList">The list of the server. (Must be the field of IExternalApplication</param>
        /// <param name="uidoc">UIDocument object</param>
        /// <param name="documentList">The list of documents which are coming from IExternalApplication.</param>
        public static void RegisterServer<T>(T revitServer, List<T> serverList, UIDocument uidoc, HashSet<Document> documentList) where T : IDirectContext3DServer
        {
            ExternalService directContext3DService = ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.DirectContext3DService);
            MultiServerService msDirectContext3DService = directContext3DService as MultiServerService;
            IList<Guid> serverIds = msDirectContext3DService.GetActiveServerIds();

            directContext3DService.AddServer(revitServer);
            serverList.Add(revitServer);

            serverIds.Add(revitServer.GetServerId());

            msDirectContext3DService.SetActiveServers(serverIds);

            // If the document is not in the list add it.
            if(!documentList.Contains(uidoc.Document)) documentList.Add(uidoc.Document);

            uidoc.UpdateAllOpenViews();
        }

        /// <summary>
        /// Unregisters all the servers of a given type.
        /// </summary>
        /// <typeparam name="T">A type of a IDirectContext3DServer</typeparam>
        ///  <param name="documentList">The list of documents which are coming from IExternalApplication.</param>
        /// <param name="serverList">The list of the server. (Must be the field of IExternalApplication</param>
        public static void UnregisterServers<T>(HashSet<Document> documentList, List<T> serverList) where T : IDirectContext3DServer
        {
            ExternalServiceId externalDrawerServiceId = ExternalServices.BuiltInExternalServices.DirectContext3DService;
            var externalDrawerService = ExternalServiceRegistry.GetService(externalDrawerServiceId) as MultiServerService;
            if (externalDrawerService == null)
                return;

            //Remove server which has the given Type (T)
            foreach (var registeredServerId in externalDrawerService.GetRegisteredServerIds())
            {
                var externalDrawServer = externalDrawerService.GetServer(registeredServerId);

                if (externalDrawServer is T)
                {
                    externalDrawerService.RemoveServer(registeredServerId);
                }

            }

            serverList.Clear();

            foreach (var doc in documentList)
            {
                if (!doc.IsValidObject) continue;
                UIDocument uidoc = new UIDocument(doc);
                uidoc.UpdateAllOpenViews();
            }

            documentList.Clear();

        }
    }
}
