using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Windows.Controls;
using System.Collections;

namespace RevitHood.Functions
{
     public class CategoriesAppa
    {
        public  SortedList<string, Category> categoriesList = new SortedList<string, Category>();
      
  

        public CategoriesAppa(UIApplication uiAp)
        {
            Document doc = uiAp.ActiveUIDocument.Document;
    
            GetCategories(doc);
        }

        public void  GetCategories(Autodesk.Revit.DB.Document doc )
        {
            Categories categories = doc.Settings.Categories;
            categories.Clear();
            foreach (Category category in categories)
                
            {
                try {
                    // check if category is model type 
                    if (CategoryType.Model == category.CategoryType)
                        categoriesList.Add(category.Name, category);
                }
                catch
                {
                }
            }

        }
    }
}
