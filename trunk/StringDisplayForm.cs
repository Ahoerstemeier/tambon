﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace De.AHoerstemeier.Tambon
{
    public partial class StringDisplayForm : Form
    {
        public StringDisplayForm(String iCaption, String iContents)
        {
            InitializeComponent();
            this.Text = iCaption;
            txtBox.Text = iContents;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
