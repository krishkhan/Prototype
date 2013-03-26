using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;

namespace SupportArrayType
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            CustomGridView custom_gv = new CustomGridView(); 
            AttributeCollection attributes = TypeDescriptor.GetAttributes(custom_gv);

            /* Prints the name of the designer by retrieving the DesignerAttribute
             * from the AttributeCollection. */
            DesignerAttribute myAttribute =
               (DesignerAttribute)attributes[typeof(DesignerAttribute)];
            MessageBox.Show("The designer for this class is: " + myAttribute.DesignerTypeName);

            
            Application.Run(new Form1());


        }
    }
}
