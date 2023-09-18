#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using RevitHood.Forms;
using RevitHood.Functions;

#endregion

namespace RevitHood
{

    [Transaction(TransactionMode.Manual)]
    public class BoxCommand : IExternalCommand
    {



        Document doc;
        UIDocument uidoc;
        SelectionBoxForm fm;
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {

            UIApplication uiapp = commandData.Application;
            this.uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            this.doc = uidoc.Document;
            Selection selection = uidoc.Selection;
            List<ElementId> selectedIds = uidoc.Selection.GetElementIds().ToList();


            if (0 == selectedIds.Count)
            {
                return Result.Failed;
            }


            this.fm = new SelectionBoxForm();

            fm.ShowDialog();

            if (!fm.isCreate)
            {
                return Result.Failed;
            }
            View3D view3D;

            var direction = new XYZ(-1, 1, -1);
            var collector = new FilteredElementCollector(doc);
            var viewFamilyType = collector.OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>()
              .FirstOrDefault(x => x.ViewFamily == ViewFamily.ThreeDimensional);
            using (Transaction ttNew = new Transaction(doc, "Creating New View"))
            {
                ttNew.Start();
                view3D = View3D.CreateIsometric(
                                  doc, viewFamilyType.Id);

                view3D.SetOrientation(new ViewOrientation3D(
                  direction, new XYZ(0, 1, 1), new XYZ(0, 1, -1)));
                if (fm.viewName != "")
                {
                    view3D.Name = fm.viewName;
                }
                ttNew.Commit();
            }
            uidoc.RequestViewChange(view3D);
            BoundingBoxXYZ xyz = getxyzBox(selectedIds);

            using (Transaction ttNew = new Transaction(doc, "Creating Section Box"))
            {
                ttNew.Start();
                view3D.SetSectionBox(xyz);
                ttNew.Commit();

            }
            return Result.Succeeded;
        }

        public BoundingBoxXYZ getxyzBox(List<ElementId> selectedIds)
        {
            List<double> BoundsX = new List<double>();
            List<double> BoundsY = new List<double>();
            List<double> BoundsZ = new List<double>();
            foreach (ElementId id in selectedIds)
            {
                Element el = doc.GetElement(id);
                BoundingBoxXYZ xyz = el.get_BoundingBox(doc.ActiveView);
                double maxX = xyz.Max.X;
                double maxY = xyz.Max.Y;
                double maxZ = xyz.Max.Z;

                double minX = xyz.Min.X;
                double minY = xyz.Min.Y;
                double minZ = xyz.Min.Z;

                BoundsX.Add(maxX);
                BoundsX.Add(minX);

                BoundsY.Add(maxY);
                BoundsY.Add(minY);

                BoundsZ.Add(maxZ);
                BoundsZ.Add(minZ);

            }

            double minXFinal = BoundsX.Min();
            double maxXFinal = BoundsX.Max();

            double minYFinal = BoundsY.Min();
            double maxYFinal = BoundsY.Max();

            double minZFinal = BoundsZ.Min();
            double maxZFinal = BoundsZ.Max();



            XYZ xyzMax = new XYZ(maxXFinal, maxYFinal, maxZFinal);
            XYZ xyzMin = new XYZ(minXFinal, minYFinal, minZFinal);

            BoundingBoxXYZ xyzBox = new BoundingBoxXYZ();
            xyzBox.Min = xyzMin;
            xyzBox.Max = xyzMax;


            return xyzBox;
        }
    }
}
