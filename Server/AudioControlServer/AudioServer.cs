using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace AudioControlServer
{
    
    class AudioServer
    {
        public enum MediaRequest
        {
            VOLUME_UP,
            VOLUME_DOWN,
            MUTE_TOGGLE,
            VOLUME_SET,
            MEDIA_NEXT , 
            MEDIA_PREVIOUS,
            MEDIA_STOP , 
            MEDIA_PLAY 
        }
        public delegate void MediaRequestHandler(MediaRequest request, int value = 0);
        public event MediaRequestHandler OnMedia;
        private int mListenPort = 0;
        private string mKey; 
        private Task mTask;
        CancellationTokenSource mCancellationSource = new CancellationTokenSource();
        CancellationToken mCancellationToken;
        private UdpClient mUdpClient;
        private MqttClient mMqttClient;
        private string MQTT_BROKER_ADDRESS = "cig.nengxin.com.cn";
        private string MQTT_BROKER_USER = "nengxin";
        private string MQTT_BROKER_PASSWD = "NX@)!*";

        public AudioServer(int port,string key)
        {
            this.mListenPort = port;
            mUdpClient = new UdpClient(new IPEndPoint(IPAddress.Any, port));
            mCancellationToken = mCancellationSource.Token;
            mKey = key;
        }
        private void sendEvent(MediaRequest request, int value = 0)
        {
            OnMedia?.Invoke(request, value);
        }

        private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            // handle message received 
            try
            {
                byte[] buffer = e.Message;
                string data = System.Text.Encoding.UTF8.GetString(buffer);
                dynamic mData = JsonConvert.DeserializeObject(data);
                string key = mData.Key;
                string action = mData.Action;
                int Volume = mData.Volume;
                if (mKey != key)
                {

                    byte[] err_msg = Encoding.UTF8.GetBytes("INVALID_KEY");
                    return;
                }
                if (action == "GET_VOLUME") {
                    float volume = AudioManager.GetMasterVolume();
                    string topic = "/audiostate/" + mKey;
                    mMqttClient.Publish(topic, Encoding.ASCII.GetBytes(volume.ToString()));
                }

                if (action == "VOLUME_UP")
                {
                    sendEvent(MediaRequest.VOLUME_UP);
                }
                else if (action == "VOLUME_DOWN")
                {
                    sendEvent(MediaRequest.VOLUME_DOWN);
                }
                else if (action == "SET_VOLUME")
                {
                    sendEvent(MediaRequest.VOLUME_SET, Volume);
                }
                else if (action == "MUTE_TOGGLE")
                {
                    sendEvent(MediaRequest.MUTE_TOGGLE);
                }

                else if (action == "MEDIA_NEXT")
                {
                    sendEvent(MediaRequest.MEDIA_NEXT);
                }
                else if (action == "MEDIA_PREVIUS")
                {
                    sendEvent(MediaRequest.MEDIA_PREVIOUS);
                }
                else if (action == "MEDIA_STOP")
                {
                    sendEvent(MediaRequest.MEDIA_STOP);
                }
                else if (action == "MEDIA_PLAY")
                {
                    sendEvent(MediaRequest.MEDIA_PLAY);
                }
                byte[] msg = Encoding.UTF8.GetBytes("OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static uint PJWHash(string str)
        {
            const uint BitsInUnsignedInt = (uint)(sizeof(uint) * 8);
            const uint ThreeQuarters = (uint)((BitsInUnsignedInt * 3) / 4);
            const uint OneEighth = (uint)(BitsInUnsignedInt / 8);
            const uint HighBits = (uint)(0xFFFFFFFF) << (int)(BitsInUnsignedInt - OneEighth);
            uint hash = 0;
            uint test = 0;
            uint i = 0;

            for (i = 0; i < str.Length; i++)
            {
                hash = (hash << (int)OneEighth) + ((byte)str[(int)i]);

                if ((test = hash & HighBits) != 0)
                {
                    hash = ((hash ^ (test >> (int)ThreeQuarters)) & (~HighBits));
                }
            }

            return hash;
        }


        public void StartServer()
        {
            // create client instance
            // create client instance 
#pragma warning disable CS0618 // 类型或成员已过时
            mMqttClient = new MqttClient(MQTT_BROKER_ADDRESS);
#pragma warning restore CS0618 // 类型或成员已过时

            // register to message received 
            mMqttClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            //string clientId = Guid.NewGuid().ToString();
            //mMqttClient.Connect(clientId);

            string clientId = PJWHash(Guid.NewGuid().ToString()).ToString();
            Properties.Settings.Default.Key = clientId;

            mMqttClient.Connect(clientId, MQTT_BROKER_USER, MQTT_BROKER_PASSWD);

            // subscribe to the topic "/home/temperature" with QoS 2 
            mKey = clientId;
            mMqttClient.Subscribe(new string[] { "/audioctrl/" + mKey }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            //mUdpClient.AllowNatTraversal(true);
            //mUdpClient.EnableBroadcast = true;
            //mTask = new Task(async () => {
            //    while (!mCancellationToken.IsCancellationRequested)
            //    {
            //        try
            //        {
            //            var UdpResult = await mUdpClient.ReceiveAsync();
            //            byte[] buffer = UdpResult.Buffer;
            //            string data = System.Text.Encoding.UTF8.GetString(buffer);
            //            dynamic mData = JsonConvert.DeserializeObject(data);
            //            string key = mData.Key;
            //            string action = mData.Action;
            //            int Volume = mData.Volume;
            //            if (mKey != key)
            //            {

            //                byte[] err_msg = Encoding.UTF8.GetBytes("INVALID_KEY");
            //                await mUdpClient.SendAsync(err_msg, err_msg.Length, UdpResult.RemoteEndPoint);
            //                Console.WriteLine($"INVALID KEY FROM : {UdpResult.RemoteEndPoint.ToString()}");

            //                continue;
            //            }
            //            if (action == "VOLUME_UP")
            //            {
            //                sendEvent(MediaRequest.VOLUME_UP);
            //            }
            //            else if (action == "VOLUME_DOWN")
            //            {
            //                sendEvent(MediaRequest.VOLUME_DOWN);
            //            }
            //            else if (action == "SET_VOLUME")
            //            {
            //                sendEvent(MediaRequest.VOLUME_SET, Volume);
            //            }
            //            else if (action == "MUTE_TOGGLE")
            //            {
            //                sendEvent(MediaRequest.MUTE_TOGGLE);
            //            }


            //            else if (action == "MEDIA_NEXT")
            //            {
            //                sendEvent(MediaRequest.MEDIA_NEXT);
            //            }
            //            else if (action == "MEDIA_PREVIUS")
            //            {
            //                sendEvent(MediaRequest.MEDIA_PREVIOUS);
            //            }
            //            else if (action == "MEDIA_STOP")
            //            {
            //                sendEvent(MediaRequest.MEDIA_STOP);
            //            }
            //            else if (action == "MEDIA_PLAY")
            //            {
            //                sendEvent(MediaRequest.MEDIA_PLAY);
            //            }
            //            byte[] msg = Encoding.UTF8.GetBytes("OK");
            //            await mUdpClient.SendAsync(msg, msg.Length, UdpResult.RemoteEndPoint);
            //        }catch(Exception e)
            //        {
            //            Console.WriteLine(e.Message);
            //        }
            //    }
            //    mUdpClient.Close();
            //    Console.WriteLine("Thread Exited");
            //},mCancellationToken);
            //mTask.Start();
        }
        public void StopServer()
        {
            mCancellationSource.Cancel(); 
        }
    }
}
