using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using GenFusionsRevitCore.Servers3dContext.Servers;

namespace GenFusionsRevitCore.Servers3dContext
{
    /// <summary>
    /// This class will be used as a state machine from Revit addins
    /// to call functions to add servers and draw on canvas.
    /// 
    /// This way, the external apps do not need to add boilerplate code for adding servers
    /// </summary>
    public class ServerStateMachine
    {
        public HashSet<Document> m_Documents = new HashSet<Document>(); //Hashset of documents

        //private List<LineServer> m_LineServers = new List<LineServer>();
        private List<MeshServer> m_MeshServers = new List<MeshServer>();
        private List<SolidServer> m_SolidServers = new List<SolidServer>();

        private readonly IExternalApplication ExApp;

        public ServerStateMachine(IExternalApplication externalApplication)
        {
            this.ExApp = externalApplication;
        }

        #region Solid Servers
        /// <summary>
        /// Draws a cube in given size at the given point.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="cubeSizeInMeters">Cube size in centimeters.</param>
        /// <param name="faceColor"></param>
        /// <param name="edgeColor"></param>
        public void DrawPointCube(Document doc, XYZ point, double cubeSizeInCm, ColorWithTransparency faceColor, ColorWithTransparency edgeColor)
        {
            Solid solid = GeomUtils.Solids.CreateCube(point, cubeSizeInCm);
            RegisterSolidServer(new UIDocument(doc), solid, true, faceColor, edgeColor);
        }

        /// <summary>
        /// Draws a list of cube in given size at the given point.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="cubeSizeInMeters">Cube size in centimeters.</param>
        /// <param name="faceColor"></param>
        /// <param name="edgeColor"></param>
        public void DrawPointsCube(Document doc, List<XYZ> points, double cubeSizeInCm, ColorWithTransparency faceColor, ColorWithTransparency edgeColor)
        {
            foreach (XYZ point in points)
            {
                DrawPointCube(doc, point, cubeSizeInCm, faceColor, edgeColor);
            }
        }

        /// <summary>
        /// Draws a spehere with given center and radius
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="radius">Radius in centimeters.</param>
        /// <param name="faceColor"></param>
        /// <param name="edgeColor"></param>
        public void DrawPointSphere(Document doc, XYZ point, double radius, ColorWithTransparency faceColor, ColorWithTransparency edgeColor)
        {
            Solid solid = GeomUtils.Solids.CreateCenterbasedSphere(point, radius);
            RegisterSolidServer(new UIDocument(doc), solid, true, faceColor, edgeColor);
        }
        /// <summary>
        /// Draws a list of speheres with given center and radius
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="radius">Radius in centimeters.</param>
        /// <param name="faceColor"></param>
        /// <param name="edgeColor"></param>
        public void DrawPointsSphere(Document doc, List<XYZ> points, double radius, ColorWithTransparency faceColor, ColorWithTransparency edgeColor)
        {
            foreach (XYZ point in points)
            {
                DrawPointSphere(doc, point, radius, faceColor, edgeColor);
            }
        }

        /// <summary>
        /// Draws a blend geometry by creating a GeometryCreationUtilities.CreateBlendGeometry.
        /// It draws rectangular profiles with different sizes at the end points and then creates
        /// the blend geometry which implies a direction.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="startPointSizeInCm"></param>
        /// <param name="endPointSizeInCm"></param>
        /// <param name="faceColor"></param>
        /// <param name="edgeColor"></param>
        public void DrawBlend(Document doc, XYZ startPoint, XYZ endPoint, double startPointSizeInCm, double endPointSizeInCm, ColorWithTransparency faceColor, ColorWithTransparency edgeColor)
        {
            // Test -- if works put it to ServerStateMachine
            Solid solid = GeomUtils.Solids.CreateBlendWithPoints(startPoint, endPoint, startPointSizeInCm, endPointSizeInCm);
            RegisterSolidServer(new UIDocument(doc), solid, true, faceColor, edgeColor);
        }

        /// <summary>
        /// Draws a solid with normals using DirectContext
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="solid"></param>
        /// <param name="faceColor"></param>
        /// <param name="edgeColor"></param>
        public void DrawSolid(Document doc, Solid solid, ColorWithTransparency faceColor, ColorWithTransparency edgeColor)
        {
            RegisterSolidServer(new UIDocument(doc), solid, false, faceColor, edgeColor);
        }

        /// <summary>
        /// Draws a solid with normals using DirectContext
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="solid"></param>
        /// <param name="faceColor"></param>
        /// <param name="edgeColor"></param>
        public void DrawSolidWithNormals(Document doc, Solid solid, ColorWithTransparency faceColor, ColorWithTransparency edgeColor)
        {
            RegisterSolidServer(new UIDocument(doc), solid, true, faceColor, edgeColor);
        }

        public void ClearSolidServers()
        {
            UnregisterSolidServers();
        }

        private void RegisterSolidServer(UIDocument uidoc, Solid solid, bool useNormals = false, ColorWithTransparency faceColor = null, ColorWithTransparency edgeColor = null)
        {
            var solidServer = new SolidServer(uidoc, solid, useNormals, faceColor, edgeColor);

            GenFusionsRevitCore.Servers3dContext.Utils.ServerUtilities.RegisterServer<SolidServer>(solidServer, m_SolidServers, uidoc, m_Documents);
        }

        private void UnregisterSolidServers()
        {
            GenFusionsRevitCore.Servers3dContext.Utils.ServerUtilities.UnregisterServers<SolidServer>(m_Documents, m_SolidServers);
        }

        #endregion
    }
}
