using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AudioControlServer
{
    class MediaControl
    {
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte virtualKey, byte scanCode, uint flags, IntPtr extraInfo);
        private const int KEYEVENTF_EXTENTEDKEY = 1;
        private const int KEYEVENTF_KEYUP = 0;
        private const int VK_MEDIA_NEXT_TRACK = 0xB0;// code to jump to next track
        private const int VK_MEDIA_PLAY_PAUSE = 0xB3;// code to play or pause a song
        private const int VK_MEDIA_PREV_TRACK = 0xB1;// code to jump to prev track
        private const int VK_MEDIA_STOP = 0xB2;

        public static void NextTrack()
        {
            keybd_event(VK_MEDIA_NEXT_TRACK, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero);
        }
        public static void PreviousTrack()
        {
            keybd_event(VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero);
        }
        public static void PlayPause()
        {
            keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero);
        }
        public static void Stop()
        {
            keybd_event(VK_MEDIA_STOP, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero);
        }
    }
}
