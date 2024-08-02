using Autodesk.Revit.DB;
using Autodesk.Revit.DB.DirectContext3D;


namespace GenFusionsRevitCore.Servers3dContext.BufferStorages
{
    internal abstract class BaseBufferStorage
    {
        public DisplayStyle DisplayStyle { get; set; }

        //Counts
        public int PrimitiveCount { get; set; }
        public int VertexBufferCount { get; set; }
        public int IndexBufferCount { get; set; }


        //Buffers
        public VertexBuffer VertexBuffer { get; set; }
        public IndexBuffer IndexBuffer { get; set; }
        public PrimitiveType BufferPrimitiveType { get; set; }

        //Formats
        public VertexFormatBits FormatBits { get; set; }
        public VertexFormat VertexFormat { get; set; }
        public EffectInstance EffectInstance { get; set; }



        /// <summary>
        /// Constroctor
        /// </summary>
        /// <param name="displayStyle"></param>
        public BaseBufferStorage(DisplayStyle displayStyle)
        {
            DisplayStyle = displayStyle;
           
        }

        public bool needsUpdate(DisplayStyle newDisplayStyle)
        {
            if (newDisplayStyle != DisplayStyle)
                return true;

            if (PrimitiveCount > 0)
                if (VertexBuffer == null || !VertexBuffer.IsValid() ||
                    IndexBuffer == null || !IndexBuffer.IsValid() ||
                    VertexFormat == null || !VertexFormat.IsValid() ||
                    EffectInstance == null || !EffectInstance.IsValid())
                    return true;

            return false;
        }

        public abstract void AddVertexPosition(ColorWithTransparency color);
        public abstract void AddVertexPosition();

        public abstract void AddVertexPositionColored(ColorWithTransparency color);

        public abstract void AddVertexPositionNormal();

        public abstract void AddVertexPositionNormalColored(ColorWithTransparency color);


    }
}
