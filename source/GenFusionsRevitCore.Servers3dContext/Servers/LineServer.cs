using Autodesk.Revit.DB;
using Autodesk.Revit.DB.DirectContext3D;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;
using GenFusionsRevitCore.Servers3dContext.BufferStorages;

namespace GenFusionsRevitCore.Servers3dContext.Servers
{
    internal class LineServer : IDirectContext3DServer
    {
        #region Fields
        private Guid m_guid;
        private UIDocument m_uiDocument;
        private List<Line> Lines;
        private LineBufferStorage LineBufferStorage;
        private ColorWithTransparency Color;
        private bool IsColored;


        public Document Document
        {
            get { return m_uiDocument != null ? m_uiDocument.Document : null; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// If IsColored marked as true and color parameter left null, the server will assign random colors to the given points.
        /// </summary>
        /// <param name="uiDoc"></param>
        /// <param name="points"></param>
        /// <param name="IsColored"></param>
        /// <param name="color"></param>
        public LineServer(UIDocument uiDoc, List<Line> lines, bool isColored = false, ColorWithTransparency color = null)
        {
            //Fundamentals
            m_guid = Guid.NewGuid();
            m_uiDocument = uiDoc;
            Lines = lines;
            IsColored = isColored;
            Color = color;

        }
        #endregion

        #region Trivial Fields
        public Guid GetServerId() { return m_guid; }
        public string GetVendorId() { return "GenFusions"; }
        public ExternalServiceId GetServiceId() { return ExternalServices.BuiltInExternalServices.DirectContext3DService; }
        public string GetName() { return "Line Server"; }
        public string GetDescription() { return "Line Server"; }
        #endregion

        #region Not Used
        public string GetApplicationId() { return ""; }
        public string GetSourceId() { return ""; }
        public bool UsesHandles() { return false; }
        #endregion

        public bool UseInTransparentPass(View view) { return true; }

        public bool CanExecute(View view) { return true; }

        public Outline GetBoundingBox(View dBView)
        {
            double minX = Lines.Min(a => new List<double>() { a.GetEndPoint(0).X, a.GetEndPoint(1).X }.Min());
            double minY = Lines.Min(a => new List<double>() { a.GetEndPoint(0).Y, a.GetEndPoint(1).Y }.Min());
            double minZ = Lines.Min(a => new List<double>() { a.GetEndPoint(0).Z, a.GetEndPoint(1).Z }.Min());

            double maxX = Lines.Max(a => new List<double>() { a.GetEndPoint(0).X, a.GetEndPoint(1).X }.Max());
            double maxY = Lines.Max(a => new List<double>() { a.GetEndPoint(0).Y, a.GetEndPoint(1).Y }.Max());
            double maxZ = Lines.Max(a => new List<double>() { a.GetEndPoint(0).Z, a.GetEndPoint(1).Z }.Max());

            Outline outline = new Outline(new XYZ(minX - 1, minY - 1, minZ - 1), new XYZ(maxX + 1, maxY + 1, maxZ + 1));
            return outline;
        }

        public void RenderScene(View dBView, DisplayStyle displayStyle)
        {
            try
            {
                // Populate geometry buffers if they are not initialized or need updating.
                if (LineBufferStorage == null || LineBufferStorage.needsUpdate(displayStyle))
                {
                    if (IsColored == false)
                    {
                        LineBufferStorage = new LineBufferStorage(displayStyle, Lines);
                        LineBufferStorage.AddVertexPosition();
                    }
                    else
                    {
                        LineBufferStorage = new LineBufferStorage(displayStyle, Lines);
                        LineBufferStorage.AddVertexPositionColored(Color);
                    }

                }

                if (LineBufferStorage.PrimitiveCount > 0)
                    DrawContext.FlushBuffer(LineBufferStorage.VertexBuffer,
                                            LineBufferStorage.VertexBufferCount,
                                            LineBufferStorage.IndexBuffer,
                                            LineBufferStorage.IndexBufferCount,
                                            LineBufferStorage.VertexFormat,
                                            LineBufferStorage.EffectInstance,
                                            LineBufferStorage.BufferPrimitiveType,
                                            0,
                                            LineBufferStorage.PrimitiveCount);

            }
            catch (Exception e)
            {
                //Handle exception somehow.
            }
        }
    }
}
