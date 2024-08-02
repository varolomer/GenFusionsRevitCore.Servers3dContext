using Autodesk.Revit.DB;
using Autodesk.Revit.DB.DirectContext3D;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;
using GenFusionsRevitCore.Servers3dContext.BufferStorages;

namespace GenFusionsRevitCore.Servers3dContext.Servers
{
    internal class SolidServer : IDirectContext3DServer
    {
        #region Fields
        private Guid m_guid;
        private UIDocument m_uiDocument;
        private Solid Solid;
        private SolidFaceBufferStorage SolidFaceBufferStorage;
        private SolidFaceBufferStorage SolidFaceBufferStorage_Transparent;
        private SolidEdgeBufferStorage SolidEdgeBufferStorage;
        private SolidEdgeBufferStorage SolidEdgeBufferStorage_Transparent;
        private ColorWithTransparency EdgeColor;
        private ColorWithTransparency FaceColor;
        private bool UseNormals;


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
        public SolidServer(UIDocument uiDoc, Solid solid, bool useNormals = false,
                           ColorWithTransparency faceColor = null,
                           ColorWithTransparency edgeColor = null)
        {
            //Fundamentals
            m_guid = Guid.NewGuid();
            m_uiDocument = uiDoc;
            Solid = SolidUtils.Clone(solid);
            FaceColor = faceColor;
            EdgeColor = edgeColor;
            UseNormals = useNormals;

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
            List<XYZ> Points = new List<XYZ>();
            foreach (Edge edge in Solid.Edges)
            {
                var edgeTes = edge.Tessellate();
                Points.AddRange(edgeTes);
            }

            double minX = Points.Min(x => x.X);
            double minY = Points.Min(x => x.Y);
            double minZ = Points.Min(x => x.Z);

            double maxX = Points.Max(x => x.X);
            double maxY = Points.Max(x => x.Y);
            double maxZ = Points.Max(x => x.Z);

            return new Outline(new XYZ(minX - 1, minY - 1, minZ - 1), new XYZ(maxX + 1, maxY + 1, maxZ + 1));
        }

        public void RenderScene(View dBView, DisplayStyle displayStyle)
        {
            try
            {
                // FOR FACES -- Populate geometry buffers if they are not initialized or need updating.
                if (SolidFaceBufferStorage == null || SolidFaceBufferStorage.needsUpdate(displayStyle))
                {
                    if (UseNormals == false)
                    {
                        SolidFaceBufferStorage = new SolidFaceBufferStorage(displayStyle, Solid);
                        SolidFaceBufferStorage_Transparent = new SolidFaceBufferStorage(displayStyle, Solid);
                        SolidFaceBufferStorage.AddVertexPosition(FaceColor);
                        SolidFaceBufferStorage_Transparent.AddVertexPosition(FaceColor);
                    }
                    else
                    {
                        SolidFaceBufferStorage = new SolidFaceBufferStorage(displayStyle, Solid);
                        SolidFaceBufferStorage_Transparent = new SolidFaceBufferStorage(displayStyle, Solid);
                        SolidFaceBufferStorage.AddVertexPositionNormalColored(FaceColor);
                        SolidFaceBufferStorage_Transparent.AddVertexPositionNormalColored(FaceColor);
                    }

                }

                // FOR EDGES - Populate geometry buffers if they are not initialized or need updating.
                if (SolidEdgeBufferStorage == null || SolidEdgeBufferStorage.needsUpdate(displayStyle))
                {

                    SolidEdgeBufferStorage = new SolidEdgeBufferStorage(displayStyle, Solid);
                    SolidEdgeBufferStorage_Transparent = new SolidEdgeBufferStorage(displayStyle, Solid);
                    SolidEdgeBufferStorage.AddVertexPosition(EdgeColor);
                    SolidEdgeBufferStorage_Transparent.AddVertexPosition(EdgeColor);

                }

                bool isTransparentPass = DrawContext.IsTransparentPass();
                bool colorHasTransparency = FaceColor.GetTransparency() != 0;

                #region For Faces
                if (!isTransparentPass && !colorHasTransparency)
                {
                    if (SolidFaceBufferStorage.PrimitiveCount > 0 && displayStyle != DisplayStyle.Wireframe)
                        DrawContext.FlushBuffer(SolidFaceBufferStorage.VertexBuffer,
                                                SolidFaceBufferStorage.VertexBufferCount,
                                                SolidFaceBufferStorage.IndexBuffer,
                                                SolidFaceBufferStorage.IndexBufferCount,
                                                SolidFaceBufferStorage.VertexFormat,
                                                SolidFaceBufferStorage.EffectInstance,
                                                SolidFaceBufferStorage.BufferPrimitiveType,
                                                0,
                                                SolidFaceBufferStorage.PrimitiveCount);
                }
                else if (isTransparentPass && colorHasTransparency)
                {
                    if (SolidFaceBufferStorage_Transparent.PrimitiveCount > 0 && displayStyle != DisplayStyle.Wireframe)
                        DrawContext.FlushBuffer(SolidFaceBufferStorage_Transparent.VertexBuffer,
                                                SolidFaceBufferStorage_Transparent.VertexBufferCount,
                                                SolidFaceBufferStorage_Transparent.IndexBuffer,
                                                SolidFaceBufferStorage_Transparent.IndexBufferCount,
                                                SolidFaceBufferStorage_Transparent.VertexFormat,
                                                SolidFaceBufferStorage_Transparent.EffectInstance,
                                                SolidFaceBufferStorage_Transparent.BufferPrimitiveType,
                                                0,
                                                SolidFaceBufferStorage_Transparent.PrimitiveCount);
                }


                #endregion

                #region For Edges
                if (SolidEdgeBufferStorage.PrimitiveCount > 0)
                    DrawContext.FlushBuffer(SolidEdgeBufferStorage.VertexBuffer,
                                            SolidEdgeBufferStorage.VertexBufferCount,
                                            SolidEdgeBufferStorage.IndexBuffer,
                                            SolidEdgeBufferStorage.IndexBufferCount,
                                            SolidEdgeBufferStorage.VertexFormat,
                                            SolidEdgeBufferStorage.EffectInstance,
                                            SolidEdgeBufferStorage.BufferPrimitiveType,
                                            0,
                                            SolidEdgeBufferStorage.PrimitiveCount);

                #endregion
            }
            catch (Exception e)
            {
                string message = e.Message;
            }
        }
    }

    #region Helper classes

    // A container to hold information associated with a triangulated face.
    class MeshInfo
    {
        public MeshInfo(Mesh mesh, XYZ normal, ColorWithTransparency color)
        {
            Mesh = mesh;
            Normal = normal;
            ColorWithTransparency = color;
        }

        public Mesh Mesh;
        public XYZ Normal;
        public ColorWithTransparency ColorWithTransparency;
    }
    #endregion
}
