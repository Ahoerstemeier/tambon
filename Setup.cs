using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace De.AHoerstemeier.Tambon
{
    public partial class Setup : Form
    {
        protected String HTMLCacheDir { get; set; }
        protected String XMLOutputDir { get; set; }
        protected String PDFDir { get; set; }
        public Setup()
        {
            InitializeComponent();
            GlobalSettings.LoadSettings();
            HTMLCacheDir = GlobalSettings.HTMLCacheDir;
            edtHTMLCacheDir.Text = HTMLCacheDir;
            XMLOutputDir = GlobalSettings.XMLOutputDir;
            edtXMLOutputDir.Text = XMLOutputDir;
            PDFDir = GlobalSettings.PDFDir;
            edtPDFDir.Text = PDFDir;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            GlobalSettings.HTMLCacheDir = HTMLCacheDir;
            GlobalSettings.PDFDir = PDFDir;
            GlobalSettings.XMLOutputDir = XMLOutputDir;
            GlobalSettings.SaveSettings();
        }

        private void edtHTMLCacheDir_TextChanged(object sender, EventArgs e)
        {
            HTMLCacheDir = edtHTMLCacheDir.Text;
        }

        private void edtXMLOutputDir_TextChanged(object sender, EventArgs e)
        {
            XMLOutputDir = edtXMLOutputDir.Text;
        }

        private void edtPDFDir_TextChanged(object sender, EventArgs e)
        {
            PDFDir = edtPDFDir.Text;
        }
    }
}
