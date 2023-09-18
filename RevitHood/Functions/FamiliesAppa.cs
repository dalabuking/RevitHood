using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Creation;

namespace RevitHood.Functions
{
    public class FamiliesAppa
    {


        public static SortedList<string, Family> familyList = new SortedList<string, Family>();
        public static SortedList<string, FamilyInstance> familyInstanceList = new SortedList<string, FamilyInstance>();

        public FamiliesAppa(ExternalCommandData commandData)
        {
            Autodesk.Revit.DB.Document doc = commandData.Application.ActiveUIDocument.Document;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            getAllFamilies(doc , uidoc);
        }

        public void getAllFamilies(Autodesk.Revit.DB.Document doc, UIDocument uidoc)
        {
 
            FilteredElementCollector families = new FilteredElementCollector(doc);
            families.OfClass(typeof(Family));
            FilteredElementCollector familyInstances = new FilteredElementCollector(doc);
            familyInstances.OfClass(typeof(FamilyInstance));



            foreach (Family family in families)
            {
                try
                {
                 
                    familyList.Add(family.Name, family);
                }
                catch
                {
                }
            }


            foreach (FamilyInstance familyInstance in familyInstances)
            {
                try
                {


                    familyInstanceList.Add(familyInstance.Name, familyInstance);
                }
                catch
                {
                }
            }



        }


    }
}
