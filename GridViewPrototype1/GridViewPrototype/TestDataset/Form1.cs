using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestDataset
{
     
    public partial class Form1 : Form
    {
        private ObjectList _objectList = new ObjectList(); 
        
        public Form1()
        {
            InitializeComponent();
            

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'variableDetailsDataSet.VariableTable' table. You can move, or remove it, as needed.
            this.variableTableTableAdapter.Fill(this.variableDetailsDataSet.VariableTable);

            this.variableDetailsDataSetBindingSource.DataSource = _objectList.GetListOfObjects();

        }
    }
}
