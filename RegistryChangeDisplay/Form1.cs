using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using DiffMatchPatch;

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

        // Where to write the file containing the changes from base to current
        string changes_FilePath = string.Format(@"{0}\changes.txt", Path.GetDirectoryName(Application.ExecutablePath));



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
                    string HKCUtext1 = await File.ReadAllTextAsync(HKCU_Current_FilePath);
                    string HKCUtext2 = await File.ReadAllTextAsync(HKCU_Init_FilePath);
                    string HKLMtext1 = await File.ReadAllTextAsync(HKLM_Current_FilePath);
                    string HKLMtext2 = await File.ReadAllTextAsync(HKLM_Init_FilePath);
                    List<Diff> changes = new List<Diff>();

                    diff_match_patch dmp = new diff_match_patch();
                    changes.AddRange(dmp.diff_main(HKCUtext1, HKCUtext2));
                    changes.AddRange(dmp.diff_main(HKLMtext1, HKLMtext2));

                    List<string> convertedDiffs = new List<string>();
                    foreach (var diff in changes)
                    {
                        //inserted, deleted...
                        convertedDiffs.Add(diff.text + " " + diff.operation.ToString());
                    }

                    using (File.Open(changes_FilePath, (FileMode)4, FileAccess.ReadWrite))
                    {
                        File.WriteAllLines(changes_FilePath, convertedDiffs);
                    }
                }

                catch (Exception)
                {
                    throw;
                }
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

            process.StartInfo.Arguments = null;
            return process;
        }
    }
}
