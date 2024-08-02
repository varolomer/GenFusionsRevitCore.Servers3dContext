using Autodesk.Revit.DB;
using Autodesk.Revit.DB.DirectContext3D;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;
using GenFusionsRevitCore.Servers3dContext.BufferStorages;

namespace GenFusionsRevitCore.Servers3dContext.Servers
{
    internal class MeshServer : IDirectContext3DServer
    {
        #region Fields
        private Guid m_guid;
        private UIDocument m_uiDocument;
        private List<Mesh> Meshes;
        private MeshBufferStorage MeshBufferStorage;
        private ColorWithTransparency Color;
        private bool IsColored;
        private bool UseEffectInstance;


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
        public MeshServer(UIDocument uiDoc, List<Mesh> meshes, bool isColored = false, bool UseEffectInstance = false, ColorWithTransparency color = null)
        {
            //Fundamentals
            m_guid = Guid.NewGuid();
            m_uiDocument = uiDoc;
            Meshes = meshes;
            IsColored = isColored;
            Color = color;
            this.UseEffectInstance = UseEffectInstance;

        }
        #endregion

        #region Trivial Fields
        public Guid GetServerId() { return m_guid; }
        public string GetVendorId() { return "GenFusions"; }
        public ExternalServiceId GetServiceId() { return ExternalServices.BuiltInExternalServices.DirectContext3DService; }
        public string GetName() { return "Mesh Server"; }
        public string GetDescription() { return "Mesh Server"; }
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
            List<XYZ> allVertices = new List<XYZ>();

            foreach (Mesh mesh in Meshes)
            {
                allVertices.AddRange(mesh.Vertices);
            }
            double minX = allVertices.Min(x => x.X);
            double minY = allVertices.Min(x => x.Y);
            double minZ = allVertices.Min(x => x.Z);

            double maxX = allVertices.Max(x => x.X);
            double maxY = allVertices.Max(x => x.Y);
            double maxZ = allVertices.Max(x => x.Z);

            return new Outline(new XYZ(minX - 1, minY - 1, minZ - 1), new XYZ(maxX + 1, maxY + 1, maxZ + 1));
        }

        public void RenderScene(View dBView, DisplayStyle displayStyle)
        {
            try
            {
                // Populate geometry buffers if they are not initialized or need updating.
                if (MeshBufferStorage == null || MeshBufferStorage.needsUpdate(displayStyle))
                {
                    if (IsColored == false)
                    {
                        MeshBufferStorage = new MeshBufferStorage(displayStyle, Meshes);
                        MeshBufferStorage.AddVertexPosition();
                    }

                    else if (IsColored == true && UseEffectInstance == true && Color != null)
                    {
                        MeshBufferStorage = new MeshBufferStorage(displayStyle, Meshes);
                        MeshBufferStorage.AddVertexPositionWithEffectInstance(Color);
                    }

                    else
                    {
                        MeshBufferStorage = new MeshBufferStorage(displayStyle, Meshes);
                        MeshBufferStorage.AddVertexPositionColored(Color);
                    }

                }

                if (MeshBufferStorage.PrimitiveCount > 0)
                    DrawContext.FlushBuffer(MeshBufferStorage.VertexBuffer,
                                            MeshBufferStorage.VertexBufferCount,
                                            MeshBufferStorage.IndexBuffer,
                                            MeshBufferStorage.IndexBufferCount,
                                            MeshBufferStorage.VertexFormat,
                                            MeshBufferStorage.EffectInstance,
                                            MeshBufferStorage.BufferPrimitiveType,
                                            0,
                                            MeshBufferStorage.PrimitiveCount);

            }
            catch (Exception e)
            {
                string message = e.Message;
            }
        }
    }
}
