using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design; 

namespace SupportArrayType
{
    [Designer("System.Windows.Forms.Design.ControlDesigner, System.Windows.Forms.Design.DLL", typeof(IRootDesigner)),DesignerCategory("Form")]
    public partial class CustomGridView : DataGridView 
    {
        public CustomGridView()
        {
            InitializeComponent();
        }

        public CustomGridView(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
