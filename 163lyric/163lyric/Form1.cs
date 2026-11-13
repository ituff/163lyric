using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace _163lyric
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Getbtn_Click(object sender, EventArgs e)
        {
            int iID;
            if (!int.TryParse(IDtb.Text.Trim(), out iID))
            {
                MessageBox.Show("请输入数字！");
                return;
            }

            NetEaseLyric lyric = new NetEaseLyric();
            LyricResult result = lyric.getLyric(iID);
            if (result.HasError)
            {
                MessageBox.Show(result.Error);
                return;
            }

            Lyrictb.Text = result.ToString();
            Clipboard.SetDataObject(Lyrictb.Text);
        }

        private void IDtb_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void Copybtn_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(Lyrictb.Text);
        }
    }
}
