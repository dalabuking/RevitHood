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
using System.Security.Cryptography;
using System.Windows;
using RevitHood.Functions;

#endregion

namespace RevitHood
{

    [Transaction(TransactionMode.Manual)]
    public class ColorCommand : IExternalCommand
    {
      
    
        ParameterPicker paraForm;
        ColorAssigner colorAss;

        ProjectParameters parameters;

        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
           
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;
            bool isParameter = true;


            parameters = new ProjectParameters(uiapp);

            parameters.uniqueParaList.Sort();


            while (isParameter) {
                paraForm = new ParameterPicker(uiapp , parameters.uniqueParaList);
                paraForm.ShowDialog();
                if (paraForm.isParameterPicked)
                {

                    colorAss = new ColorAssigner(uiapp, paraForm.pickedParameter);

                    colorAss.ShowDialog();
                    isParameter = colorAss.isBack; 
                }
                else
                {
                    isParameter = false;
                }
            }
        
           
          
           // ProjectParameters parameters = new ProjectParameters(commandData);


            //FamiliesAppa families = new FamiliesAppa(commandData);
            //  modelElements elems = new modelElements(commandData);

            return Result.Succeeded;
        }
    }
}
