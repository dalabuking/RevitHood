using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace RevitHood
{
    public class ProjectParameters
    {

        public List<string> paraList = new List<string>();


        public List<string> uniqueParaList = new List<string>();



        public ProjectParameters(UIApplication uiAp)
        {
            Document doc = uiAp.ActiveUIDocument.Document;

            GetAllParameters(doc);
        }

        private void GetAllParameters(Autodesk.Revit.DB.Document doc)
        {

            FilteredElementCollector familyInstances = new FilteredElementCollector(doc);
            familyInstances.OfClass(typeof(FamilyInstance)).WhereElementIsViewIndependent().Where(e => e.Category.CategoryType == CategoryType.Model).ToList();

            foreach (FamilyInstance FmlyInst in familyInstances)
            {
                // Add Instance Parameter names to list
                foreach (Parameter Param in FmlyInst.Parameters)
                {
                 
                        paraList.Add(Param.Definition.Name);
                    
                }
            }
           
            uniqueParaList = paraList.Distinct().ToList();
           


        }
        // public List<string>  getParams(Autodesk.Revit.DB.Document doc)







    }
}
