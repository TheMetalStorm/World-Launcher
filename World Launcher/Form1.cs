using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using World_Launcher.Properties;

namespace World_Launcher
{
    public partial class WorldLauncher : Form
    {
        string smwRomPath;
        string emulatorPath;
        string patchFolderPath;
        string thisFolderPath = Application.ExecutablePath.Replace("World Launcher.exe", "");
        string patchedRomsFolder;

        public WorldLauncher()
        {
            InitializeComponent();
            this.Text = "World Launcher";

            lv_game.MouseDoubleClick += new MouseEventHandler(lv_game_MouseDoubleClick);
            this.Load += new EventHandler(Form1_Load);

            //Creates Folder for patched ROMs (if it does not exist yet)
            System.IO.Directory.CreateDirectory(thisFolderPath + "PatchedRoms");
            patchedRomsFolder = thisFolderPath + "PatchedRoms";

            //Remember Lcoation of last used Rom File
            smwRomPath = Settings.Default["smwRomPath"].ToString();
            emulatorPath = Settings.Default["emulatorPath"].ToString();
            patchFolderPath = Settings.Default["patchFolderPath"].ToString();

            //checkBox1.Checked = Boolean.Parse(Settings.Default["DeletePatchesCheckbox"].ToString());
            FullscreenCheckBox.Checked = Boolean.Parse(Settings.Default["FullscreenCheckBox"].ToString());
        }

