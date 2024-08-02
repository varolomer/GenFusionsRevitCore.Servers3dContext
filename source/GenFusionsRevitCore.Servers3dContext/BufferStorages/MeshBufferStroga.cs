using Autodesk.Revit.DB;
using Autodesk.Revit.DB.DirectContext3D;
using GenFusionsRevitCore.Servers3dContext.Graphics;

namespace GenFusionsRevitCore.Servers3dContext.BufferStorages
{
    internal class MeshBufferStorage : BaseBufferStorage
    {
        public readonly List<Mesh> Meshes;
        public readonly List<IndexTriangle> IndexTriangles;
        public readonly List<List<VertexPosition>> Vertices_Position; //Every mesh's faces` vertex should be group so we can assign individual colors for meshes not individual colors for every vertex


        public MeshBufferStorage(DisplayStyle displayStyle, List<Mesh> meshes) : base(displayStyle)
        {
            this.BufferPrimitiveType = PrimitiveType.TriangleList;

            //Set fundamentals
            this.Meshes = meshes;
            this.DisplayStyle = displayStyle;

            //While we are already looping we can already determine the index triangles.
            IndexTriangles = new List<IndexTriangle>();
            Vertices_Position = new List<List<VertexPosition>>();
            int triangleIndex = 0;
            foreach (Mesh mesh in this.Meshes)
            {
                var Vertices_Position_Sub = new List<VertexPosition>();
                //Set counts
                this.PrimitiveCount += mesh.NumTriangles;

                for (int i = 0; i < mesh.NumTriangles; i++)
                {
                    MeshTriangle meshTriangle = mesh.get_Triangle(i);
                    XYZ p1 = meshTriangle.get_Vertex(0);
                    XYZ p2 = meshTriangle.get_Vertex(1);
                    XYZ p3 = meshTriangle.get_Vertex(2);

                    //Add vertices
                    Vertices_Position_Sub.Add(new VertexPosition(p1));
                    Vertices_Position_Sub.Add(new VertexPosition(p2));
                    Vertices_Position_Sub.Add(new VertexPosition(p3));

                    this.VertexBufferCount += 3;

                    //Add indices
                    IndexTriangles.Add(new IndexTriangle(triangleIndex, triangleIndex + 1, triangleIndex + 2));
                    triangleIndex += 3;
                }

                Vertices_Position.Add(Vertices_Position_Sub);
            }

        }

        /// <summary>
        /// Effect instance is not considered.
        /// </summary>
        public override void AddVertexPosition()
        {
            #region Vertex Buffer
            //Set format bits
            this.FormatBits = VertexFormatBits.Position;
            this.VertexFormat = new VertexFormat(this.FormatBits);
            this.EffectInstance = new EffectInstance(this.FormatBits);

            //Set the buffer size -- The format of the vertices determines the size of the vertex buffer.
            int vertexBufferSizeInFloats = VertexPosition.GetSizeInFloats() * this.VertexBufferCount;

            //Create Vertex Buffer and map so that the vertex data can be written into it
            this.VertexBuffer = new VertexBuffer(vertexBufferSizeInFloats);
            this.VertexBuffer.Map(vertexBufferSizeInFloats);

            //A VertexStream is used to write data into a VertexBuffer.
            VertexStreamPosition vertexStream = this.VertexBuffer.GetVertexStreamPosition();

            //Add vertices all at once
            foreach (List<VertexPosition> list in this.Vertices_Position) //Every list contains one meshes vertices
            {
                vertexStream.AddVertices(list);
            }

            //Unmap
            this.VertexBuffer.Unmap();
            #endregion

            #region IndexBuffer

            //Get the size of the index buffer
            this.IndexBufferCount = this.PrimitiveCount * IndexTriangle.GetSizeInShortInts();
            int indexBufferSizeInShortInts = 1 * this.IndexBufferCount;

            //Create Index Buffer and map so that the vertex data can be written into it
            this.IndexBuffer = new IndexBuffer(indexBufferSizeInShortInts);
            this.IndexBuffer.Map(indexBufferSizeInShortInts);

            // An IndexStream is used to write data into an IndexBuffer.
            IndexStreamTriangle indexStream = this.IndexBuffer.GetIndexStreamTriangle();

            //Add indices
            indexStream.AddTriangles(this.IndexTriangles);


            //Unmap the buffers so they can be used for rendering
            this.IndexBuffer.Unmap();
            #endregion
        }

