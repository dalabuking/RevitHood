using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Windows.Controls;
using System.Collections;
using Autodesk.Revit.UI.Selection;
using System.Drawing;

namespace RevitHood.Functions
{
     public class asingColor
    {
        public static SortedList<string, Element> categoriesList = new SortedList<string, Element>();
        public asingColor(ExternalCommandData commandData, Category catType , System.Drawing.Color colorToAsign)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ColorCategories(doc, uidoc, catType , colorToAsign);
        }
        public void  ColorCategories(Autodesk.Revit.DB.Document doc, UIDocument uidoc, Category catType, System.Drawing.Color colorToAsign)
        {
            FilteredElementCollector elements = new FilteredElementCollector(doc);
            List<Element> AllElem = new FilteredElementCollector(doc, uidoc.ActiveView.Id).WhereElementIsViewIndependent().
             Where(e => e.Category == catType).ToList();


            List<Material> materials = new List<Material>(
       new FilteredElementCollector(doc)
         .WhereElementIsNotElementType()
         .OfClass(typeof(Material))
         .ToElements()
         .Cast<Material>());

            Material solidMat = materials.Find(x => x.Name == "Solid fill");
           
            FillPatternElement solidFillPattern = elements.OfClass(typeof(FillPatternElement)).Cast<FillPatternElement>().First(a => a.GetFillPattern().IsSolidFill);
            Autodesk.Revit.DB.Color color = new Autodesk.Revit.DB.Color(colorToAsign.R, colorToAsign.G, colorToAsign.B); // RGB
            OverrideGraphicSettings ogs = new OverrideGraphicSettings();
            ogs.SetSurfaceForegroundPatternColor(color); // or other here
            ogs.SetSurfaceForegroundPatternId(solidFillPattern.Id);
            foreach (Element el in AllElem)
            {
                try
                {
                    using (Transaction tx = new Transaction(doc))
                    {
                        tx.Start("Change Element Color");
                        doc.ActiveView.SetElementOverrides(el.Id, ogs);
                        //el.Category.Material = materials[0];
                        tx.Commit();
                    }

                }
                catch
                {

                }

            }
            
            // Selection sel = uidoc.Selection;
            //sel.SetElementIds(AllElem.ToList().Select(o => o.Id).ToList());
        }
    }
}
