using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using DiffMatchPatch;

namespace Registry_Change_Display
{
    public partial class Registry_Change_Recorder : Form
    {
        Process process;

        //init is on first run
        string HKCU_Init_FilePath { get; }
        string HKLM_Init_FilePath { get; }
        string HKCU_Init_Command { get; }
        string HKLM_Init_Command { get; }

        // today is current information
        string HKCU_Today_FilePath { get; }
        string HKLM_Today_FilePath { get; }
        string HKCU_Today_Dump_Command { get; }
        string HKLM_Today_Dump_Command { get; }


        // The files selected in the loader
        string File1_Path { get; set; }
        string File2_Path { get; set; }


        // The text in the files to diff
        string[] File1_string_array { get; set; }
        string[] File2_string_array { get; set; }


        // changes is the diff between loaded and base
        string changes_FilePath { get; }

        IEnumerable<string> changes { get; set; }

        // diffCollection is the array of different entries
        List<string> diffCollection { get; set; }

        public Registry_Change_Recorder()
        {
            InitializeComponent();

            // the base files from the first time the app is run on a fresh install
            HKCU_Init_FilePath = string.Format(@"{0}\Base-HKCU.txt", Path.GetDirectoryName(Application.ExecutablePath));
            HKLM_Init_FilePath = string.Format(@"{0}\Base-HKLM.txt", Path.GetDirectoryName(Application.ExecutablePath));
            HKCU_Init_Command = string.Format(@"dir -rec -erroraction ignore HKCU:\ | % name > {0}\Base-HKCU.txt", Path.GetDirectoryName(Application.ExecutablePath));
            HKLM_Init_Command = string.Format(@"dir -rec -erroraction ignore HKLM:\ | % name > {0}\Base-HKLM.txt", Path.GetDirectoryName(Application.ExecutablePath));

            // the registry dump with todays date
            HKCU_Today_FilePath = string.Format(@"{0}\Current-HKCU-{1}.txt",
                Path.GetDirectoryName(Application.ExecutablePath), DateTime.Now.ToString("ddMMyyyy", CultureInfo.InvariantCulture));

            HKLM_Today_FilePath = string.Format(@"{0}\Current-HKLM-{1}.txt",
                Path.GetDirectoryName(Application.ExecutablePath), DateTime.Now.ToString("ddMMyyyy", CultureInfo.InvariantCulture));

            HKCU_Today_Dump_Command = string.Format(@"dir -rec -erroraction ignore HKCU:\ | % name > {0}\Current-HKCU-{1}.txt",
                                Path.GetDirectoryName(Application.ExecutablePath), DateTime.Now.ToString("ddMMyyyy", CultureInfo.InvariantCulture));

            HKLM_Today_Dump_Command = string.Format(@"dir -rec -erroraction ignore HKLM:\ | % name >  {0}\Current-HKLM-{1}.txt", Path.GetDirectoryName(Application.ExecutablePath), DateTime.Now.ToString("ddMMyyyy", CultureInfo.InvariantCulture));



            changes_FilePath = string.Format(@"{0}\changes.txt", Path.GetDirectoryName(Application.ExecutablePath));
        }


        private void Image_Registry_Click(object sender, EventArgs e)
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
                         await using (File.Open(HKCU_Today_FilePath, (FileMode)4, FileAccess.ReadWrite))
                         {
                             startProcess();
                             process.StartInfo.Arguments += HKCU_Today_Dump_Command;
                             process.Start();
                             process.Close();
                         };

                         //OpenOrCreate, ReadWrite
                         await using (File.Open(HKLM_Today_FilePath, (FileMode)4, FileAccess.ReadWrite))
                         {
                             startProcess();
                             process.StartInfo.Arguments += HKLM_Today_Dump_Command;
                             process.Start();
                             process.Close();
                         };
                     }
                     catch (Exception)
                     {
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
                        throw;
                    }
                });
            }
        }


        private void Diff_File1_File2_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(async () =>
            {

                try
                {

                    if (File1_Path != string.Empty)
                    {
                        // if file1 is loaded
                        File1_string_array = await File.ReadAllLinesAsync(File1_Path);

                        if (File2_Path != string.Empty)
                        {
                            File2_string_array = await File.ReadAllLinesAsync(File2_Path);
                        }
                        else
                        {
                            File2_string_array = await File.ReadAllLinesAsync(HKCU_Today_FilePath);
                            File2_string_array = await File.ReadAllLinesAsync(HKLM_Today_FilePath);
                        }
                    }
                    else
                    {
                        File1_string_array = await File.ReadAllLinesAsync(HKCU_Init_FilePath);
                        File1_string_array = await File.ReadAllLinesAsync(HKLM_Init_FilePath);
                    }
                }
                finally
                {
                    changes = File2_string_array.Except(File1_string_array);


                    //List<Diff> changes = new List<Diff>();

                    //diff_match_patch dmp = new diff_match_patch();


                    //for (int i = 0; i < File2_string_array.Length; --i)
                    //{
                    //   var diff = dmp.diff_main(File1_string_array[i], File2_string_array[i]) ;
                    //    foreach (var item in diff)
                    //    {
                    //        if (item.operation != Operation.EQUAL)
                    //        {
                    //            changes.Add(item);
                    //            File3_string_array.Append(File2_string_array[i]);
                                
                    
                    // I couldn't find a way to remove the compared line from the array.


                    //        }
                    //    }
                    //}
                    //List<string> convertedDiffs = new List<string>();
                    //foreach (var diff in changes)
                    //{
                    //    //inserted, deleted...
                    //    convertedDiffs.Add(diff.text + " " + diff.operation.ToString());
                    //}

                    //diffCollection = convertedDiffs;
                    
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

        private async void Save_File_Click(object sender, EventArgs e)
        {
            DialogResult saveresult = saveFileDialog1.ShowDialog();
            saveFileDialog1.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            if (saveresult == DialogResult.OK)
            {
                try
                {
                    string path = saveFileDialog1.FileName;
                    File.WriteAllLines(path, changes);
                    StreamWriter sw = new StreamWriter(path);
                    foreach (var item in diffCollection)
                    {
                       await sw.WriteLineAsync(item);
                    }
                    sw.Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("There was an issue saving the file.");
                }
            }
        }

        private void Load_File1_Click(object sender, EventArgs e)
        {
            int size = -1;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file1 = openFileDialog1.FileName;
                try
                {
                    File1_Path = file1;
                }
                catch (IOException)
                {
                    MessageBox.Show("error loading File 1");
                }
            }
            Console.WriteLine(size); // <-- Shows file size in debugging mode.
            Console.WriteLine(result); // <-- For debugging use.
        }

        private void Load_File2_Click(object sender, EventArgs e)
        {
            int size = -1;
            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            openFileDialog2.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            DialogResult result = openFileDialog2.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file2 = openFileDialog2.FileName;
                try
                {
                    File2_Path = file2;
                }
                catch (IOException)
                {
                    MessageBox.Show("error loading File 2");
                }
            }
            Console.WriteLine(size); // <-- Shows file size in debugging mode.
            Console.WriteLine(result); // <-- For debugging use.
        }
    }
}
