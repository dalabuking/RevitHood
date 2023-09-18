#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

using System.Drawing;
using System.Windows.Media;


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
            "Element Selector",
            Assembly.GetExecutingAssembly().Location,
            "RevitHood.ChangeParametersCommand"
            );
            Image imgColor = RevitHood.Properties.Resources.cl32;
            ImageSource imgSrcColor = GetSoruceImage(imgColor);

            PushButton buttonColor = panel.AddItem(colorBtn) as PushButton;
            buttonColor.ToolTip = "Cool Tool To Add Color To Parameter Values";
            buttonColor.Enabled = true;
          
          
            buttonColor.Image = imgSrcColor;
            buttonColor.LargeImage = imgSrcColor;

            Image imgPara = RevitHood.Properties.Resources.para32;
            ImageSource imgSrcPara = GetSoruceImage(imgPara);

            PushButton changeParameter = panel.AddItem(changeBtn) as PushButton;
            changeParameter.ToolTip = "Cool Tool To Change Parameter Values";
            changeParameter.Enabled = true;

            changeParameter.Image = imgSrcPara;
            changeParameter.LargeImage = imgSrcPara;

            string panelNameBox = "Selection Box";
            RibbonPanel panelBox = aplication.CreateRibbonPanel(tabName, panelNameBox);

            PushButtonData boxBtn = new PushButtonData(
              "C_Btn",
              "Create Selection Box",
              Assembly.GetExecutingAssembly().Location,
              "RevitHood.BoxCommand"
              );

            Image imgBox = RevitHood.Properties.Resources.bx32;
            ImageSource imgSrcBox = GetSoruceImage(imgBox);

            PushButton boxButton = panelBox.AddItem(boxBtn) as PushButton;
            boxButton.ToolTip = "Cool Tool To Create View With Section Box";
            boxButton.Enabled = true;
            boxButton.Image = imgSrcBox;
            boxButton.LargeImage = imgSrcBox;
            


            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }


        private BitmapSource GetSoruceImage(Image img)
        {
            BitmapImage bmp = new BitmapImage();
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png);
                ms.Position =0;
                

                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource = null;
                bmp.StreamSource = ms;


                bmp.EndInit();
            }
            return bmp;
        }
    }
}
