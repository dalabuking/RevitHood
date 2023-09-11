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
using RevitHood.Functions;
using System.Security.Cryptography;
using System.Windows.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows;
using System.Security.AccessControl;
using System.Windows.Media;

namespace RevitHood
{
    public partial class ColorAssigner : System.Windows.Forms.Form
    {


        private UIDocument uiDoc;
        private Autodesk.Revit.ApplicationServices.Application oApp;
        private Document oDoc;
        private Random rnd = new Random();
        private int numberOfAsigns = 0;
        CategoriesAppa cats;
        ProjectParameters parameters;
        private List<string> parameterValues = new List<string>();
        private List<string> uniqueParameterValues = new List<string>();
        private string parameterN;

        public bool isBack = true; 


        private bool isOverride = false;


        public List<paraElement> elList;

        private UIApplication uiApp;

        //public static SortedList<string, List<Element>> elementsToColor = new SortedList<string, List<Element>>();

        Dictionary<string, List<Element>> elementsToColor
       = new Dictionary<string, List<Element>>();

        public ColorAssigner(UIApplication uiApp, string parameterName)
        {
            InitializeComponent();

            parameterN = parameterName;

            this.uiApp = uiApp;
            uiDoc = uiApp.ActiveUIDocument;
            oApp = uiApp.Application;
            oDoc = uiDoc.Document;

            cats = new CategoriesAppa(uiApp);
            parameters = new ProjectParameters(uiApp);

            DataGridViewColumn parameterLists = new DataGridViewTextBoxColumn();
            {
                parameterLists.Width = 150;
                parameterLists.HeaderText = parameterName;
                parameterLists.Name = parameterName;


            }
            parameterLists.ReadOnly = true;
            dataGridView1.Columns.Add(parameterLists);



            DataGridViewButtonColumn button = new DataGridViewButtonColumn();
            {
                button.FlatStyle = FlatStyle.Flat;
           
                button.Width = 70;
                button.Name = "Color";
                
                button.Selected = false;
                
                button.UseColumnTextForButtonValue = true; //dont forget this line

            }


            dataGridView1.Columns.Add(button);


            DataGridViewColumn numberOfElements = new DataGridViewTextBoxColumn();
            {
                numberOfElements.Width = 60;
                numberOfElements.HeaderText = "Count";
                numberOfElements.Name = "num";


            }

            numberOfElements.ReadOnly = true;
            dataGridView1.Columns.Add(numberOfElements);
            int rowId;


            getAllParameterValuesOfElements(parameterName);

            uniqueParameterValues.Sort();


            foreach (string para in elementsToColor.Keys)
            {

                var num = elementsToColor[para].Count();
                rowId = dataGridView1.Rows.Add();
                DataGridViewRow row = dataGridView1.Rows[rowId];
                row.Cells[parameterName].Value = para.ToString();


                row.Cells["num"].Value = num.ToString();
            }

            asignRandomColors();

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeRows = false;
        }

        private void getAllParameterValuesOfElements(string parametarName)
        {

            List<Element> AllElem = new FilteredElementCollector(oDoc, uiDoc.ActiveView.Id).WhereElementIsViewIndependent().Where(e => e.Category.CategoryType == CategoryType.Model).ToList();
            foreach (Element el in AllElem)
            {
                var value = el.LookupParameter(parametarName)?.AsValueString();
                if (value == null)
                {
                    continue;
                }
                parameterValues.Add(value);

                if (!elementsToColor.ContainsKey(value))
                {
                    elementsToColor.Add(value,
                        new List<Element>());
                }
                elementsToColor[value].Add(el);
            }
            uniqueParameterValues = parameterValues.Distinct().ToList();
        }

        private void asignColorToElements(List<Element> elementsToColor, System.Drawing.Color colorToAsign)
        {


            FilteredElementCollector elements = new FilteredElementCollector(oDoc);

            List<Material> materials = new List<Material>(
       new FilteredElementCollector(oDoc)
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
            foreach (Element el in elementsToColor)
            {


                try
                {
                    using (Transaction tx = new Transaction(oDoc))
                    {
                        tx.Start("Change Element Color");
                        oDoc.ActiveView.SetElementOverrides(el.Id, ogs);
                        //el.Category.Material = materials[0];
                        tx.Commit();

                        numberOfAsigns += 1;
                    }





                }
                catch
                {

                }

            }


            isOverride = false;
        }

        private void asignRandomColors()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {

                System.Drawing.Color randomColor;
                var cell = ((DataGridViewButtonCell)row.Cells[1]);
                var cell0 = ((DataGridViewCell)row.Cells[0]);

                randomColor = System.Drawing.Color.FromArgb(rnd.Next(220), rnd.Next(220), rnd.Next(220));
                cell.Style.BackColor = randomColor;

            }

        }



        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {

                colorDialog1.AllowFullOpen = true;
                colorDialog1.ShowDialog();

                dataGridView1.Rows[e.RowIndex].Cells[1].Style.BackColor = colorDialog1.Color;
                //dataGridView1.Rows[e.RowIndex].Cells[0].Style.BackColor = colorDialog1.Color;
                dataGridView1.ClearSelection();
                //TODO - Button Clicked - Execute Code Here
                asignColors();
            }



        }

        private void button1_Click(object sender, EventArgs e)
        {
            asignRandomColors();
            asignColors();
        }

        private void button2_Click(object sender, EventArgs e)
        {



            isOverride = true;


        }

        private void asignColors()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var cellName = ((DataGridViewCell)row.Cells[0]);
                var cell = ((DataGridViewButtonCell)row.Cells[1]);
                List<Element> toColor = new List<Element>();
                if (cellName.Value.ToString() == "")
                {

                }
                else
                {
                    toColor = elementsToColor[cellName.Value.ToString()];
                    asignColorToElements(toColor, cell.Style.BackColor);
                }
            }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {




        }

        private void removeColors(List<Element> elementsToColor)
        {

            OverrideGraphicSettings ogs = new OverrideGraphicSettings();
            foreach (Element el in elementsToColor)
            {
                try
                {
                    using (Transaction tx = new Transaction(oDoc))
                    {
                        tx.Start("Change Element Color");
                        oDoc.ActiveView.SetElementOverrides(el.Id, ogs);
                        //el.Category.Material = materials[0];
                        tx.Commit();


                    }

                }
                catch
                {

                }

            }
        }








        private void removeCols() 
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var cellName = ((DataGridViewCell)row.Cells[0]);
                var cell = ((DataGridViewButtonCell)row.Cells[1]);
                List<Element> toColor = new List<Element>();
                if (cellName.Value.ToString() == "")
                {

                }
                else
                {
                    toColor = elementsToColor[cellName.Value.ToString()];
                    removeColors(toColor);
                }
            }

        }

        private void ColorAssigne_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.MessageBox.Show("Closing");
        }

        private void ColorAssigne_FormClosing(object sender, FormClosingEventArgs e)
        {

            System.Windows.MessageBox.Show("Closing2");
        }

        void ColorAssigne_FormClosed(object sender, FormClosedEventArgs e)
        {
        
            if (!isOverride)
            {
                removeCols();
            }


            // Do something
        }



        private void ColorAssigner_Load(object sender, EventArgs e)
        {
            isBack = false;
            asignColors();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            isBack = true;
            this.Close();
            
       

        }
    }
    public class paraElement
    {

        public string name;
        public List<Element> elements;
        public paraElement(string name)
        {
              this.name = name;
                


        }
    }
}
