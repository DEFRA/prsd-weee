namespace EA.Weee.Sroc.Migration.UI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class Form1 : Form
    {
        private readonly IUpdateProducerCharges updateProducerCharges;

        public Form1(IUpdateProducerCharges updateProducerCharges)
        {
            InitializeComponent();

            this.updateProducerCharges = updateProducerCharges;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            updateProducerCharges.Test();
        }
    }
}
