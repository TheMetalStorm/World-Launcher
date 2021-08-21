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

namespace World_Launcher
{
    public partial class WorldLauncher : Form
    {
        string smwRomPath = null;
        string emulatorPath = null;
        string patchFolderPath = null;

        public WorldLauncher()
        {
            InitializeComponent();
            this.Text = "World Launcher";
        }

        private void Form1_Resize(object sender, System.EventArgs e)
        {
            Control control = (Control)sender;

            // Ensure the Form remains square (Height = Width).
            if (control.Size.Height != control.Size.Width)
            {
                control.Size = new Size(control.Size.Width, control.Size.Width);
            }

            //TODO: Scale everything based on window size

            /*float scaleX = ((float)formNewWidth / formBaseWidth);
            float scaleY = ((float)formNewHeight / formBaseHeight);
            this.Scale(new SizeF(scaleX, scaleY));*/
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            //TODO: create internal folder to store patched games
            DisplayFolder("C:\\Users\\arapo_000\\Desktop\\Neuer Ordner", lv_game);

        }

        public void DisplayFolder(string folderPath, ListView lv)
        {
            string[] files = System.IO.Directory.GetFiles(folderPath);

            for (int x = 0; x < files.Length; x++)
            {
                lv.Items.Add(files[x].Substring(folderPath.Length+1));
            }
        }

        private void lv_game_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            smwRomPath = openFileDialog1.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            patchFolderPath = folderBrowserDialog1.SelectedPath;
        }

        
        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            emulatorPath = openFileDialog2.FileName;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (smwRomPath == null)
            {
                MessageBox.Show("Location of the SMW Rom has not been chosen yet.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if(patchFolderPath == null)
            {
                MessageBox.Show("Folder containing patches has not been chosen yet.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
                else if(emulatorPath == null)
            {
                MessageBox.Show("Location of the Emulator has not been chosen yet.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            else
            {
                
                //TODO: Patch files
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
           
        }



    }
}
