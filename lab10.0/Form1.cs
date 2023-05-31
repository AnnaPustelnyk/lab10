using System.Media;
using WMPLib;

namespace lab10._0
{
    public partial class Form1 : Form
    {
        SoundPlayer player = null;
        private double lastPosition = 0;
        private List<string> list = new List<string>();
        private WindowsMediaPlayer Player = new WindowsMediaPlayer();
        private int currentSongIndex = 0;
        public Form1()
        {
            InitializeComponent();
            listView1.View = View.Details;
            listView1.Dock = DockStyle.Fill;
            this.Controls.Add(listView1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "MP3 Files (*.mp3)|*.mp3";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string s in ofd.FileNames)
                {
                    list.Add(s);
                    ListViewItem listItem = new ListViewItem(s);
                    listItem.Text = Path.GetFileName(s);
                    listView1.Items.Add(listItem);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                string fileName = item.Text;
                list.Remove(fileName);
                listView1.Items.Remove(item);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (Player.playState == WMPPlayState.wmppsPaused)
            {
                Player.controls.currentPosition = lastPosition;
                Player.controls.play();
            }
            else PlaySelectedSong();
            currentSongIndex = listView1.SelectedIndices[0];
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (Player.playState == WMPPlayState.wmppsPlaying)
            {
                lastPosition = Player.controls.currentPosition;
                Player.controls.pause();
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Player.controls.stop();
            timer1.Stop();
            label1.Text = "00:00";
            progressBar1.Value = 0;
        }

        private void PlaySelectedSong()
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string selectedFileName = listView1.SelectedItems[0].Text;
                string selectedFilePath = list.Find(x => Path.GetFileName(x) == selectedFileName);

                if (selectedFilePath != null)
                {
                    Player.URL = selectedFilePath;
                    Player.controls.play();
                    timer1.Start();
                }
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string selectedSong = listView1.SelectedItems[0].Text;
                PlaySelectedSong();
            }
        }

        private void progressBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (Player.playState == WMPPlayState.wmppsPlaying)
            {
                double newPosition = (e.X / (double)progressBar1.Width) * progressBar1.Maximum;
                Player.controls.currentPosition = newPosition;
            }
        }

        private void progressBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Player.playState == WMPPlayState.wmppsPlaying)
            {
                double newPosition = (e.X / (double)progressBar1.Width) * progressBar1.Maximum;
                Player.controls.currentPosition = newPosition;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Player.playState == WMPPlayState.wmppsPlaying)
            {
                TimeSpan currentTime = TimeSpan.FromSeconds(Player.controls.currentPosition);
                TimeSpan totalTime = TimeSpan.FromSeconds(Player.currentMedia.duration);

                label1.Text = currentTime.ToString(@"mm\:ss");
                label3.Text = Player.currentMedia.durationString;

                progressBar1.Maximum = (int)totalTime.TotalSeconds;
                progressBar1.Value = (int)currentTime.TotalSeconds;

                if ((int)currentTime.TotalSeconds == (int)totalTime.TotalSeconds - 1)
                {
                    Player.controls.pause();

                    int nextIndex = (currentSongIndex + 1) % listView1.Items.Count;
                    listView1.Items[nextIndex].Selected = true;
                    listView1.Items[nextIndex].EnsureVisible();

                    PlaySelectedSong();
                }
            }
        }
    }
}