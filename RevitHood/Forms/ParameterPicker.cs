using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace RevitHood
{

   
    public partial class ParameterPicker : System.Windows.Forms.Form
    {
        public bool isParameterPicked = false;
        public string pickedParameter = "";
        public ElementId pickedParameterId;
        private UIApplication uiApp;

        List<string> parametersList;
        public ParameterPicker(UIApplication uiApp, List<string> parametersList)
        {
            InitializeComponent();
            this.parametersList = parametersList;
            this.uiApp = uiApp;
           
            foreach (string para in parametersList)
            {
                listBox1.Items.Add(para);

            }


        }

        private void ParameterPicker_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                foreach (string para in parametersList)
                {
                    listBox1.Items.Add(para);

                }
            }
            listBox1.Items.Clear();

            foreach (string str in parametersList)
            {
                if (str.StartsWith(textBox1.Text, StringComparison.CurrentCultureIgnoreCase))
                {
                    listBox1.Items.Add(str);
                }
            }
        }





        private void listBox1_DoubleClick(object sender, System.EventArgs e)
        {

            if (listBox1.SelectedItem != null)
            {
              
                pickedParameter = listBox1.SelectedItem.ToString();
           
                isParameterPicked = true;
    
                this.Close();
            
               


            }


        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
