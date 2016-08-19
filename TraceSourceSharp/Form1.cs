using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TraceSourceSharp
{
    public partial class Form1 : Form
    {
        private readonly string configFileName = "config.xml";
        public Form1()
        {
            InitializeComponent();
            Source.DataCenter.main.StatusEvent += UpdateStatus;
            View.ViewCenter.main.Width = 800;
            View.ViewCenter.main.Height = 800;
            timer1.Start();
        }

        private void ResetForm()
        {
            this.Text = "NotSet,No Working Directory";
        }

        public  void UpdateStatus(string info)
        {
            if(this.InvokeRequired)
            {
                this.BeginInvoke((Action)(() => this.UpdateStatus(info)));
            }
            else
            {
                if(info.StartsWith("(1)"))
                {
                    toolStripStatusLabel1.Text = info.Substring(3);
                }
                else if(info.StartsWith("(2)"))
                {
                    toolStripStatusLabel2.Text = info.Substring(3);
                }
            }
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            View.ViewCenter.main.DrawBuffer(g);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if(keyData == Keys.F5)
            {
                Source.DataCenter.main.Reload();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SetWorkDirectory(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(Source.DataCenter.main.config.root))
            {
                MessageBox.Show(
                    @"the root directory can only be initialize once,
try run another instance for now folder"
                    ,"Already intialized");
                return;
            }

            string xpath = Application.StartupPath + @"\" + configFileName;

            string defPath = null;
            try
            {
                XDocument xdoc = XDocument.Load(xpath);
                var node = from item in xdoc.Descendants("folder")
                           where !string.IsNullOrEmpty(item.Value)
                           select item.Value;
                defPath = node.FirstOrDefault();
            }
            catch(Exception)
            {
                // yes,keep silent always
            }
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if(!string.IsNullOrEmpty(defPath))
            {
                fbd.SelectedPath = defPath;
            }
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Source.DataCenter.main.config.root = fbd.SelectedPath;
                toolStripTextBox1.Text = fbd.SelectedPath;
                DirectoryInfo di = new DirectoryInfo(fbd.SelectedPath);
                this.Text = di.Name + "@" + di.FullName;

                Action act = () => {
                    try
                    {
                        XDocument xdoc = null;
                        if (File.Exists(xpath))
                        {
                            xdoc = XDocument.Load(xpath);
                        }
                        else
                        {
                            xdoc = new XDocument();
                        }
                        var node = from item in xdoc.Descendants("folder")
                                   select item;
                        if (node.Count() == 0)
                        {
                            xdoc.Add(new XElement("folder", fbd.SelectedPath));
                        }
                        else
                        {
                            node.First().Value = fbd.SelectedPath;
                        }
                        xdoc.Save(xpath);
                    }
                    catch (Exception)
                    {
                        // just keep silent for anything
                    }
                };
                Utils.SilenceTask.main.PostTask(act, true);
            }
            else
            {
                ResetForm();
            }
        }

        private void OnTimer(object sender, EventArgs e)
        {
            panel1.Invalidate();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
        }
    }
}
