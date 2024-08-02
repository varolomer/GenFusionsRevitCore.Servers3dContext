using Autodesk.Revit.Attributes;
using GenFusionsRevitCore.Servers3dContext.Graphics;
using Nice3point.Revit.Toolkit.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample3DProject.Commands
{
    public class WallSolidExtractor
    {
        /// <summary>
        /// Extracts the solid from a given wall element.
        /// </summary>
        /// <param name="wall">The wall element from which to extract the solid.</param>
        /// <returns>The solid geometry of the wall.</returns>
        public static Solid GetWallSolid(Wall wall)
        {
            // Check if the input is valid
            if (wall == null)
            {
                throw new ArgumentNullException(nameof(wall));
            }

            // Get the geometry of the wall
            Options geomOptions = new Options
            {
                ComputeReferences = true,
                DetailLevel = ViewDetailLevel.Fine,
                IncludeNonVisibleObjects = false
            };

            GeometryElement geomElement = wall.get_Geometry(geomOptions);

            // Iterate through the geometry objects
            foreach (GeometryObject geomObj in geomElement)
            {
                // If the geometry object is a solid, return it
                if (geomObj is Solid solid)
                {
                    if (solid.Volume > 0) // Check if the solid has volume
                    {
                        return solid;
                    }
                }

                // If the geometry object is an instance, extract its solids
                if (geomObj is GeometryInstance geomInstance)
                {
                    GeometryElement instanceGeomElement = geomInstance.GetInstanceGeometry();
                    foreach (GeometryObject instanceGeomObj in instanceGeomElement)
                    {
                        if (instanceGeomObj is Solid instanceSolid)
                        {
                            if (instanceSolid.Volume > 0) // Check if the solid has volume
                            {
                                return instanceSolid;
                            }
                        }
                    }
                }
            }

            // If no solid was found, return null
            return null;
        }
    }


    /// <summary>
    /// Draws the solids of walls in the document with some transformation.
    /// </summary>
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class DrawSolidsOfWalls : ExternalCommand
    {
        public override void Execute()
        {
            var wallsCollector = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_Walls)
                                                                       .WhereElementIsNotElementType()
                                                                       .Where(x => (x as Wall) != null)
                                                                       .Cast<Wall>();

            double offset = (8.0).FromMeters();
            Transform offsetTransform = Transform.CreateTranslation(XYZ.BasisX.Multiply(offset));
            foreach (var wall in wallsCollector)
            {
                Solid wallSolid = WallSolidExtractor.GetWallSolid(wall);

                if (wallSolid == null) continue;

                Solid transformedSolid = wallSolid.CreateTransformed(offsetTransform);
                ExAp.appInstance.ServerStateMachine.DrawSolid(Document, transformedSolid, new ColorWithTransparency(255,165,0,50), SimpleColors.White);
            }
        }
    }
}
