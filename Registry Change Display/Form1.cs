using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

namespace Registry_Change_Display
{
    public partial class Registry_Change_Recorder : Form
    {

        // changes are the things that have changed between registry images
        public ObservableCollection<string> changes;

        // process is an instance of PowerShell
        Process process;

        // Where to write the base snapshot for HKCU 
        string HKCU_Init_FilePath = string.Format(@"{0}\Base-HKCU.txt", Path.GetDirectoryName(Application.ExecutablePath));
        // Where to write the base snapshot for HKLM
        string HKLM_Init_FilePath = string.Format(@"{0}\Base-HKLM.txt", Path.GetDirectoryName(Application.ExecutablePath));
        
        
        // Where to write the current snapshot for HKCU
        string HKCU_Current_FilePath =
            string.Format(@"{0}\Current-HKCU-{1}.txt", Path.GetDirectoryName(Application.ExecutablePath), DateTime.Now.ToString("ddMMyyyy",
                  CultureInfo.InvariantCulture));

        // Where to write the current snapshot for HKLM
        string HKLM_Current_FilePath =
            string.Format(@"{0}\Current-HKLM-{1}.txt", Path.GetDirectoryName(Application.ExecutablePath), DateTime.Now.ToString("ddMMyyyy",
                  CultureInfo.InvariantCulture));


        // Command to write the HKCU information to the base file
        string HKCU_Init_Command = string.Format(@"dir -rec -erroraction ignore HKCU:\ | % name > {0}\Base-HKCU.txt", Path.GetDirectoryName(Application.ExecutablePath));

        // Command to write the HKLM information to the base file
        string HKLM_Init_Command = string.Format(@"dir -rec -erroraction ignore HKLM:\ | % name > {0}\Base-HKLM.txt", Path.GetDirectoryName(Application.ExecutablePath));

        // Command to write the HKCU information to a file with appended date and time
        string current_registry_HKCU_command =
                            string.Format(@"dir -rec -erroraction ignore HKCU:\ | % name > {0}\Current-HKCU-{1}.txt",
                                Path.GetDirectoryName(Application.ExecutablePath), DateTime.Now.ToString("ddMMyyyy", CultureInfo.InvariantCulture));

        // Command to write the HKLM information to a file with appended date and time
        string current_registry_HKLM_command =
                            string.Format(@"dir -rec -erroraction ignore HKLM:\ | % name >  {0}\Current-HKLM-{1}.txt", Path.GetDirectoryName(Application.ExecutablePath), DateTime.Now.ToString("ddMMyyyy", CultureInfo.InvariantCulture));


        // Command to compare the base HKCU file with the current registry snapshot file
        string compare_HKCU_registry_changes_command =
                    string.Format(@"Compare-Object (Get-Content -Path {0}\Base-HKCU.txt)(Get-Content -Path {0}\Current-HKCU-{1}.txt | % name > {0}\changesHKCU.txt", Path.GetDirectoryName(Application.ExecutablePath), DateTime.Now.ToString("ddMMyyyy",
                  CultureInfo.InvariantCulture));

        // Command to compare the base HKLM file with the current registry snapshot file
        string compare_HKLM_registry_changes_command =
                    string.Format(@"Compare-Object (Get-Content -Path {0}\Base-HKLM.txt)(Get-Content -Path {0}\Current-HKLM-{1}.txt | % name > {0}\changesHKLM.txt", Path.GetDirectoryName(Application.ExecutablePath), DateTime.Now.ToString("ddMMyyyy",
                  CultureInfo.InvariantCulture));

        // Where to write the file containing the changes from base to current for HKCU
        string changes_HKCU_FilePath = string.Format(@"{0}\changesHKCU.txt", Path.GetDirectoryName(Application.ExecutablePath));

        // Where to write the file containing the changes from base to current for HKLM
        string changes_HKLM_FilePath = string.Format(@"{0}\changesHKLM.txt", Path.GetDirectoryName(Application.ExecutablePath));


        public Registry_Change_Recorder()
        {
            InitializeComponent();
            // Form.Shown is run when the form is first displayed.
            Shown += OnShown;
        }

        private void OnShown(object? sender, EventArgs e)
        {
            // bind the listbox to the changes collection
            listBox1.DataSource = changes;
        }
        private void create_Initial_Snapshot_Click(object sender, EventArgs e)
        {
            //create the files and then pipe to them the data for the base snapshot.
            //I figure the file should be open to read/write data to/from it.

            if (File.Exists(HKCU_Init_FilePath) && (File.Exists(HKLM_Init_FilePath)))
            {
                Task.Factory.StartNew(async () =>
                 {

                     try
                     {
                        // try starting two powershell consoles and reading the registry into them

                        //OpenOrCreate, ReadWrite
                         await using (File.Open(HKCU_Current_FilePath, (FileMode)4, FileAccess.ReadWrite))
                         {
                             startProcess();
                             process.StartInfo.Arguments += current_registry_HKCU_command;
                             process.Start();
                             process.Close();
                         };

                        //OpenOrCreate, ReadWrite
                         await using (File.Open(HKLM_Current_FilePath, (FileMode)4, FileAccess.ReadWrite))
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
                 });
            }
            else
            {
                Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        //File Mode 4 is OpenOrCreate
                        await using (File.Open(HKCU_Init_FilePath, (FileMode)4, FileAccess.ReadWrite))
                        {

                            startProcess();
                            process.StartInfo.Arguments += HKCU_Init_Command;
                            process.Start();
                            process.Close();
                        };


                        //File Mode 4 is OpenOrCreate
                        await using (File.Open(HKLM_Init_FilePath, (FileMode)4, FileAccess.ReadWrite))
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
                });
            }
        }


        private void List_Changes_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    // File Mode 3 is Open only
                    await using (File.Open(HKCU_Current_FilePath, (FileMode)3, FileAccess.Read))
                    {
                        await using (File.Open(changes_HKCU_FilePath, (FileMode)4, FileAccess.ReadWrite))
                        {
                            startProcess();

                            process.StartInfo.Arguments += compare_HKCU_registry_changes_command;
                            process.OutputDataReceived += (sender, args) => Display(sender, args.Data);
                            process.ErrorDataReceived += (sender, args) => Display(sender, args.Data);
                            process.Start();
                            process.Close();
                        }
                    }

                    // File Mode 3 is Open only
                    await using (File.Open(HKLM_Current_FilePath, (FileMode)3, FileAccess.Read))
                    {
                        await using (File.Open(changes_HKLM_FilePath, (FileMode)4, FileAccess.ReadWrite))
                        {
                            startProcess();
                            process.StartInfo.Arguments += compare_HKLM_registry_changes_command;
                            process.OutputDataReceived += (sender, args) => Display(sender, args.Data);
                            process.ErrorDataReceived += (sender, args) => Display(sender, args.Data);
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
            });
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

        process.StartInfo.Arguments = null;
        return process;
    }

    SynchronizationContext _syncContext;
    void Display(object s, string args)
    {
        if (changes == null)
        {
            changes = new ObservableCollection<string>();
        }
        // add the item to the collection and use the syncContext to coordinate between threads.
        _syncContext.Post(_ => changes.Add(args), s);
    }
}
}
