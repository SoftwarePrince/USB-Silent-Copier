using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace USB_copyer
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        String rsf;

        private void Form2_Load(object sender, EventArgs e)
        {
          try            {
            Hide();
            var ikkt = (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"disk.txt"));
            rsf = (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"rsf.txt"));

            
            File.Delete (rsf);
            var tmph = new System.IO.StreamReader(ikkt);
            var rl = tmph.ReadToEnd();
            tmph.Close();

            var res = MessageBox.Show("Do You Want To Scan Data in "+rl+ "\n\nIf You don't Responce, I will Automatically Start Scanning in 10 sec!", "Xtrem ROX USB Scanner Antivirus", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            //if the file does not exist other form will know the User clicked No
            if (res == DialogResult.No)
            {
                File.Delete(ikkt);
                File.Delete(rsf);
            }
            else
            {
              File.Create(rsf);
            }

                Close();

            }       catch {return; }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //File.Create(rsf);
            //no need to create file close() exists msgbox as if clicked Cancel and executes responding code             
            Close();
        }
    }
}
