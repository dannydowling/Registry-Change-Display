using DiffLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RegistryChangeTracker
{
    public partial class Form1 : Form
    {
        internal static SharedTypes _st { get; set; }
        public Form1()
        {
            InitializeComponent();
            _st = new SharedTypes();

            resetListBox();
        }

        private void resetListBox()
        {
            if (_st.changes_FilePath != null)
            {
                using (StreamReader r = new StreamReader(_st.changes_FilePath))
                {
                    string line;
                    while ((line = r.ReadLine()) != null)
                    {
                        listBox1.Items.Add(line);

                    }
                }
            }
        }

        private void Update_Button_Clicked(object sender, EventArgs e)
        {

            //if the registry has been imaged at least once, then we can create new files to compare to that base registry image.

            if (File.Exists(_st.Init_FilePath))
            {
                Task.Factory.StartNew(() =>
                {

                    try
                    {
                        // try starting two powershell consoles and reading the registry into them

                        //OpenOrCreate, ReadWrite
                        using (File.Open(_st.Today_Registry_Image_FilePath, (FileMode)4, FileAccess.ReadWrite))
                        {
                            // start process is a method in this file.
                            startProcess();

                            _st.process.StartInfo.Arguments = "";
                            _st.process.StartInfo.Arguments += _st.HKCU_Today_Dump_Command;
                            _st.process.Start();
                            string HKCU_Output = _st.process.StandardOutput.ReadToEnd();
                            _st.process.Close();

                            // clear out any commands
                            _st.process.StartInfo.Arguments = "";
                            _st.process.StartInfo.Arguments += _st.HKLM_Today_Dump_Command;
                            _st.process.Start();
                            string HKLM_Output = _st.process.StandardOutput.ReadToEnd();
                            _st.Unformatted_Powershell_Output_Registry_image_base = new Tuple<string, string>(HKCU_Output, HKLM_Output);
                            _st.process.Close();

                            

                            _st.process.WaitForExit();
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
                Task.Factory.StartNew(() =>
                {
                    // otherwise, we're creating the base registry image files

                    try
                    {
                        //File Mode 4 is OpenOrCreate
                        using (File.Open(_st.Init_FilePath, (FileMode)4, FileAccess.ReadWrite))
                        {

                            startProcess();

                            _st.process.StartInfo.Arguments = "";
                            _st.process.StartInfo.Arguments += _st.HKCU_Init_Command;
                            _st.process.Start();
                            string HKCU_Output = _st.process.StandardOutput.ReadToEnd();
                            _st.process.Close();

                            // clear out any commands
                            _st.process.StartInfo.Arguments = "";
                            _st.process.StartInfo.Arguments += _st.HKLM_Init_Command;
                            _st.process.Start();
                            string HKLM_Output = _st.process.StandardOutput.ReadToEnd();
                            _st.Unformatted_Powershell_Output_Registry_image_base = new Tuple<string, string>(HKCU_Output, HKLM_Output);
                            _st.process.Close();

                            _st.process.WaitForExit();
                        };
                    }
                    catch (FileNotFoundException f)
                    {
                        MessageBox.Show(f.Message + " File not found exception. ");

                        OpenFileDialog openFileDialog1 = new OpenFileDialog();
                        openFileDialog1.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
                        openFileDialog1.Multiselect = true; // Allow multiple file selection

                        DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.

                        if (result == DialogResult.OK) // Test result.
                        {
                            foreach (string file in openFileDialog1.FileNames)
                            {
                                listBox1.Items.Add(file);
                                _st.OpenFilePaths.Add(file);
                            }
                        }
                    }
                });
            }
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


        private void ProcessFiles()
        {
            Task.Run(() =>
            {
                try
                {
                    List<string[]> fileContents = new List<string[]>();

                    foreach (string item in _st.OpenFilePaths)
                    {
                       fileContents.Add(File.ReadAllLines(item));
                    }

                    for (int i = 0; i < fileContents.Count; i--)
                    {
                        // Calculate the differences between the two files
                        var diffedSections = Diff.CalculateSections(fileContents[i], fileContents[i--]);
                        var changedRegistryValues = Diff.AlignElements(fileContents[i], fileContents[i--], diffedSections, new StringSimilarityDiffElementAligner());

                        foreach (var item in changedRegistryValues)
                        {
                            _st.changes.Append(item.ToString());
                        }
                    }

                    // Dump the Registry values
                    if (File.Exists(_st.HKCU_Init_FilePath) && (File.Exists(_st.HKLM_Init_FilePath)))
                    {
                        DumpValuesToFile("HKCU", _st.HKCU_Today_Dump_Command, _st.Today_Registry_Image_FilePath);
                        DumpValuesToFile("HKLM", _st.HKLM_Today_Dump_Command, _st.HKLM_Today_FilePath);
                    }
                    else
                    {
                        DumpValuesToFile("HKCU", _st.HKCU_Init_Command, _st.HKCU_Init_FilePath);
                        DumpValuesToFile("HKLM", _st.HKLM_Init_Command, _st.HKLM_Init_FilePath);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });
        }

        private void DumpValuesToFile(string hive, string dumpCommand, string filePath)
        {
            try
            {
                // File Mode 4 is OpenOrCreate
                using (File.Open(filePath, (FileMode)4, FileAccess.ReadWrite))
                {
                    startProcess();
                    _st.process.StartInfo.Arguments += dumpCommand;
                    _st.process.Start();
                    _st.process.Close();
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        //private void startProcess()
        //{
        //    _st.process = new Process();
        //    _st.process.StartInfo.FileName = "powershell.exe";
        //    _st.process.StartInfo.UseShellExecute = false;
        //    _st.process.StartInfo.RedirectStandardOutput = true;
        //    _st.process.StartInfo.RedirectStandardError = true;
        //    _st.process.StartInfo.CreateNoWindow = true;
        //}



        private void ListView_Selected_Index_Changed(object sender, EventArgs e)
        {
           
        }

        private void SaveButton_Click(object sender, EventArgs e)
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

       
    }
}