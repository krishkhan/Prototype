using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FlexiDataGridView
{
    public partial class Form1 : Form
    {
        private DataGridView.HitTestInfo hitTestGrid;
        private DataTable collapsed;
        private DataTable expanded;

        public Form1()
        {
            InitializeComponent();
            collapsed = new DataTable();
            expanded = new DataTable(); 
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            DataColumn dtc = new DataColumn("TreeNode");
            dtc.DataType = typeof(System.String);
            dtc.MaxLength = 1;
            collapsed.Columns.Add(dtc);

            dtc = new DataColumn("Name") ;
            dtc.DataType = typeof(System.String);
            collapsed.Columns.Add(dtc);    

            dtc = new DataColumn("Type");
            dtc.DataType = typeof(System.String);
            collapsed.Columns.Add(dtc);

            dtc = new DataColumn("Dimension");
            dtc.DataType = typeof(System.UInt16);
            collapsed.Columns.Add(dtc);    

            

            DataRow dr = collapsed.NewRow();
            dr[0] = "";
            dr[1] = "Var1";
            dr[2] = "BOOL";
            dr[3] = 0;
            collapsed.Rows.Add(dr);   

            dr = collapsed.NewRow();
            dr[0] = "+";
            dr[1] = "Var1";
            dr[2] = "BOOL";
            dr[3] = 2;
            collapsed.Rows.Add(dr);

            dataGridView1.DataSource = collapsed;
 
        }

        private void SetTableStyle(DataTable dtNew)
        {
            DataGridTableStyle tableStyle = new DataGridTableStyle();
            tableStyle.MappingName = dtNew.TableName;
            tableStyle.PreferredRowHeight = 22;
            tableStyle.RowHeadersVisible = false;
            
        }

        private void OnGridViewClicked(object sender, MouseEventArgs e)
        {
            hitTestGrid = dataGridView1.HitTest(e.X, e.Y);
            int selectedRow;
            if ((hitTestGrid.RowY  == -1) || (hitTestGrid.ColumnX  == -1))
            {
                return;
            }
            if (hitTestGrid.ColumnIndex   == 0)
            {
                selectedRow = hitTestGrid.RowIndex;
                BuildNewDataTable(selectedRow);
            }
        }


        private void BuildNewDataTable(int selectedRow)
        {
            //Get the value form the cell
            string treeNode = dataGridView1[selectedRow, 0].ToString();
            bool blnFlag = false;

            DataTable dtNew = null;
            switch (treeNode)
            {
                case "+":
                    dtNew = GetExpandedRows(selectedRow);
                    blnFlag = true;
                    break;
                case "-":
                    //dtNew = GetCollopseRows(selectedRow);
                    blnFlag = true;
                    break;
                default:
                    //Do nothing
                    break;
            }

            if (blnFlag == true)
            {
                dtNew.TableName = "BindingTable";
                //dataGrid1.DataSource = dtNew;
                //SetTableStyle(dtNew);
            }
        }
            
        private DataTable GetExpandedRows(int selectedRow)
        {
            DataTable destination = collapsed.Clone();

            return destination;
              
        }
    }
}
