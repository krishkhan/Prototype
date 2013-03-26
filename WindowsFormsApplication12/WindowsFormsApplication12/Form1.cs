using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsFormsApplication12
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\Database1.mdf;Integrated Security=True;User Instance=True");
            con.Open();
            SqlCommand comm = new SqlCommand("select * from UserData",con);
            
            DataTable master = new DataTable();
            DataTable child = new DataTable(); 
           
            // Fill Table 2 with Data
            SqlDataAdapter da = new SqlDataAdapter(comm);
            da.Fill(master);
           
           // Fill Table1 with data 
            comm = new SqlCommand("select * from UserDetail",con);
            da.Fill(child);

            con.Close();

            DataSet ds = new DataSet(); 
           
            //Add two DataTables  in Dataset
            ds.Tables.Add(master);
            ds.Tables.Add(child);
            
            // Create a Relation in Memory
            DataRelation relation = new DataRelation("",ds.Tables[0].Columns[0],ds.Tables[1].Columns[0],true);
            ds.Relations.Add(relation);
            dataGrid1.DataSource = ds.Tables[0];
        
       }
    }
}