        /// <summary>
        /// Effect instance is considered.
        /// </summary>
        public void AddVertexPositionWithEffectInstance(ColorWithTransparency color)
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
            foreach (List<VertexPosition> list in this.Vertices_Position) //Every list contains one meshes vertices
            {
                vertexStream.AddVertices(list);
            }
                

            //Unmap
            this.VertexBuffer.Unmap();
            #endregion

            #region IndexBuffer

            //Get the size of the index buffer
            this.IndexBufferCount = this.PrimitiveCount * IndexTriangle.GetSizeInShortInts();
            int indexBufferSizeInShortInts = 1 * this.IndexBufferCount;

            //Create Index Buffer and map so that the vertex data can be written into it
            this.IndexBuffer = new IndexBuffer(indexBufferSizeInShortInts);
            this.IndexBuffer.Map(indexBufferSizeInShortInts);

            // An IndexStream is used to write data into an IndexBuffer.
            IndexStreamTriangle indexStream = this.IndexBuffer.GetIndexStreamTriangle();

            //Add indices
            indexStream.AddTriangles(this.IndexTriangles);


            //Unmap the buffers so they can be used for rendering
            this.IndexBuffer.Unmap();
            #endregion
        }

        /// <summary>
        /// If the color param is null assigns random color to every line.
        /// Effect instance is not considered
        /// </summary>
        /// <param name="color"></param>
        public override void AddVertexPositionColored(ColorWithTransparency color)
        {
            #region Vertex Buffer
            //Set format bits
            this.FormatBits = VertexFormatBits.PositionColored;
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
            int vertexBufferSizeInFloats = VertexPositionColored.GetSizeInFloats() * this.VertexBufferCount;

            //Create Vertex Buffer and map so that the vertex data can be written into it
            this.VertexBuffer = new VertexBuffer(vertexBufferSizeInFloats);
            this.VertexBuffer.Map(vertexBufferSizeInFloats);

            //A VertexStream is used to write data into a VertexBuffer.
            VertexStreamPositionColored vertexStream = this.VertexBuffer.GetVertexStreamPositionColored();


            //If color parameter is not null use only that color
            if (color != null)
            {
                this.EffectInstance.SetTransparency(color.GetTransparency());
                foreach (List<VertexPosition> list in this.Vertices_Position) //Every list contains one meshes vertices
                {
                    foreach (VertexPosition point in list)
                    {
                        vertexStream.AddVertex(new VertexPositionColored(point.Position, color));
                    }
                        
                }
            }
            //If color parameter is null assign random colors
            else
            {
                var colors = new List<ColorWithTransparency>() { SimpleColors.Green, SimpleColors.Blue, SimpleColors.Red, SimpleColors.Orange, SimpleColors.Magenta, SimpleColors.Cyan };
                Random random = new Random();
                int colorIndex = 0;

                foreach (List<VertexPosition> list in this.Vertices_Position) //Every list contains one meshes vertices
                {
                    colorIndex = random.Next(0, colors.Count);

                    foreach (VertexPosition point in list)
                    {
                        vertexStream.AddVertex(new VertexPositionColored(point.Position, colors[colorIndex]));
                    }
                }
            }

            //Unmap
            this.VertexBuffer.Unmap();
            #endregion

            #region IndexBuffer

            //Get the size of the index buffer
            this.IndexBufferCount = this.PrimitiveCount * IndexTriangle.GetSizeInShortInts();
            int indexBufferSizeInShortInts = 1 * this.IndexBufferCount;

            //Create Index Buffer and map so that the vertex data can be written into it
            this.IndexBuffer = new IndexBuffer(indexBufferSizeInShortInts);
            this.IndexBuffer.Map(indexBufferSizeInShortInts);

            // An IndexStream is used to write data into an IndexBuffer.
            IndexStreamTriangle indexStream = this.IndexBuffer.GetIndexStreamTriangle();

            //Add indices
            indexStream.AddTriangles(this.IndexTriangles);

            //Unmap the buffers so they can be used for rendering
            this.IndexBuffer.Unmap();
            #endregion
        }

        public override void AddVertexPosition(ColorWithTransparency color)
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
