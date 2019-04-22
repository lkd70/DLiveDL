using NReco.VideoConverter;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;

namespace DLiveDL
{
    public partial class frmMain : Form
    {
        FFMpegConverter ffmpeg = new FFMpegConverter();
        string selected_format = Format.mp4;
        string file_name = "";

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            ffmpeg.ConvertProgress += UpdateProgress;
            comboBox1.Items.Add("rawvideo");
            comboBox1.Items.Add("webm");
            comboBox1.Items.Add("mov");
            comboBox1.Items.Add("flv");
            comboBox1.Items.Add("mp4");
            comboBox1.SelectedIndex = 4;
        }

        private void brnDownload_Click(object sender, EventArgs e)
        {
            if (!isUrlValid(txtURL.Text))
            {
                MessageBox.Show("URL isn't valid. Please copy and paste the URL exactly as it is shown.");
                return;
            }

            if (sfdBrowse.FileName == "")
            {
                MessageBox.Show("Please click 'browse' and find a suitable directory and name for your file.");
                return;
            }
            else
            {
                file_name = sfdBrowse.FileName;
            }

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                ffmpeg.ConvertMedia(txtURL.Text, file_name, selected_format);
            }).Start();
        }

        private void btnBrowseFile_Click(object sender, EventArgs e)
        {
            sfdBrowse.DefaultExt = comboBox1.SelectedItem.ToString();
            sfdBrowse.ShowDialog();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selected_format = Format.mp4;
            if (comboBox1.SelectedIndex == 0) selected_format = Format.raw_video;
            if (comboBox1.SelectedIndex == 1) selected_format = Format.webm;
            if (comboBox1.SelectedIndex == 2) selected_format = Format.mov;
            if (comboBox1.SelectedIndex == 3) selected_format = Format.flv;
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            ffmpeg.Abort();
        }

        private void UpdateProgress(object sender, ConvertProgressEventArgs e)
        {
            lblProgress.BeginInvoke((MethodInvoker)delegate () {
                lblProgress.Text = "Progress: " + e.Processed.ToString() + " / " + e.TotalDuration.ToString();
                if (e.Processed == e.TotalDuration)
                {
                    lblProgress.Text = "Progress: Done";
                    MessageBox.Show("Download completed");
                }
            });

        }

        private static double Divide(TimeSpan dividend, TimeSpan divisor)
        {
            return dividend.Ticks / divisor.Ticks;
        }

        private bool isUrlValid(string url)
        {
            Regex url_reg = new Regex(@"https:\/\/playback.prd.dlivecdn.com\/live\/[a-zA-Z0-9-_]+\/\d+\/(?:vod|master\/stitched-playlist).m3u8");
            Match match = url_reg.Match(url);
            return match.Success;
        }
    }
}
