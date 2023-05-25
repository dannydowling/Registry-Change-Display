using System.Data;
using DiffLib;

namespace Registry_Change_Display
{
    public partial class Filter_Page : Form
    {        

        public Filter_Page()
        {
            InitializeComponent();
             
            resetListBox();            
        }

        private void resetListBox()
        {
          
            List<string> lines = new List<string>();
            if (Registry_Change_Recorder._st.changes_FilePath != null)
            {
                using (StreamReader r = new StreamReader(Registry_Change_Recorder._st.changes_FilePath))
                {
                    string? line;
                    while ((line = r.ReadLine()) != null)
                    {
                        listBox1.Items.Add(line);

                    }
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
            listBox1.Items.Clear();

            if (!string.IsNullOrEmpty(Registry_Change_Recorder._st.changes_FilePath))
            {
                Diff.CalculateSections(File.ReadAllLines(Registry_Change_Recorder._st.changes_FilePath), File.ReadAllLines(Registry_Change_Recorder._st.changes_FilePath));
            }











            //    if (!string.IsNullOrEmpty(Registry_Change_Recorder._st.changes_FilePath))
            //{
            //    _filtered_lines = File.ReadAllLines(Registry_Change_Recorder._st.changes_FilePath).Where(n => n.Contains(filter)).Select(m => m).ToList();
            //}
            //if (_filtered_lines != null)
            //{
            //    listBox1.Items.AddRange(_filtered_lines.ToArray());
            //}
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
                    _filtered_lines = File.ReadAllLines(Registry_Change_Recorder._st.changes_FilePath).Where(n => n.Contains(search)).Select(m => m).ToList();
                }
            }
            catch (Exception)
            {
                _filtered_lines = File.ReadAllLines(Registry_Change_Recorder._st.changes_FilePath).ToList();
                MessageBox.Show("end of file reached.");
            }
        }
    }
}
