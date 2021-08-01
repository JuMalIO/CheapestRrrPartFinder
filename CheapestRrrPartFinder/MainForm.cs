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
        }

        private List<(decimal price, string url)> Items;

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length <= 0)
            {
                MessageBox.Show("Error: empty url!");
                return;
            }

            textBox1.Enabled = false;
            button1.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;

            listBox1.Items.Clear();

            Items = new Downloader().Download(textBox1.Text).OrderBy(x => x.price).ToList();

            foreach (var item in Items)
            {
                listBox1.Items.Add($"{item.price,20:F2}\t{item.url}");
            }

            textBox1.Enabled = true;
            button1.Enabled = true;
            Cursor.Current = Cursors.Default;
            SystemSounds.Asterisk.Play();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = ((ListBox)sender).SelectedIndex;

            if (index < Items.Count)
            {
                var url = Items[index].url;

                try
                {
                    Process.Start(url);
                }
                catch
                {
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
    }
}
