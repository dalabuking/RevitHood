using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;

using System.Text.RegularExpressions;

namespace RevitHood
{
    public partial class ChangeParameterForm : System.Windows.Forms.Form
    {

        private string parameterN;
        EditableParameters parameters;

        private UIDocument uiDoc;
        private Autodesk.Revit.ApplicationServices.Application oApp;
        private Document oDoc;
        private UIApplication uiApp; 



        Dictionary<string, List<Element>> elementsWithValue;

        Dictionary<string, List<string>> allParameterValues;


        List<string> keysValue;

        public ChangeParameterForm(UIApplication uiApp, string parameterName)
        {
            InitializeComponent();

            this.elementsWithValue = new Dictionary<string, List<Element>>();
            this.allParameterValues = new Dictionary<string, List<string>>();


            this.parameterN = parameterName;


            this.uiApp = uiApp;
            uiDoc = uiApp.ActiveUIDocument;
            oApp = uiApp.Application;
            oDoc = uiDoc.Document;


            DataGridViewButtonColumn elementLists = new DataGridViewButtonColumn();
            {
                elementLists.Width = 50;
                elementLists.HeaderText = "Selection";
                elementLists.Name = "Selection";
                elementLists.ReadOnly = true;
                elementLists.Text = "Select";
                elementLists.UseColumnTextForButtonValue = true; //dont forget this line


            }


            dataGridView1.Columns.Add(elementLists);

            parameters = new EditableParameters(uiApp);


            DataGridViewComboBoxColumn parameterLists = new DataGridViewComboBoxColumn();
            {
                parameterLists.Width = 250;
                parameterLists.HeaderText = parameterName;
                parameterLists.Name = parameterName;


            }
         
            dataGridView1.Columns.Add(parameterLists);

            DataGridViewColumn countList = new DataGridViewTextBoxColumn();
            {
                countList.Width = 50;
                countList.HeaderText = "Count";
                countList.Name = "num";


            }

            dataGridView1.Columns.Add(countList);





            DataGridViewButtonColumn changeParemeterButton = new DataGridViewButtonColumn();
            {
                changeParemeterButton.FlatStyle = FlatStyle.Flat;

                changeParemeterButton.Width = 70;
                changeParemeterButton.Name = "Apply";
                changeParemeterButton.Text = "Apply";
                changeParemeterButton.Selected = false;

                changeParemeterButton.UseColumnTextForButtonValue = true; //dont forget this line

            }
            dataGridView1.Columns.Add(changeParemeterButton);
            DataGridViewButtonColumn detailViewButton = new DataGridViewButtonColumn();
            {
                detailViewButton.FlatStyle = FlatStyle.Flat;

                detailViewButton.Width = 70;
                detailViewButton.Text = "Details >";
                detailViewButton.Name = "Details";
                detailViewButton.Selected = false;

                detailViewButton.UseColumnTextForButtonValue = true; //dont forget this line

            }
            dataGridView1.Columns.Add(detailViewButton);

            getAllParameterValuesOfElements(parameterName);

            int rowId;

            keysValue= new List<string>( elementsWithValue.Keys);

          


            foreach (string para in keysValue)
            {
                rowId = dataGridView1.Rows.Add();
                DataGridViewRow row = dataGridView1.Rows[rowId];
                //row.Cells[parameterName].Value = para.ToString();

                DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)(row.Cells[parameterName]);
                cell.DataSource = allParameterValues[para].Distinct().ToList(); 
                cell.Value = para.ToString();
                var num = elementsWithValue[para].Count();
   
                row.Cells["num"].Value = num.ToString();

          

            }

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeRows = false;


        }

        private void getAllParameterValuesOfElements(string parameterName)
        {

            List<Element> AllElem = new FilteredElementCollector(oDoc, uiDoc.ActiveView.Id).WhereElementIsViewIndependent().Where(e => e.Category.CategoryType == CategoryType.Model).ToList();





          //  AllElem = AllElem.OfType<Element>().ToList();

            
            foreach (Element el in AllElem)
            {



                FilteredElementCollector elementsWithType = new FilteredElementCollector(oDoc);
                elementsWithType.OfCategoryId(el.Category.Id).Where(e => e.GetType() == el.GetType()).ToList();

                

                  Parameter Para = el.LookupParameter(parameterName);
             

                if (Para == null) { continue; };


             
               

               var value = Para.AsValueString();

                if(value == null)
                {
                    value = "null";
                }
            



                if (!elementsWithValue.ContainsKey(value))
                {
                    elementsWithValue.Add(value,
                        new List<Element>());

                    allParameterValues.Add(value,
                       new List<string>());
                    foreach (Element el2 in elementsWithType)
                    {
                        Parameter Para2 = el2.LookupParameter(parameterName);
                        if (Para2 == null) { continue; };
                        var value2 = Para2.AsValueString();
                        if (value2 == null)
                        {
                            value2 = "null";
                        }
                       
                        allParameterValues[value].Add(value2);
                    }
                }
                else
                {

                }
                elementsWithValue[value].Add(el);

                
          
            }

        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            

            if (senderGrid.Columns[e.ColumnIndex].HeaderText == "Apply")
            {
             

                var oldValue = keysValue[e.RowIndex];
                var cells = dataGridView1.Rows[e.RowIndex].Cells;

                string newValue = cells[parameterN].Value.ToString();

           

                if (oldValue != newValue)
                {
                    foreach (Element el in elementsWithValue[keysValue[e.RowIndex]])
                    {

                        using (Transaction tx = new Transaction(oDoc))
                        {
                            tx.Start("Change Parameter Values");
                            Parameter para = el.LookupParameter(parameterN);
                            if (para.IsReadOnly)
                            {
                                
                                tx.Commit();
                                continue;
                            }
                       
                            

                            if (para.StorageType == StorageType.String)
                            {
                                para.SetValueString(newValue);
                             
                            }
                            else if  (para.StorageType == StorageType.Double)
                            {
                                newValue = Regex.Replace(newValue, "[^0-9.]", "");
                        
                                para.Set(double.Parse(newValue) / 304.8);
                            }

                            else if (para.StorageType == StorageType.Integer)
                            {
                                newValue = Regex.Replace(newValue, "[^0-9.]", "");
                           
                                para.Set(int.Parse(newValue)/ 304.8);
                            }

                            //el.Category.Material = materials[0];
                            tx.Commit();


                        }
                       
                    }
                }




            }
            else if (senderGrid.Columns[e.ColumnIndex].HeaderText == "Selection")
            {
               
                List<ElementId> elementsIds = new List<ElementId>();

                foreach (Element el in elementsWithValue[keysValue[e.RowIndex]])
                {
                    elementsIds.Add(el.Id);
                }
               
                uiDoc.Selection.SetElementIds(elementsIds);
                uiDoc.ShowElements(elementsIds);
            }
        }
    }
}
