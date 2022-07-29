using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Registry_Change_Display
{
    public partial class Filter_Page : Form
    {
        readonly SharedTypes _st;

        public Filter_Page()
        {
            InitializeComponent();

            try
            {
                listBox1.DataSource = File.ReadAllLines(_st.changes_FilePath);
            }
            catch (IOException)
            {
                MessageBox.Show("operation failed");
            }
            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        List<string> _filtered_lines;

        private void Filter_Button_Click(object sender, EventArgs e)
        {
            string filter = textBox1.Text;
            listBox1.Items.Clear();

            _filtered_lines = new List<string>();

            _filtered_lines = File.ReadAllLines(_st.changes_FilePath).Where(n => n.Contains(filter)).Select(m => m).ToList();

            listBox1.DataSource = _filtered_lines;
        }

        private void Search_Button_Click(object sender, EventArgs e)
        {
            string search = textBox1.Text;
            try
            {
                if (_filtered_lines != null)
                {
                    for (int i = listBox1.SelectedIndex; i < _filtered_lines.Count; i++)
                    {
                        listBox1.SelectedIndex = _filtered_lines.IndexOf(search);
                    }
                }
                else
                {
                    _filtered_lines = File.ReadAllLines(_st.changes_FilePath).Where(n => n.Contains(search)).Select(m => m).ToList();
                }
            }
            catch (Exception)
            {
                _filtered_lines = File.ReadAllLines(_st.changes_FilePath).ToList();
                MessageBox.Show("end of file reached.");
            }
        }
    }
}
