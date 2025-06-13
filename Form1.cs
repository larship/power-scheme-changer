using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PowerSchemeChanger
{
    public partial class Form1 : Form
    {
        private Guid[] schemeGuidsList;
        private string[] schemeNamesList;
        private Guid activePowerSchemeGuid = Guid.Empty;

        public Form1()
        {
            InitializeComponent();
        }

        public void fillPowerSchemes(IEnumerable<Guid> schemeGuids, IEnumerable<string> schemeNames)
        {
            this.schemeGuidsList = schemeGuids.ToArray();
            this.schemeNamesList = schemeNames.ToArray();

            foreach (string schemeName in schemeNames)
            {
                comboBox1.Items.Add(schemeName);
            }
        }

        public void updateActivePowerScheme(Guid activePowerSchemeGuid)
        {
            for (int i = 0; i < this.schemeGuidsList.Length; i++)
            {
                if (activePowerSchemeGuid.Equals(this.schemeGuidsList[i]))
                {
                    comboBox1.SelectedIndex = i;
                    label3.Text = this.schemeNamesList[i];
                    break;
                }
            }

            this.activePowerSchemeGuid = activePowerSchemeGuid;

            updateApplyButtonEnabledState();
        }

        private void updateApplyButtonEnabledState()
        {
            button2.Enabled = this.activePowerSchemeGuid != Guid.Empty && activePowerSchemeGuid != this.schemeGuidsList[comboBox1.SelectedIndex];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Guid guidToActivate = this.schemeGuidsList[comboBox1.SelectedIndex];

            if (this.activePowerSchemeGuid == guidToActivate)
            {
                return;
            }

            Program.PowerSetActiveScheme(IntPtr.Zero, guidToActivate);
            updateActivePowerScheme(guidToActivate);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateApplyButtonEnabledState();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
