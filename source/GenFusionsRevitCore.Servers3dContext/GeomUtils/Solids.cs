using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GenFusionsRevitCore.Servers3dContext.GeomUtils
{
    internal class Solids
    {
        /// <summary>
        /// Creates a solid of a cube which has the given point in center
        /// and which has the given size.
        /// </summary>
        /// <param name="center">Center of the cube</param>
        /// <param name="cubeSizeInCm">Size of the cube in internal unit</param>
        /// <returns></returns>
        public static Solid CreateCube(XYZ center, double cubeSize = 5)
        {
            double halfCubeSize = cubeSize / 2;

            XYZ p1 = new XYZ(center.X + halfCubeSize, center.Y + halfCubeSize, center.Z - halfCubeSize);
            XYZ p2 = new XYZ(center.X - halfCubeSize, center.Y + halfCubeSize, center.Z - halfCubeSize);
            XYZ p3 = new XYZ(center.X - halfCubeSize, center.Y - halfCubeSize, center.Z - halfCubeSize);
            XYZ p4 = new XYZ(center.X + halfCubeSize, center.Y - halfCubeSize, center.Z - halfCubeSize);

            Curve curve1 = Line.CreateBound(p1, p2);
            Curve curve2 = Line.CreateBound(p2, p3);
            Curve curve3 = Line.CreateBound(p3, p4);
            Curve curve4 = Line.CreateBound(p4, p1);

            CurveLoop profile = new CurveLoop();
            profile.Append(curve1);
            profile.Append(curve2);
            profile.Append(curve3);
            profile.Append(curve4);

            List<CurveLoop> profiles = new List<CurveLoop>() { profile };

            Solid solid = GeometryCreationUtilities.CreateExtrusionGeometry(profiles, new XYZ(0, 0, 1), cubeSize);

            return solid;

        }

        /// <summary>
        /// Create a centerbased sphere
        /// </summary>
        /// <param name="center">The given sphere center</param>
        /// <param name="radiusInCm">The given sphere's radius in internal unit</param>
        /// <returns>The created sphere</returns>
        public static Solid CreateCenterbasedSphere(XYZ center, double radius)
        {
            Frame frame = new Frame(center,
               Autodesk.Revit.DB.XYZ.BasisX,
               Autodesk.Revit.DB.XYZ.BasisY,
               Autodesk.Revit.DB.XYZ.BasisZ);

            List<CurveLoop> profileloops = new List<CurveLoop>();
            CurveLoop profileloop = new CurveLoop();
            Curve cemiEllipse = Ellipse.CreateCurve(center, radius, radius,
               Autodesk.Revit.DB.XYZ.BasisX,
               Autodesk.Revit.DB.XYZ.BasisZ,
               -Math.PI / 2.0, Math.PI / 2.0);
            profileloop.Append(cemiEllipse);
            profileloop.Append(Line.CreateBound(
               new XYZ(center.X, center.Y, center.Z + radius),
               new XYZ(center.X, center.Y, center.Z - radius)));
            profileloops.Add(profileloop);

            return GeometryCreationUtilities.CreateRevolvedGeometry(frame, profileloops, -Math.PI, Math.PI);
        }

        /// <summary>
        /// Creates a blend geometry, blending rectangles at the start point and 
        /// end point of the curve.
        /// </summary>
        /// <param name="startPoint">Start point of the path</param>
        /// <param name="endPoint">End point of the path</param>
        /// <param name="startPointSize">The size of the rectangle in the start point (in internal unit)</param>
        /// <param name="endPointSize">The size of the rectangle in the end point (in internal unit)</param>
        /// <returns></returns>
        public static Solid CreateBlendWithPoints(XYZ startPoint, XYZ endPoint, double startPointSize, double endPointSize)
        {
            double degrees = 90.0;
            double radians_90degrees = degrees * Math.PI / 180.0;
            XYZ direction = endPoint - startPoint;

            // Determine which cross product to use
            XYZ crossZ = direction.CrossProduct(XYZ.BasisZ);
            XYZ crossX = direction.CrossProduct(XYZ.BasisX);
            XYZ crossY = direction.CrossProduct(XYZ.BasisY);
            XYZ crossVector;
            if (!crossZ.IsZeroLength()) crossVector = crossZ;
            else if (!crossX.IsZeroLength()) crossVector = crossX;
            else if (!crossY.IsZeroLength()) crossVector = crossY;
            else throw new Exception("Cross vectors returns zero.");

            XYZ up = direction.CrossProduct(crossVector);
            double angleBetweenZ = direction.AngleTo(XYZ.BasisZ);
            double half_startPointSize = startPointSize / 2;
            double half_endPointSize = endPointSize / 2;

            //-------------------------------------------------------------------------------------------------------
            // Start Point
            //-------------------------------------------------------------------------------------------------------
            XYZ startPoint_p1 = startPoint.Add(up.Normalize().Multiply(half_startPointSize));
            XYZ startPoint_p2 = startPoint.Add(crossVector.Normalize().Multiply(half_startPointSize));
            XYZ startPoint_p3 = startPoint.Add(up.Normalize().Multiply(half_startPointSize).Negate());
            XYZ startPoint_p4 = startPoint.Add(crossVector.Normalize().Multiply(half_startPointSize).Negate());
            Line startPoint_line1 = Line.CreateBound(startPoint_p1, startPoint_p2);
            Line startPoint_line2 = Line.CreateBound(startPoint_p2, startPoint_p3);
            Line startPoint_line3 = Line.CreateBound(startPoint_p3, startPoint_p4);
            Line startPoint_line4 = Line.CreateBound(startPoint_p4, startPoint_p1);

            // Create the XY curveloop
            CurveLoop startPointXYCurveLoop = new CurveLoop();
            startPointXYCurveLoop.Append(startPoint_line1); startPointXYCurveLoop.Append(startPoint_line2); startPointXYCurveLoop.Append(startPoint_line3); startPointXYCurveLoop.Append(startPoint_line4);

            // Create the actual (transformed curve loop)
            Transform SP_Rotation_Transform = Transform.CreateRotationAtPoint(direction, angleBetweenZ, startPoint);
            CurveLoop SP_CurveLoop = CurveLoop.CreateViaTransform(startPointXYCurveLoop, SP_Rotation_Transform);

            //-------------------------------------------------------------------------------------------------------
            // End Point
            //-------------------------------------------------------------------------------------------------------
            XYZ endPoint_p1 = endPoint.Add(up.Normalize().Multiply(half_endPointSize));
            XYZ endPoint_p2 = endPoint.Add(crossVector.Normalize().Multiply(half_endPointSize));
            XYZ endPoint_p3 = endPoint.Add(up.Normalize().Multiply(half_endPointSize).Negate());
            XYZ endPoint_p4 = endPoint.Add(crossVector.Normalize().Multiply(half_endPointSize).Negate());
            Line endPoint_line1 = Line.CreateBound(endPoint_p1, endPoint_p2);
            Line endPoint_line2 = Line.CreateBound(endPoint_p2, endPoint_p3);
            Line endPoint_line3 = Line.CreateBound(endPoint_p3, endPoint_p4);
            Line endPoint_line4 = Line.CreateBound(endPoint_p4, endPoint_p1);

            // Create the XY curveloop
            CurveLoop endPointXYCurveLoop = new CurveLoop();
            endPointXYCurveLoop.Append(endPoint_line1); endPointXYCurveLoop.Append(endPoint_line2); endPointXYCurveLoop.Append(endPoint_line3); endPointXYCurveLoop.Append(endPoint_line4);

            // Create the actual (transformed curve loop)
            Transform EP_Rotation_Transform = Transform.CreateRotationAtPoint(direction, angleBetweenZ, endPoint);
            CurveLoop EP_CurveLoop = CurveLoop.CreateViaTransform(endPointXYCurveLoop, EP_Rotation_Transform);

            //-------------------------------------------------------------------------------------------------------
            // GeoOps
            //-------------------------------------------------------------------------------------------------------
            // Create vertex pairs
            List<VertexPair> vertexPairs = new List<VertexPair>();
            vertexPairs.Add(new VertexPair(0, 0));
            vertexPairs.Add(new VertexPair(1, 1));
            vertexPairs.Add(new VertexPair(2, 2));
            vertexPairs.Add(new VertexPair(3, 3));

            //Create solid
            Solid solid = GeometryCreationUtilities.CreateBlendGeometry(SP_CurveLoop, EP_CurveLoop, vertexPairs);
            return solid;

        }
    }
}
