using System.Globalization;
using System;
using System.Windows.Forms;

namespace EncodingNormalizerVsx.View
{
    [ProvideToolboxControl("EncodingNormalizerVsx.View.DefinitionPage", false)]
    public partial class DefinitionPage : UserControl
    {
        public DefinitionPage()
        {
            InitializeComponent();
        }

        public EncodingNormalizerVsx.DefinitionPage Owner { set; get; }

        private void Button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(string.Format(CultureInfo.CurrentUICulture, "We are inside {0}.Button1_Click()", this.ToString()));
        }
    }
}
