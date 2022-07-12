using System.Diagnostics;
using System.Text;

namespace Registry_Change_Display
{
    public partial class Form1 : Form
    {
        string path = Path.GetDirectoryName(Application.ExecutablePath);
        string date = string.Format(DateTime.Now.ToShortDateString()).Replace(@"\", "-");
        public Form1()
        {
            InitializeComponent();
        }
        private void create_Initial_Snapshot_Click(object sender, EventArgs e)
        {
            //create the files and then pipe to them the data for the base snapshot.
            //I figure the file should be open to read/write data to/from it.

            string HKCU_Init_FilePath = string.Format(@"{0}\\..\Base-HKCU.txt", path);
                                                //OpenOrCreate, ReadWrite
            using (File.Open(HKCU_Init_FilePath, (FileMode)4, FileAccess.ReadWrite))
            {
                string command1 = string.Format(@"dir -rec -erroraction ignore HKCU:\ | %name > {0}", HKCU_Init_FilePath);
                StringBuilder builder = new StringBuilder();
                builder.Append(command1);
                run_PowerShell_Command(builder.ToString());
            };


            string HKLM_Init_FilePath = string.Format(@"{0}\\..\Base-HKLM.txt", path);
                                                //OpenOrCreate, ReadWrite
            using (File.Open(HKLM_Init_FilePath, (FileMode)4, FileAccess.ReadWrite))
            {
                string command2 = string.Format(@"dir -rec -erroraction ignore HKLM:\ | % name > {0}", HKLM_Init_FilePath);
                StringBuilder builder2 = new StringBuilder();
                builder2.Append(command2);
                run_PowerShell_Command(builder2.ToString());
            };
        }
            

        private void List_Changes_Click(object sender, EventArgs e)
        {
           
            string HKCU_Current_FilePath = string.Format(@"{0}\\..\Current-HKCU-{1}.txt", path, date);

                                                  //OpenOrCreate, ReadWrite
            using (File.Open(HKCU_Current_FilePath, (FileMode)4, FileAccess.ReadWrite))
            {
                StringBuilder Current_HKCU_stringbuilder = new StringBuilder();
                string current_registry_HKCU_command = string.Format(
                    $@"dir -rec -erroraction ignore HKCU:\ | % name > {0}", HKCU_Current_FilePath);
                Current_HKCU_stringbuilder.Clear();
                Current_HKCU_stringbuilder.Append(current_registry_HKCU_command);
                run_PowerShell_Command(Current_HKCU_stringbuilder.ToString());

                StringBuilder Compare_HKCU_stringbuilder = new StringBuilder();
                string compare_HKCU_registry_changes_command = string.Format(
                        @"Compare-Object (Get-Content -Path \\..\Base-HKCU.txt)(Get-Content-Path {0}", HKCU_Current_FilePath);
                Compare_HKCU_stringbuilder.Clear();
                Compare_HKCU_stringbuilder.Append(compare_HKCU_registry_changes_command);
                run_PowerShell_Command(Compare_HKCU_stringbuilder.ToString());
            };


            string HKLM_Current_FilePath = string.Format(@"{0}\\..\Current-HKLM-{1}.txt", path, date);
                                                    //OpenOrCreate, ReadWrite
            using (File.Open(HKLM_Current_FilePath,(FileMode)4, FileAccess.ReadWrite))
            {
                StringBuilder Current_HKLM_stringbuilder = new StringBuilder();
                string current_registry_HKLM_command = string.Format(
                        @"dir -rec -erroraction ignore HKLM:\ | % name > {0}", HKLM_Current_FilePath);
                Current_HKLM_stringbuilder.Clear();
                Current_HKLM_stringbuilder.Append(current_registry_HKLM_command);
                run_PowerShell_Command(Current_HKLM_stringbuilder.ToString());

                StringBuilder Compare_HKLM_stringbuilder = new StringBuilder();
                string compare_HKLM_registry_changes_command = string.Format(
                        @"Compare-Object (Get-Content -Path .\\..\Base-HKLM.txt)(Get-Content-Path {0})", HKLM_Current_FilePath);
                Compare_HKLM_stringbuilder.Clear();
                Compare_HKLM_stringbuilder.Append(compare_HKLM_registry_changes_command);
                run_PowerShell_Command(Compare_HKLM_stringbuilder.ToString());
            };
        }
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

            process.OutputDataReceived += (sender, args) => Display(args.Data);
            process.ErrorDataReceived += (sender, args) => Display(args.Data);

            process.CloseMainWindow();
            process.Dispose();
        }

        SynchronizationContext _syncContext;
        void Display(string output)
        {
            _syncContext.Post(_ => listBox1.Items.Add(output), null);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
