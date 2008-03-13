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
            AHGlobalSettings.LoadSettings();
            HTMLCacheDir = AHGlobalSettings.HTMLCacheDir;
            edtHTMLCacheDir.Text = HTMLCacheDir;
            XMLOutputDir = AHGlobalSettings.XMLOutputDir;
            edtXMLOutputDir.Text = XMLOutputDir;
            PDFDir = AHGlobalSettings.PDFDir;
            edtPDFDir.Text = PDFDir;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            AHGlobalSettings.HTMLCacheDir = HTMLCacheDir;
            AHGlobalSettings.PDFDir = PDFDir;
            AHGlobalSettings.XMLOutputDir = XMLOutputDir;
            AHGlobalSettings.SaveSettings();
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
