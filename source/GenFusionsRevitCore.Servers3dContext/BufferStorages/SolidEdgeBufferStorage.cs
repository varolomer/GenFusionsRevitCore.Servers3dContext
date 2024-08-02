using Autodesk.Revit.DB;
using Autodesk.Revit.DB.DirectContext3D;

namespace GenFusionsRevitCore.Servers3dContext.BufferStorages
{
    internal class SolidEdgeBufferStorage : BaseBufferStorage
    {
        public readonly List<IndexLine> IndexLines;
        public readonly List<VertexPosition> Vertices;
        public Solid Solid;
        public EdgeArray EdgeArray;
        //public readonly List<Tuple<XYZ, XYZ>> VerticesWithNormals;

        public SolidEdgeBufferStorage(DisplayStyle displayStyle, Solid solid) : base(displayStyle)
        {
            this.BufferPrimitiveType = PrimitiveType.LineList;
            this.Solid = solid; //Keep the solid in scope to prevent garbage collection on native handles.
            this.DisplayStyle = displayStyle;

            IndexLines = new List<IndexLine>();
            Vertices = new List<VertexPosition>();

            //VerticesWithNormals = new List<Tuple<XYZ, XYZ>>();

            int lineIndex = 0;
            EdgeArray = solid.Edges; //Keep the solid in scope to prevent garbage collection on native handles.
            foreach (Edge edge in EdgeArray)
            {
                //Get the mesh representation of the face
                List<XYZ> tesellatedEdge = edge.Tessellate().ToList();
                var Vertices_Sub = (from XYZ p in tesellatedEdge select new VertexPosition(p)).ToList();
                Vertices.AddRange(Vertices_Sub);

                //Set counts
                this.PrimitiveCount += tesellatedEdge.Count - 1; //I.e. If there are three points in poly line (tesselated edge) it means there is going to be two lines
                this.VertexBufferCount += tesellatedEdge.Count;



                for(int i = 0; i < tesellatedEdge.Count - 1; i++)
                {
                    IndexLines.Add(new IndexLine(lineIndex, lineIndex + 1));

                    lineIndex += 2;
                }
            }

        }

        /// <summary>
        /// Effect instance is not considered.
        /// </summary>
        public override void AddVertexPosition(ColorWithTransparency color)
        {
            #region Vertex Buffer
            //Set format bits
            this.FormatBits = VertexFormatBits.Position;
            this.VertexFormat = new VertexFormat(this.FormatBits);
            this.EffectInstance = new EffectInstance(this.FormatBits);
            this.EffectInstance.SetColor(color.GetColor());
            this.EffectInstance.SetDiffuseColor(color.GetColor());
            this.EffectInstance.SetTransparency(color.GetTransparency() / 255.0);

            // So in the shaded view we do not override the automatic shadings of given color
            if (this.DisplayStyle == DisplayStyle.HLR)
            {
                this.EffectInstance.SetSpecularColor(color.GetColor());
                this.EffectInstance.SetAmbientColor(color.GetColor());
                this.EffectInstance.SetEmissiveColor(color.GetColor());
            }

            //Set the buffer size -- The format of the vertices determines the size of the vertex buffer.
            int vertexBufferSizeInFloats = VertexPosition.GetSizeInFloats() * this.VertexBufferCount;

            //Create Vertex Buffer and map so that the vertex data can be written into it
            this.VertexBuffer = new VertexBuffer(vertexBufferSizeInFloats);
            this.VertexBuffer.Map(vertexBufferSizeInFloats);

            //A VertexStream is used to write data into a VertexBuffer.
            VertexStreamPosition vertexStream = this.VertexBuffer.GetVertexStreamPosition();

            //Add vertices all at once
            vertexStream.AddVertices(this.Vertices);

            //Unmap
            this.VertexBuffer.Unmap();
            #endregion

            #region IndexBuffer

            //Get the size of the index buffer
            this.IndexBufferCount = this.PrimitiveCount * IndexLine.GetSizeInShortInts();
            int indexBufferSizeInShortInts = 1 * this.IndexBufferCount;

            //Create Index Buffer and map so that the vertex data can be written into it
            this.IndexBuffer = new IndexBuffer(indexBufferSizeInShortInts);
            this.IndexBuffer.Map(indexBufferSizeInShortInts);

            // An IndexStream is used to write data into an IndexBuffer.
            IndexStreamLine indexStream = this.IndexBuffer.GetIndexStreamLine();

            //Add indices
            indexStream.AddLines(this.IndexLines);


            //Unmap the buffers so they can be used for rendering
            this.IndexBuffer.Unmap();
            #endregion
        }

        public override void AddVertexPosition()
        {
            throw new NotImplementedException();
        }

        public override void AddVertexPositionColored(ColorWithTransparency color)
        {
            throw new NotImplementedException();
        }

        public override void AddVertexPositionNormal()
        {
            throw new NotImplementedException();
        }

        public override void AddVertexPositionNormalColored(ColorWithTransparency color)
        {
            throw new NotImplementedException();
        }
    }
}
