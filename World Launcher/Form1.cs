using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.IO;
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

            checkBox1.Checked = Boolean.Parse(Settings.Default["DeletePatchesCheckbox"].ToString());
            FullscreenCheckBox.Checked = Boolean.Parse(Settings.Default["FullscreenCheckBox"].ToString());
        }

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
            else {
                //TODO: implement this
                ListViewHitTestInfo info = lv_game.HitTest(e.X, e.Y);
                ListViewItem item = info.Item;

                if (item != null)
                {
                    string prompt;
                    if (FullscreenCheckBox.Checked) prompt = emulatorPath + " -fullscreen" + @" """ + patchedRomsFolder + "\\" + item.Text + @"""";
                    else prompt = emulatorPath + @" """ + patchedRomsFolder + "\\" + item.Text + @"""" ;
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
                Debug.WriteLine(fileName);
                lv_game.Items.Add(fileName);
            }

        }

        #endregion

        #region Buttons
        private void button1_Click(object sender, EventArgs e)
        {
            
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
            else if(patchFolderPath == null || patchFolderPath == "")
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

            //TODO: Patch files that are in .zip format
            //Get all files in the right format 
            string[] allPatches = Directory.GetFiles(@patchFolderPath, "*.bps");
            foreach (string patchPath in allPatches)
             {
                Patch(patchPath);
                
            }
        }
        private void Patch(string patchPath)
        {
            //Get Name of Patch
            int cutOff = patchPath.LastIndexOf(@"\");
            string name = patchPath.Substring(cutOff).Replace(@".bps", "")+".smc";
            //Decide, where it should get saved
            string outputPath = @"""" + patchedRomsFolder + name + @"""";

            //compose promopt for CMD
            string prompt = "flips --apply "+ @"""" + patchPath + @"""" + " " + @"""" + smwRomPath + @"""" + " " + outputPath;

            //Calls Flips CLI which patches file and saves it in the PatchedRoms Folder
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

            DisplayFile(name.Substring(1));

            //Delete file if CheckBox1 is checked
            if (checkBox1.Checked)
            {
               
                // Create a reference to a file.
                FileInfo fi = new FileInfo(patchPath);
                //Wait until file is not in use anymore
                while (IsFileLocked(fi)){}
                //Delete file
                fi.Delete();
            }

        }

        //Source: https://stackoverflow.com/questions/10504647/deleting-files-in-use by dknaack
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
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default["DeletePatchesCheckbox"] = checkBox1.Checked.ToString();
            Settings.Default.Save();

        }

        private void FullscreenCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default["FullscreenCheckBox"] = FullscreenCheckBox.Checked.ToString();
            Settings.Default.Save();

        }

        #endregion


    }
}
