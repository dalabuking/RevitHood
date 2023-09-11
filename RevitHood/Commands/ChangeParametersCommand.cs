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
using System.Windows;
using System.Windows.Media;
using RevitHood.Functions;

#endregion

namespace RevitHood
{

    [Transaction(TransactionMode.Manual)]
    public class ChangeParametersCommand : IExternalCommand
    {
      
    
        ParameterPicker paraForm;

        EditableParameters parameters;

        RevitHood.ChangeParameterForm changeParameter;

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

            parameters = new EditableParameters(uiapp);
            parameters.uniqueParaList.Sort();


            while (isParameter) {
                paraForm = new ParameterPicker(uiapp, parameters.uniqueParaList);
                paraForm.ShowDialog();
                if (paraForm.isParameterPicked)
                {
              
                  changeParameter = new RevitHood.ChangeParameterForm(uiapp, paraForm.pickedParameter);

                    changeParameter.ShowDialog();

                }
                else
                {
                    isParameter = false;
                }
            }
        
           
          
      

            return Result.Succeeded;
        }
    }
}
