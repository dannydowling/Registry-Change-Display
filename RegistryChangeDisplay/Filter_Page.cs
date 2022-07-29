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
        SharedTypes _st;

        public Filter_Page()
        {
            InitializeComponent();

            SharedTypes st = new SharedTypes();
            _st = st;

            resetListBox();            
        }

        private void resetListBox()
        {
            listBox1.Items.Clear();

            List<string> lines = new List<string>();
            using (StreamReader r = new StreamReader(_st.changes_FilePath))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    listBox1.Items.Add(line);

                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = listBox1.SelectedIndex.ToString();
        }

        List<string> _filtered_lines;

        private void Filter_Button_Click(object sender, EventArgs e)
        {
            string filter = textBox1.Text;
            listBox1.Items.Clear();

            _filtered_lines = new List<string>();

            _filtered_lines = File.ReadAllLines(_st.changes_FilePath).Where(n => n.Contains(filter)).Select(m => m).ToList();

            listBox1.Items.Clear();
            foreach (var item in _filtered_lines)
            {
                listBox1.Items.Add(item);
            }            
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