        #region Form1Methods
        private void Form1_Resize(object sender, System.EventArgs e)
        {
            Control control = (Control)sender;

            // Ensure the Form remains square (Height = Width).
            if (control.Size.Height != control.Size.Width)
            {
                control.Size = new Size(control.Size.Width, control.Size.Width);
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            DisplayFolder();

        }
        #endregion

        #region Lv_game
        private void lv_game_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lv_game_MouseDoubleClick(object sender, MouseEventArgs e)

        {

            if (emulatorPath == null || emulatorPath == "")
            {
                MessageBox.Show("Location of the Emulator has not been chosen yet", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                ListViewHitTestInfo info = lv_game.HitTest(e.X, e.Y);
                ListViewItem item = info.Item;

                if (item != null)
                {
                    string prompt;
                    if (FullscreenCheckBox.Checked) prompt = emulatorPath + " -fullscreen" + @" """ + patchedRomsFolder + "\\" + item.Text + @"""";
                    else prompt = emulatorPath + @" """ + patchedRomsFolder + "\\" + item.Text + @"""";
                    Process cmd = new Process();
                    cmd.StartInfo.FileName = "cmd.exe";
                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.Start();

                    cmd.StandardInput.WriteLine(prompt);
                    cmd.StandardInput.Flush();
                    cmd.StandardInput.Close();
                    cmd.WaitForExit();
                    //MessageBox.Show("The selected Item Name is: " + item.Text);
                }
                else
                {
                    this.lv_game.SelectedItems.Clear();
                    MessageBox.Show("No Item is selected");
                }
            }


        }




        public void DisplayFolder()
        {
            string[] files = Directory.GetFiles(patchedRomsFolder);

            for (int x = 0; x < files.Length; x++)
            {

                DisplayFile(files[x].Substring(patchedRomsFolder.Length + 1));
            }
        }

        public void DisplayFile(string fileName)
        {
            ListViewItem item = lv_game.FindItemWithText(fileName);
            if (item == null)
            {
                lv_game.Items.Add(fileName);
            }

        }

        #endregion

        #region Buttons
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "smc file | *.smc";
            openFileDialog1.ShowDialog();
            smwRomPath = openFileDialog1.FileName;
            Settings.Default["smwRomPath"] = smwRomPath;
            Settings.Default.Save();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            patchFolderPath = folderBrowserDialog1.SelectedPath;
            Settings.Default["patchFolderPath"] = patchFolderPath;
            Settings.Default.Save();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog2.Filter = "Emulator | *.exe";
            openFileDialog2.ShowDialog();
            emulatorPath = openFileDialog2.FileName;
            Settings.Default["emulatorPath"] = emulatorPath;
            Settings.Default.Save();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (smwRomPath == null || smwRomPath == "")
            {
                MessageBox.Show("Location of the SMW Rom has not been chosen yet", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (patchFolderPath == null || patchFolderPath == "")
            {
                MessageBox.Show("Folder containing patches has not been chosen yet", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                PatchAll();
            }
        }

        #endregion

        #region Patching
        private void PatchAll()
        {
            //Unzip Patch Files from Zips
            List<String> allPatches = Directory.GetFiles(@patchFolderPath, "*.*", SearchOption.AllDirectories)
                                      .Where(file => new string[] { ".zip"}
                                      .Contains(Path.GetExtension(file)))
                                      .ToList();

            foreach (string patchPath in allPatches)
            {
                if (patchPath.Contains(".zip"))
                {
                    string zipPath = @patchPath;

                    using(ZipFile zip = ZipFile.Read(zipPath))
{
                         foreach (ZipEntry e in zip)
                              {
                                if(e.FileName.Contains(".ips") || e.FileName.Contains(".bps"))
                                try
                                {
                                    e.Extract(patchFolderPath, ExtractExistingFileAction.OverwriteSilently);
                                }
                                catch(IOException s)
                                {
                                    MessageBox.Show(s.Message, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                              }
                        }
                    
                } 
            }

            //Get all Patches
            allPatches = Directory.GetFiles(@patchFolderPath, "*.*", SearchOption.AllDirectories)
                                    .Where(file => new string[] { ".bps", ".ips"}
                                    .Contains(Path.GetExtension(file)))
                                    .ToList();

            //Patch each one
            foreach (string patchPath in allPatches)
            {
                Patch(patchPath);

            }

            //Delete file if CheckBox1 is checked
            /*if (checkBox1.Checked)
            {
                List<String> all = Directory.GetFiles(@patchFolderPath, "*.*", SearchOption.AllDirectories).ToList();
                foreach (string path in all)
                {
                    // Create a reference to a file.
                    FileInfo fi = new FileInfo(path);
                    //Wait until file is not in use anymore
                    while (IsFileLocked(fi)) { }
                    //Delete file
                    fi.Delete();
                }
                   
            }
            //else only delete created temp files
            else
            {*/
                List<String> all = Directory.GetFiles(@patchFolderPath, "*.temp", SearchOption.AllDirectories).ToList();
                foreach (string path in all)
                {
                    // Create a reference to a file.
                    FileInfo fi = new FileInfo(path);
                    //Wait until file is not in use anymore
                    while (IsFileLocked(fi)) { }
                    //Delete file
                    fi.Delete();
                }
            //}
        }
        private void Patch(string patchPath)
        {

            string name = "";
            //Get Name of Patch
            // get index of last \, which comes before the file name 
            int cutOff = patchPath.LastIndexOf(@"\");

            //check if bps or ips file, create substring of patchPath after cutOff, delete the original file ending and add .smc
            if (patchPath.Contains(".bps"))
            {
                name = patchPath.Substring(cutOff + 1).Replace(@".bps", "") + ".smc";

                callCMD(createCMDPrompt(name, patchPath));

            }
            else if (patchPath.Contains(".ips"))
            {
                name = patchPath.Substring(cutOff + 1).Replace(@".ips", "") + ".smc";
                callCMD(createCMDPrompt(name, patchPath));

            }

            if (name == "") return;
            
            DisplayFile(name);

        }

        string createCMDPrompt(string name, string patchPath)
        {
            string outputPath = @"""" + patchedRomsFolder + @"\" + name + @"""";
            return "flips --apply " + @"""" + patchPath + @"""" + " " + @"""" + smwRomPath + @"""" + " " + outputPath;
        }

        //Source: https://stackoverflow.com/questions/10504647/deleting-files-in-use by dknaack
        void callCMD(string prompt)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine(prompt);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
        }
        protected virtual bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
        #endregion

        #region CheckBox
       /* private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default["DeletePatchesCheckbox"] = checkBox1.Checked.ToString();
            Settings.Default.Save();
        }*/

        private void FullscreenCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default["FullscreenCheckBox"] = FullscreenCheckBox.Checked.ToString();
            Settings.Default.Save();
            
        }

        #endregion

        #region Draw
        //Source: https://docs.microsoft.com/en-us/dotnet/desktop/winforms/advanced/how-to-draw-text-on-a-windows-form?view=netframeworkdesktop-4.8 by Microsoft, modified
        /*public void DrawString(string text, float x, float y)
        {
            System.Drawing.Graphics formGraphics = this.CreateGraphics();
            string drawString = text;
            System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 10);
            System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
            formGraphics.DrawString(drawString, drawFont, drawBrush, x, y, drawFormat);
            drawFont.Dispose();
            drawBrush.Dispose();
            formGraphics.Dispose();
        }*/
        #endregion

    }
}
