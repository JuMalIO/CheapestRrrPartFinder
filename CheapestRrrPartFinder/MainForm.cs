using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CheapestRrrPartFinder
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            WindowName = base.Text;
        }

        private string WindowName;
        private List<(decimal price, string url)> Items;

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length <= 0)
            {
                MessageBox.Show("Error: empty url!");
                return;
            }

            textBox1.Enabled = false;
            numericUpDown1.Enabled = false;
            button1.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;

            Items = new Downloader().Download(textBox1.Text, Update).OrderBy(x => x.price).ToList();

            Filter(textBox2.Text);

            textBox1.Enabled = true;
            numericUpDown1.Enabled = true;
            button1.Enabled = true;
            Cursor.Current = Cursors.Default;
            SystemSounds.Asterisk.Play();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = ((ListBox)sender).SelectedIndex;

            if (index < listBox1.Items.Count)
            {
                var url = ((string)listBox1.Items[index]).Split("\t")[1];

                try
                {
                    Process.Start(url);
                }
                catch
                {
                    Clipboard.SetText(url);

                    // hack because of this: https://github.com/dotnet/corefx/issues/10361
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        url = url.Replace("&", "^&");
                        Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        Process.Start("xdg-open", url);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        Process.Start("open", url);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Filter(((TextBox)sender).Text);
        }

        private void Filter(string text)
        {
            listBox1.Items.Clear();

            foreach (var item in Items)
            {
                if (string.IsNullOrWhiteSpace(text) || item.url.Contains(text))
                {
                    listBox1.Items.Add($"{item.price,20:F2}\t{item.url}");
                }
            }
        }

        private void Update(int page, int totalPages)
        {
            base.Text = $"{WindowName} {page} / {totalPages}";
            base.Refresh();
            System.Threading.Thread.Sleep((int)numericUpDown1.Value);
        }
    }
}
