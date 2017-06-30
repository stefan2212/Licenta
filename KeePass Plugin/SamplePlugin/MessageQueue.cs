using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace SamplePlugin
{
    class MessageQueue
    {
        private static RSAParameters publicKey;
        private static RSAParameters privateKey;       
        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        private NetworkStream nwStream = null;
        public MessageQueue()
        {
            //InitializeComponent();
          //  InitializeComponent();
        }

        public void connectToSever()
        {
            msg("Client Started");
            generateKey();
            clientSocket.Connect("127.0.0.1", 8888);
            nwStream = clientSocket.GetStream();
        }

        public void SendMessage(string textTosend)
        {
            byte[] bytesToSend = Encoding.UTF8.GetBytes(textTosend);
            byte[] dataLenght = Encoding.UTF8.GetBytes(Convert.ToString(textTosend.Length));
            nwStream.Write(dataLenght, 0, dataLenght.Length);
            nwStream.Flush();
            nwStream.Write(bytesToSend, 0, bytesToSend.Length);
            nwStream.Flush();
        }

        private static void generateKey()
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                publicKey = rsa.ExportParameters(false);
                privateKey = rsa.ExportParameters(true);
            }
        }

        private static byte[] Encrypt (byte [] input)
        {
            byte[] encrypted;
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportParameters(publicKey);
                encrypted = rsa.Encrypt(input, false);
            }
                return encrypted;
        }

        public string ReciveMessage()
        {
            byte[] dataToRecive = new byte[1024];
            nwStream.Read(dataToRecive,0,1024);
            string textRecived = System.Text.Encoding.ASCII.GetString(dataToRecive);
            //textRecived = textRecived.Substring(0, textRecived.IndexOf("$"));
            return textRecived;
        }

        public byte [] Decrypt(byte [] input)
        {
            byte[] decrypted;
            using(var rsa= new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportParameters(privateKey);
                decrypted = rsa.Decrypt(input, true);
            }
            return decrypted;
        }

        private  byte[] preparePackege(byte [] data)
        {
            byte[] initialize = new byte[1];
            initialize[0] = 2;
            byte[] separator = new byte[1];
            separator[0] = 4;
            byte[] dataLenght = Encoding.UTF8.GetBytes(Convert.ToString(data.Length));
            MemoryStream ms = new MemoryStream();
            ms.Write(initialize, 0, initialize.Length);
            ms.Write(dataLenght, 0, dataLenght.Length);
            ms.Write(separator, 0, separator.Length);
            ms.Write(data, 0, data.Length);
            return ms.ToArray();
        }

        public void msg(string mesg)
        {
            Console.WriteLine("I got" + mesg);
        }

        public void closeClient()
        {
            nwStream.Close();
            clientSocket.Close();
        }

    }
}
