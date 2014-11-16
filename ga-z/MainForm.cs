﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
namespace ga_z
{
    public partial class MainForm : Form 
    {
        FolderBrowserDialog FBD = new FolderBrowserDialog();
        String []files;
        String []dirs;
        FTP ftp = new FTP();
        bool Conn = false;
        Thread Th_Connect;
        
        public MainForm()
        {
            InitializeComponent();
            Port.SelectedIndex = 0;
            
        }        

        private void FolderOpen_Click(object sender, EventArgs e)
        {
            
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                FolderFileList.Items.Clear();
                FileAttributes attributes = File.GetAttributes(FBD.SelectedPath);
                files = Directory.GetFiles(FBD.SelectedPath);
                dirs = Directory.GetDirectories(FBD.SelectedPath);
                                
                 
                if (FBD.SelectedPath != "C:\\")
                {
                    ListViewItem lvi = new ListViewItem("..",0);
                    lvi.SubItems.Add("상위폴더");
                    lvi.SubItems.Add("폴더");
                    FolderFileList.Items.Add(lvi);
                }
                
                foreach (string dir in dirs)
                {
                    
                   ListViewItem lvi = new ListViewItem(Path.GetFileName(dir), 0);
                   lvi.SubItems.Add(Path.GetDirectoryName(dir));
                   lvi.SubItems.Add("폴더");
                   FolderFileList.Items.Add(lvi);
                  
                    
                }
                foreach (string file in files)
                {
                    ListViewItem lvi = new ListViewItem(Path.GetFileName(file), 1);
                    lvi.SubItems.Add(Path.GetDirectoryName(file));
                    lvi.SubItems.Add("파일");
                    lvi.SubItems.Add(Path.GetExtension(file));
                    FolderFileList.Items.Add(lvi);
                }
            }
        }

        private void FolderFileList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
                if (FolderFileList.SelectedItems.Count == 1)
                {
                    string ClickFile = FolderFileList.FocusedItem.SubItems[0].Text;
                                        
                    if (ClickFile == "..")
                    {
                        
                        string temp = FBD.SelectedPath;
                        string [] temp_path = temp.Split('\\');
                        temp = "";
                        for (int i = 0; i < temp_path.Count() - 2; i++)
                        {                            
                            temp = temp + temp_path[i] + "\\";
                        }
                        if (temp_path.Count() != 2)
                        {
                            temp = temp + temp_path[temp_path.Count() - 2];
                        }
                        else if (temp_path.Count() == 2)
                        {
                            temp = "C:\\";
                        }
                        FBD.SelectedPath=temp;
                       
                    }
                    else
                    {
                        string Clicktype = FolderFileList.FocusedItem.SubItems[2].Text;
                        if (Clicktype == "폴더")
                        {
                            string path = FolderFileList.FocusedItem.SubItems[1].Text;
                            FBD.SelectedPath = path + "\\" + ClickFile;
                        }
                    }
                   
                    FolderFileList.Items.Clear();
                    files = Directory.GetFiles(FBD.SelectedPath);
                    dirs = Directory.GetDirectories(FBD.SelectedPath);
                    
                    if (FBD.SelectedPath != "C:\\")
                    {
                        FolderFileList.Items.Add("..", 0);
                                      
                    }
                   
                    
                    foreach (string dir in dirs)
                    {
                        ListViewItem lvi = new ListViewItem(Path.GetFileName(dir),0);
                        lvi.SubItems.Add(Path.GetDirectoryName(dir));
                        lvi.SubItems.Add("폴더");
                        lvi.SubItems.Add(Path.GetExtension(dir));
                        FolderFileList.Items.Add(lvi);
                    }
                    foreach (string file in files)
                    {
                        ListViewItem lvi = new ListViewItem(Path.GetFileName(file), 1);
                        lvi.SubItems.Add(Path.GetDirectoryName(file));
                        lvi.SubItems.Add("파일");
                        lvi.SubItems.Add(Path.GetExtension(file));
                        FolderFileList.Items.Add(lvi);
                    }
            
                }
            
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            string port="";
            if (Port.Text == "FTP")
            {
                port = "21";
            }
            else if(Port.Text == "SFTP")
            {
                port = "22";
            }
            
            if (Conn == false)
            {
                Th_Connect = new Thread(connectForm.show);
                Th_Connect.Start();
                Conn = ftp.Connected(0, Host.Text, User.Text, Password.Text, port, FTPListview);
                connectForm.close();
                Connect.Text = "접속끊기"; 
            }
            else if (Conn == true)
            {
                Conn = ftp.DisConnenct(FTPListview);
                Connect.Text = "접속"; 
            }            
            
        }

        private void FTPListview_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (FTPListview.SelectedItems.Count == 1)
            {
                ftp.DoubleClick(FTPListview,FTPListview.FocusedItem.SubItems[0].Text,FTPListview.FocusedItem.SubItems[3].Text);
            }
        }

        private void 업로드ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FolderFileList.SelectedItems.Count == 1 && Conn == true)
            {
                string localpath = FolderFileList.FocusedItem.SubItems[1].Text + "\\" + FolderFileList.FocusedItem.SubItems[0].Text;
                ftp.Upload(localpath);
            }
        }              
    }
}
