using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace RevitHood
{
    public class EditableParameters
    {

        public List<string> paraList = new List<string>();


        public List<string> uniqueParaList = new List<string>();



        public EditableParameters(UIApplication uiAp)
        {
            Document doc = uiAp.ActiveUIDocument.Document;

            GetAllParameters(doc);
        }

        private void GetAllParameters(Autodesk.Revit.DB.Document doc)
        {

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(FamilyInstance)).WhereElementIsViewIndependent().Where(e => e.Category.CategoryType == CategoryType.Model).ToList();

            foreach (Element el in collector)
            {
              
           

               

                    // Add Instance Parameter names to list
                foreach (Parameter Param in el.Parameters)
                {
                    if (!Param.IsReadOnly)
                    {
                        paraList.Add(Param.Definition.Name);

                    }

                }
                
            }
           
            uniqueParaList = paraList.Distinct().ToList();



        }
        // public List<string>  getParams(Autodesk.Revit.DB.Document doc)







    }
}
