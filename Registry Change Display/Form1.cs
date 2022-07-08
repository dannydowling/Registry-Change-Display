using System.Diagnostics;
using System.Text;

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
            try
            {
                string command1 = string.Format(@"dir -rec -erroraction ignore HKLM:\ | %name > {0}\Base - HKLM.txt", path);
                StringBuilder builder = new StringBuilder();
                builder.Append(command1);
                run_PowerShell_Command(builder.ToString());
            }
            catch (Exception)
            {

            }
            finally
            {
                string command2 = string.Format(@"dir -rec -erroraction ignore HKCU:\ | % name > {0}\Base - HKCU.txt", path);
                StringBuilder builder = new StringBuilder();
                builder.Append(command2);
                run_PowerShell_Command(builder.ToString());
            }
        }

        StreamReader standardOutput;
        public void run_PowerShell_Command(string command)
        {
            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.FileName = "PowerShell.exe";

            process.Start();
            process.StandardInput.WriteLine(command);
            standardOutput = process.StandardOutput;
            process.CloseMainWindow();
            process.Dispose();
        }

        private void List_Changes_Click(object sender, EventArgs e)
        {
            string path = Path.GetDirectoryName(Application.ExecutablePath);

            StringBuilder builder = new StringBuilder();
            try
            {
                string current_registry_HKCU_command = string.Format(
                    $@"dir -rec -erroraction ignore HKCU:\ | % name > {0}/Current-HKCU-$(get-date -f yyyy-MM-dd).txt", path);
                builder.Clear();
                builder.Append(current_registry_HKCU_command);
                run_PowerShell_Command(builder.ToString());

                string current_registry_HKLM_command = string.Format(
                    $@"dir -rec -erroraction ignore HKLM:\ | % name > {0}/Current-HKLM-$(get-date -f yyyy-MM-dd).txt", path);
                builder.Clear();
                builder.Append(current_registry_HKLM_command);
                run_PowerShell_Command(builder.ToString());
            }            
            catch (Exception)
            {

            }
            finally
            {
                string compare_HKCU_registry_changes_command = string.Format(
                    $@"Compare-Object (Get-Content -Path .\Base-HKCU.txt)(Get-Content-Path .\{0}/Current-HKCU-$(get-date -f yyyy-MM-dd).txt)", path);
                builder.Clear();
                builder.Append(compare_HKCU_registry_changes_command);
                run_PowerShell_Command(builder.ToString());

                string compare_HKLM_registry_changes_command = string.Format(
                    $@"Compare-Object (Get-Content -Path .\Base-HKLM.txt)(Get-Content-Path .\{0}/Current-HKLM-$(get-date -f yyyy-MM-dd).txt)", path);
                builder.Clear();
                builder.Append(compare_HKLM_registry_changes_command);
                run_PowerShell_Command(builder.ToString());

                using (standardOutput)
                {
                    foreach (var line in standardOutput.ReadToEnd().Split("/n"))
                    {
                        listBox1.Items.Add(line);
                    }
                }
            }
        }
    }
}