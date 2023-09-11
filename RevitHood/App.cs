#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace RevitHood
{
     internal class App : IExternalApplication 
    {
 
        public Result OnStartup(UIControlledApplication aplication)
        {
           
            string tabName = "RH Addons";
            string panelName = "Parameters";
            aplication.CreateRibbonTab(tabName);
            RibbonPanel panel = aplication.CreateRibbonPanel(tabName, panelName);
            PushButtonData colorBtn = new PushButtonData(
               "C_Btn",
               "Color Parameters",
               Assembly.GetExecutingAssembly().Location,
               "RevitHood.ColorCommand"
               );

            PushButtonData changeBtn = new PushButtonData(
            "CP_Btn",
            "Change Parameters",
            Assembly.GetExecutingAssembly().Location,
            "RevitHood.ChangeParametersCommand"
            );
            PushButton buttonColor = panel.AddItem(colorBtn) as PushButton;
            buttonColor.ToolTip = "Cool Tool To Add Color To Parameter Values";
            buttonColor.Enabled = true;

            PushButton changeParameter = panel.AddItem(changeBtn) as PushButton;
            changeParameter.ToolTip = "Cool Tool To Change Parameter Values";
            changeParameter.Enabled = true;

            string panelNameBox = "Selection Box";
            RibbonPanel panelBox = aplication.CreateRibbonPanel(tabName, panelNameBox);

            PushButtonData boxBtn = new PushButtonData(
              "C_Btn",
              "Create Box",
              Assembly.GetExecutingAssembly().Location,
              "RevitHood.BoxCommand"
              );

            PushButton boxButton = panelBox.AddItem(boxBtn) as PushButton;
            boxButton.ToolTip = "Cool Tool To Create View With Section Box";
            boxButton.Enabled = true;


            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}
