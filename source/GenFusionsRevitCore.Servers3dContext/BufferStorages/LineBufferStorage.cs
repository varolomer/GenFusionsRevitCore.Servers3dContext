using Autodesk.Revit.DB;
using Autodesk.Revit.DB.DirectContext3D;
using GenFusionsRevitCore.Servers3dContext.Graphics;

namespace GenFusionsRevitCore.Servers3dContext.BufferStorages
{
    internal class LineBufferStorage : BaseBufferStorage
    {
        public readonly List<XYZ> Points;
        public LineBufferStorage(DisplayStyle displayStyle, List<Line> lines) : base(displayStyle)
        {
            //Set fundamentals
            this.Points = new List<XYZ>();
            this.DisplayStyle = displayStyle;

            foreach (Line line in lines)
            {
                this.Points.Add(line.GetEndPoint(0));
                this.Points.Add(line.GetEndPoint(1));
            }

            //Set counts -- Vertext count and index count will be the same for points
            this.VertexBufferCount = this.Points.Count;
            this.PrimitiveCount = lines.Count;
            this.BufferPrimitiveType = PrimitiveType.LineList;
        }

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

            foreach (XYZ point in this.Points)
            {
                vertexStream.AddVertex(new VertexPosition(point));
            }

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

            //int index = 0;
            //foreach(XYZ point in this.Points) //The points of the lines are already ordered
            //{
            //    indexStream.AddLine(new IndexLine(index, index + 1));
            //    index += 2;
            //}


            int index = 0;
            for (int i = 0; i < this.PrimitiveCount; i++)
            {
                indexStream.AddLine(new IndexLine(index, index + 1));
                index+=2;
            }

            //Unmap the buffers so they can be used for rendering
            this.IndexBuffer.Unmap();
            #endregion
        }

        /// <summary>
        /// If the color param is null assigns random color to every line.
        /// </summary>
        /// <param name="color"></param>
        public override void AddVertexPositionColored(ColorWithTransparency color = null)
        {
            #region Vertex Buffer
            //Set format bits
            this.FormatBits = VertexFormatBits.PositionColored;
            this.VertexFormat = new VertexFormat(this.FormatBits);
            this.EffectInstance = new EffectInstance(this.FormatBits);

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
                foreach (XYZ point in this.Points)
                {
                    vertexStream.AddVertex(new VertexPositionColored(point, color));
                }
            }
            //If color parameter is null assign random colors
            else
            {
                var colors = new List<ColorWithTransparency>() { SimpleColors.Green, SimpleColors.Blue, SimpleColors.Red, SimpleColors.Orange, SimpleColors.Magenta, SimpleColors.Cyan };
                Random random = new Random();
                int colorIndex = 0;
                int index = 0;
                foreach (XYZ point in this.Points) //Every pair of points will belong to a line.
                {
                    if(index == 0 || index % 2 == 0)
                    {
                        colorIndex = random.Next(0, colors.Count);
                        vertexStream.AddVertex(new VertexPositionColored(point, colors[colorIndex]));
                    }
                    else
                    {
                        vertexStream.AddVertex(new VertexPositionColored(point, colors[colorIndex]));
                    }

                    index++;

                }
            }

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

            int index2 = 0;
            for (int i = 0; i < this.PrimitiveCount; i++)
            {
                indexStream.AddLine(new IndexLine(index2, index2 + 1));
                index2 += 2;
            }

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
