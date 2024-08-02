using Autodesk.Revit.DB;
using Autodesk.Revit.DB.DirectContext3D;

namespace GenFusionsRevitCore.Servers3dContext.BufferStorages
{
    internal class SolidFaceBufferStorage : BaseBufferStorage
    {
        public readonly List<IndexTriangle> IndexTriangles;
        public readonly List<VertexPosition> Vertices; 
        public readonly List<Tuple<XYZ, XYZ>> VerticesWithNormals;

        public Solid Solid;
        public FaceArray FaceArray;

        public SolidFaceBufferStorage(DisplayStyle displayStyle, Solid solid) : base(displayStyle)
        {
            this.BufferPrimitiveType = PrimitiveType.TriangleList;
            this.Solid = solid; //Keep the solid in scope to prevent garbage collection on native handles.
            this.DisplayStyle = displayStyle;

            //While we are already looping we can already determine the index triangles.
            IndexTriangles = new List<IndexTriangle>();
            Vertices = new List<VertexPosition>();
            VerticesWithNormals = new List<Tuple<XYZ, XYZ>>();
            

            int triangleIndex = 0;
            FaceArray = solid.Faces; //Keep the solid in scope to prevent garbage collection on native handles.
            foreach (Face face in FaceArray)
            {
                //Get the mesh representation of the face
                //This face does not have to be a PlanarFace all the time. In this case it should be considered that there will be multiple normals
                Mesh mesh = face.Triangulate();
                XYZ faceNormal = face.ComputeNormal(new UV(0.5, 0.5)); 

                //Set counts
                this.PrimitiveCount += mesh.NumTriangles;
               
                //Iterate over the mesh representation of the face
                for (int i = 0; i < mesh.NumTriangles; i++) //i is the index of triangle
                {
                    MeshTriangle meshTriangle = mesh.get_Triangle(i);

                    XYZ p1 = meshTriangle.get_Vertex(0);
                    XYZ p2 = meshTriangle.get_Vertex(1);
                    XYZ p3 = meshTriangle.get_Vertex(2);

                    //Find the normal vector by the given three points
                    var dir = (p2 - p1).CrossProduct(p3 - p1);
                    var normal = dir.Normalize();

                    //If the found normal vector is not in the same approximate direction with the face normal, the reverse it
                    if(normal.DotProduct(faceNormal) < 0)
                    {
                        normal = normal.Negate();
                    }

                    //Add vertices
                    Vertices.Add(new VertexPosition(p1));
                    Vertices.Add(new VertexPosition(p2));
                    Vertices.Add(new VertexPosition(p3));
                    VerticesWithNormals.Add(new Tuple<XYZ, XYZ>(p1, normal));
                    VerticesWithNormals.Add(new Tuple<XYZ, XYZ>(p2, normal));
                    VerticesWithNormals.Add(new Tuple<XYZ, XYZ>(p3, normal));

                    this.VertexBufferCount += 3;

                    //Add indices
                    IndexTriangles.Add(new IndexTriangle(triangleIndex, triangleIndex + 1, triangleIndex + 2));
                    triangleIndex += 3;
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
            if(this.DisplayStyle == DisplayStyle.HLR)
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
        /// Effect instance is not considered.
        /// </summary>
        public override void AddVertexPositionNormalColored(ColorWithTransparency color)
        {
            #region Vertex Buffer
            //Set format bits
            this.FormatBits = VertexFormatBits.PositionNormalColored;
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
            int vertexBufferSizeInFloats = VertexPositionNormalColored.GetSizeInFloats() * this.VertexBufferCount;

            //Create Vertex Buffer and map so that the vertex data can be written into it
            this.VertexBuffer = new VertexBuffer(vertexBufferSizeInFloats);
            this.VertexBuffer.Map(vertexBufferSizeInFloats);

            //A VertexStream is used to write data into a VertexBuffer.
            VertexStreamPositionNormalColored vertexStream = this.VertexBuffer.GetVertexStreamPositionNormalColored();

            //Add vertices all at once
            foreach(var item in this.VerticesWithNormals)
            {
                vertexStream.AddVertex(new VertexPositionNormalColored(item.Item1, item.Item2, color));
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
            foreach(var tri in this.IndexTriangles)
            {
                indexStream.AddTriangle(tri);
            }
            
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
    }
}
