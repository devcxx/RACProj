using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioControlServer
{
    public partial class FormControl : Form
    {
        AudioServer mAudioServer;

        public FormControl()
        {
            InitializeComponent();
            initServer();

        }
        private void initServer()
        {
            mAudioServer = new AudioServer(Properties.Settings.Default.Port,Properties.Settings.Default.Key);
            mAudioServer.OnMedia += MAudioServer_OnMedia;
            mAudioServer.StartServer();
        }
        private void MAudioServer_OnMedia(AudioServer.MediaRequest request, int value = 0)
        {
            float volume = AudioManager.GetMasterVolume();
            if (request == AudioServer.MediaRequest.MUTE_TOGGLE)
            {
                AudioManager.ToggleMasterVolumeMute();
            }
            else if (request == AudioServer.MediaRequest.VOLUME_DOWN)
            {
                if (volume - 5 <= 0)
                    AudioManager.SetMasterVolume(0);
                AudioManager.SetMasterVolume(volume -5);
            }
            else if (request == AudioServer.MediaRequest.VOLUME_UP)
            {
                if (volume + 5 >= 100)
                    AudioManager.SetMasterVolume(100);
                AudioManager.SetMasterVolume(volume + 5);
            } else if (request == AudioServer.MediaRequest.VOLUME_SET)
            {
                if (value < 0 || value > 100)
                    return;
                AudioManager.SetMasterVolume(value);
            }
            else if (request == AudioServer.MediaRequest.MEDIA_PLAY)
            {
                MediaControl.PlayPause();
            }
            else if (request == AudioServer.MediaRequest.MEDIA_STOP)
            {
                MediaControl.Stop();
            }
            else if (request == AudioServer.MediaRequest.MEDIA_NEXT)
            {
                 MediaControl.NextTrack();
                
            }
            else if (request == AudioServer.MediaRequest.MEDIA_PREVIOUS)
            {                
                MediaControl.PreviousTrack();
            }
        }

        private void FormControl_Load(object sender, EventArgs e)
        {
            this.Hide();
            txtKey.Text = Properties.Settings.Default.Key;
            //PortNumber.Value = Properties.Settings.Default.Port;

        }
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            //Properties.Settings.Default.Key = txtKey.Text;
            //Properties.Settings.Default.Port = (int) PortNumber.Value;
            //Properties.Settings.Default.Save();
            //mAudioServer.StopServer();
            //mAudioServer.OnMedia -= MAudioServer_OnMedia;
            //mAudioServer = null;
            //initServer(); 
            //trayicon.ShowBalloonTip(1000,"Audio Control Server", $"Listening On : {PortNumber.Value}", ToolTipIcon.Info);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mAudioServer.StopServer();
            Environment.Exit(0);
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}
