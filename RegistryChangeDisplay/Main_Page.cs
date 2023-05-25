using DiffLib;
using System.Diagnostics;
using System.Globalization;

namespace Registry_Change_Display
{

    public partial class Registry_Change_Recorder : Form
    {

        internal static SharedTypes _st { get; set; }

        public Registry_Change_Recorder()
        {
            InitializeComponent();

            _st = new SharedTypes();
        }


        private void Image_Registry_Click(object sender, EventArgs e)
        {
            //create the files and then pipe to them the data for the base snapshot.
            //I figure the file should be open to read/write data to/from it.

            if (File.Exists(_st.HKCU_Init_FilePath) && (File.Exists(_st.HKLM_Init_FilePath)))
            {
                Task.Factory.StartNew(async () =>
                 {

                     try
                     {
                         // try starting two powershell consoles and reading the registry into them

                         //OpenOrCreate, ReadWrite
                         await using (File.Open(_st.HKCU_Today_FilePath, (FileMode)4, FileAccess.ReadWrite))
                         {
                             startProcess();
                             _st.process.StartInfo.Arguments += _st.HKCU_Today_Dump_Command;
                             _st.process.Start();
                             _st.process.Close();
                         };

                         //OpenOrCreate, ReadWrite
                         await using (File.Open(_st.HKLM_Today_FilePath, (FileMode)4, FileAccess.ReadWrite))
                         {
                             startProcess();
                             _st.process.StartInfo.Arguments += _st.HKLM_Today_Dump_Command;
                             _st.process.Start();
                             _st.process.Close();
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
                        await using (File.Open(_st.HKCU_Init_FilePath, (FileMode)4, FileAccess.ReadWrite))
                        {

                            startProcess();
                            _st.process.StartInfo.Arguments += _st.HKCU_Init_Command;
                            _st.process.Start();
                            _st.process.Close();
                        };


                        //File Mode 4 is OpenOrCreate
                        await using (File.Open(_st.HKLM_Init_FilePath, (FileMode)4, FileAccess.ReadWrite))
                        {
                            startProcess();
                            _st.process.StartInfo.Arguments += _st.HKLM_Init_Command;
                            _st.process.Start();
                            _st.process.Close();
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

                    if (_st.File1_Path != string.Empty)
                    {
                        // if file1 is loaded
                        _st.File1_string_array = await File.ReadAllLinesAsync(_st.File1_Path);

                        if (_st.File2_Path != string.Empty)
                        {
                            _st.File2_string_array = await File.ReadAllLinesAsync(_st.File2_Path);
                        }
                        else
                        {
                            _st.File2_string_array = await File.ReadAllLinesAsync(_st.HKCU_Today_FilePath);
                            _st.File2_string_array = await File.ReadAllLinesAsync(_st.HKLM_Today_FilePath);
                        }
                    }
                    else
                    {
                        _st.File1_string_array = await File.ReadAllLinesAsync(_st.HKCU_Init_FilePath);
                        _st.File1_string_array = await File.ReadAllLinesAsync(_st.HKLM_Init_FilePath);
                    }
                }
                finally
                {

                    if (_st.File2_string_array != null && _st.File1_string_array != null)
                    {
                        var diffedSections = Diff.CalculateSections(_st.File1_string_array, _st.File2_string_array);
                        var changedRegistryValues = Diff.AlignElements(_st.File1_string_array, _st.File2_string_array, diffedSections, new StringSimilarityDiffElementAligner());

                        foreach (var item in changedRegistryValues)
                        {
                            _st.changes.Append(item.ToString());
                        }
                    }                    
                }
            });
        }

        
        Process startProcess()
        {
            _st.process = new Process();
            _st.process.StartInfo.UseShellExecute = false;
            _st.process.StartInfo.CreateNoWindow = true;
            
            _st.process.StartInfo.RedirectStandardInput = true;
            _st.process.StartInfo.RedirectStandardOutput = true;
            _st.process.StartInfo.RedirectStandardError = true;
            
            _st.process.StartInfo.FileName = "PowerShell.exe";
            
            _st.process.StartInfo.Arguments = null;
            return _st.process;
        }

        private void Save_File_Click(object sender, EventArgs e)
        {
            DialogResult saveresult = saveFileDialog1.ShowDialog();
            saveFileDialog1.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            if (saveresult == DialogResult.OK)
            {
                try
                {
                    string path = saveFileDialog1.FileName;

                    _st.changes_FilePath = path;

                    if (_st.changes != null)
                    {
                        File.WriteAllLines(path, _st.changes);
                    }
                    else
                    {
                        MessageBox.Show("No changes to save.");
                    }
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
                    _st.File1_Path = file1;
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
                    _st.File2_Path = file2;
                }
                catch (IOException)
                {
                    MessageBox.Show("error loading File 2");
                }
            }
            Console.WriteLine(size); // <-- Shows file size in debugging mode.
            Console.WriteLine(result); // <-- For debugging use.
        }

        private void Filter_Button_Click(object sender, EventArgs e)
        {
            var filter_Page = new Filter_Page();
            filter_Page.Show();
        }
    }
}
