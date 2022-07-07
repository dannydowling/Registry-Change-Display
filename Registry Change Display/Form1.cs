using System.Diagnostics;

namespace Registry_Change_Display
{
    public partial class Form1 : Form
    {
        List<string> changes = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void create_Initial_Snapshot_Click(object sender, EventArgs e)
        {
            string path = Path.GetDirectoryName(Application.ExecutablePath);

            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.FileName = "PowerShell.exe";           
            process.Start();
            process.WaitForInputIdle();
            process.StandardInput.WriteLine(@"dir -rec -erroraction ignore HKLM:\ | % name > {0}\Base-HKLM.txt", path);
            process.WaitForInputIdle();
            process.StandardInput.WriteLine(@"dir - rec - erroraction ignore HKCU:\ | % name > {0}\Base-HKCU.txt", path);
            process.CloseMainWindow();
            process.Dispose();
        }

        private void List_Changes_Click(object sender, EventArgs e)
        {
            string path = Path.GetDirectoryName(Application.ExecutablePath);

            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.FileName = "PowerShell.exe";
            process.Start();
            process.WaitForInputIdle();
            process.StandardInput.WriteLine(@"dir -rec -erroraction ignore HKCU:\ | % name > {0}\HKCU-{1}.txt", path, DateTime.Now.ToShortDateString());
            process.WaitForInputIdle();
            process.StandardInput.WriteLine(@"Compare -Object(Get-Content-Path.\Base-HKCU.txt)(Get-Content-Path.\{0}\HKCU-{1}.txt)", path, DateTime.Now.ToShortDateString());
            StreamReader reader = new StreamReader(process.StandardOutput.ReadToEnd());
            using (reader)
            {
                foreach (var line in reader.ReadLine())
                {
                    listBox1.Items.Add(line);
                }
            }

            process.CloseMainWindow();
            process.Dispose();
            
        }
    }
}