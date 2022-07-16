using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Registry_Change_Display
{
    public partial class Form1 : Form
    {
        public event EventHandler<EventArgs> Changed;

        Process process;

        string HKCU_Init_FilePath = string.Format(@"{0}\Base-HKCU.txt", Path.GetDirectoryName(Application.ExecutablePath));

        string HKLM_Init_FilePath = string.Format(@"{0}\Base-HKLM.txt", Path.GetDirectoryName(Application.ExecutablePath));

        string changes_HKLM_FilePath = string.Format(@"{0}\changesHKLM.txt", Path.GetDirectoryName(Application.ExecutablePath));

        string changes_HKCU_FilePath = string.Format(@"{0}\changesHKCU.txt", Path.GetDirectoryName(Application.ExecutablePath));

        string HKCU_Current_FilePath =
            string.Format(@"{0}\Current-HKCU-{1}.txt", Path.GetDirectoryName(Application.ExecutablePath), DateTime.Now.ToString("ddMMyyyy",
                  CultureInfo.InvariantCulture));

        string HKLM_Current_FilePath =
            string.Format(@"{0}\Current-HKLM-{1}.txt", Path.GetDirectoryName(Application.ExecutablePath), DateTime.Now.ToString("ddMMyyyy",
                  CultureInfo.InvariantCulture));

        string HKCU_Init_Command =
            string.Format(@"dir -rec -erroraction ignore HKCU:\ | % name > {0}\Base-HKCU.txt",
                Path.GetDirectoryName(Application.ExecutablePath));

        string HKLM_Init_Command = string.Format(@"dir -rec -erroraction ignore HKLM:\ | % name > {0}\Base-HKLM.txt", Path.GetDirectoryName(Application.ExecutablePath));

        string current_registry_HKCU_command =
                            string.Format(@"dir -rec -erroraction ignore HKCU:\ | % name > {0}\Current-HKCU-{1}.txt",
                                Path.GetDirectoryName(Application.ExecutablePath), DateTime.Now.ToString("ddMMyyyy",
                  CultureInfo.InvariantCulture));

        string current_registry_HKLM_command =
                            string.Format(@"dir -rec -erroraction ignore HKLM:\ | % name >  {0}\Current-HKLM-{1}.txt", Path.GetDirectoryName(Application.ExecutablePath), DateTime.Now.ToString("ddMMyyyy",
                  CultureInfo.InvariantCulture));


        string compare_HKCU_registry_changes_command =
                    string.Format(@"Compare-Object (Get-Content -Path {0}\Base-HKCU.txt)(Get-Content -Path {0}\Current-HKCU-{1}.txt | % name > {0}\changesHKCU.txt", Path.GetDirectoryName(Application.ExecutablePath), DateTime.Now.ToString("ddMMyyyy",
                  CultureInfo.InvariantCulture));


        string compare_HKLM_registry_changes_command =
                    string.Format(@"Compare-Object (Get-Content -Path {0}\Base-HKLM.txt)(Get-Content -Path {0}\Current-HKLM-{1}.txt | % name > {0}\changesHKLM.txt", Path.GetDirectoryName(Application.ExecutablePath), DateTime.Now.ToString("ddMMyyyy",
                  CultureInfo.InvariantCulture));




        public Form1()
        {
            InitializeComponent();
        }
        private void create_Initial_Snapshot_Click(object sender, EventArgs e)
        {

            //create the files and then pipe to them the data for the base snapshot.
            //I figure the file should be open to read/write data to/from it.

            if (File.Exists(HKCU_Init_FilePath) && (File.Exists(HKLM_Init_FilePath)))
            {
                try
                {
                    // try starting two powershell consoles and reading the registry into them

                    //OpenOrCreate, ReadWrite
                    using (File.Open(HKCU_Current_FilePath, (FileMode)4, FileAccess.ReadWrite))
                    {
                        startProcess();
                        process.StartInfo.Arguments += current_registry_HKCU_command;
                        process.Start();
                        process.Close();
                    };

                    //OpenOrCreate, ReadWrite
                    using (File.Open(HKLM_Current_FilePath, (FileMode)4, FileAccess.ReadWrite))
                    {
                        startProcess();
                        process.StartInfo.Arguments += current_registry_HKLM_command;
                        process.Start();
                        process.Close();
                    };
                }
                catch (Exception)
                {
                    process.Close();
                    process.Dispose();
                    throw;
                }
            }
            else
            {
                try
                {
                    //OpenOrCreate, ReadWrite
                    using (File.Open(HKCU_Init_FilePath, (FileMode)4, FileAccess.ReadWrite))
                    {

                        startProcess();

                        process.StartInfo.Arguments += HKCU_Init_Command;
                        process.Start();
                        process.Close();
                    };


                    //OpenOrCreate, ReadWrite
                    using (File.Open(HKLM_Init_FilePath, (FileMode)4, FileAccess.ReadWrite))
                    {
                        startProcess();


                        process.StartInfo.Arguments += HKLM_Init_Command;
                        process.Start();
                        process.Close();
                    };
                }
                catch (Exception)
                {
                    process.Close();
                    process.Dispose();
                    throw;
                }
            }
        }


        private void List_Changes_Click(object sender, EventArgs e)
        {
            try
            {
                using (File.Open(HKCU_Current_FilePath, (FileMode)4, FileAccess.Read))
                {
                    using (File.Open(changes_HKCU_FilePath, (FileMode)4, FileAccess.ReadWrite))
                    {
                        startProcess();

                        process.StartInfo.Arguments += compare_HKCU_registry_changes_command;

                        process.Start();
                        process.Close();
                    }
                }

                using (File.Open(HKLM_Current_FilePath, (FileMode)4, FileAccess.Read))
                {
                    using (File.Open(changes_HKLM_FilePath, (FileMode)4, FileAccess.ReadWrite))
                    {



                        startProcess();
                        process.StartInfo.Arguments += compare_HKLM_registry_changes_command;
                        process.Start();
                        process.Close();
                    }
                }
            }
            catch (Exception)
            {
                process.Close();
                process.Dispose();
                throw;
            }
        }

        Process startProcess()
        {
            process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.StartInfo.FileName = "PowerShell.exe";
            process.OutputDataReceived += (sender, args) => Display(sender, args.Data);
            process.ErrorDataReceived += (sender, args) => Display(sender, args.Data);
            process.StartInfo.Arguments = null;
            return process;
        }



        SynchronizationContext _syncContext;
        void Display(object s, string args)
        {
            _syncContext.Post(_ => listBox1.Items.Add(args), s);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
