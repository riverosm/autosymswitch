using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;

namespace AutoSymSwitch
{
    public class SymetrixControl
    {
        private String SYMETRIX_IP = "169.254.164.244";
        private int SYMETRIX_PORT = 48631;
        private int SYMETRIX_READ_TIMEOUT = 5000;
        private int[] SYMETRIX_ALLOWED_PRESETS = { 1, 2 };
        private string[] SYMETRIX_PRESETS_NAMES = { "Preset #1", "Preset #2" };
        private String[] SYMETRIX_ALLOWED_IPS = { "192.168.0.92", "169.254.164.245" };
        private String SYMETRIX_VALID_TOKEN = "1212";
        private String SYMETRIX_EOF = "\r\n";
        private TcpClient symetrixConnection;

        public SymetrixControl(string ip, string token, string newPreset = "")
        {
            if (!isValidIP(ip))
                throw new System.Exception("IP not in white list");

            if (!isValidToken(token))
                throw new System.Exception("Invalid token");

            if (newPreset != "" && !isValidPreset(newPreset))
                throw new System.Exception("Invalid preset");
        }

        public ResponseStatusWithInfo getInfo()
        {
            List<Preset> presets = new List<Preset>();

            for (int i = 0; i < SYMETRIX_ALLOWED_PRESETS.Length; i++)
            {
                presets.Add(new Preset(SYMETRIX_ALLOWED_PRESETS[i], SYMETRIX_PRESETS_NAMES[i]));
            }

            return new ResponseStatusWithInfo(new ResponseStatus("200", "Symetrix info retreived"), new SymetrixInfo(SYMETRIX_IP, presets));
        }

        public ResponseStatusWithoutInfo controlSymetrix(string action, string newPreset = "")
        {
            if (action != "getPreset" && action != "setPreset")
                return new ResponseStatusWithoutInfo(new ResponseStatus("400", "Invalid request"));

            try
            {
                string informationText = "";

                createConnection();

                switch (action)
                {
                    case "getPreset":
                        informationText = getPreset();
                        break;
                    case "setPreset":
                        informationText = setPreset(newPreset);
                        break;
                }

                closeConnection();

                return new ResponseStatusWithoutInfo(new ResponseStatus("200", informationText));
            }
            catch (Exception ex)
            {
                return new ResponseStatusWithoutInfo(new ResponseStatus("500", ex.Message));
            }


        }

        private void createConnection()
        {
            // TODO - closeConnection with Q!
            new Logger().WriteToFile("createConnection  - Connecting to " + SYMETRIX_IP + ":" + SYMETRIX_PORT + "...");

            try
            {
                symetrixConnection = new TcpClient();
                symetrixConnection.Connect(new IPEndPoint(IPAddress.Parse(SYMETRIX_IP), SYMETRIX_PORT));
                new Logger().WriteToFile("createConnection  - Connected to " + SYMETRIX_IP + ":" + SYMETRIX_PORT);
            }
            catch (Exception ex)
            {
                throw new System.Exception("createConnection - " + ex.Message);
            }
        }

        private void closeConnection()
        {
            try
            {
                symetrixConnection.Close();
                new Logger().WriteToFile("closeConnection  - Disconnected from " + SYMETRIX_IP);
            }
            catch (Exception ex)
            {
                throw new System.Exception("closeConnection - " + ex.Message);
            }
        }

        private void sendData(string data)
        {
            new Logger().WriteToFile("sendData: " + data.Replace(SYMETRIX_EOF, ""));

            try
            {
                Byte[] sendData = System.Text.Encoding.ASCII.GetBytes(data);
                NetworkStream stream = symetrixConnection.GetStream();
                stream.Write(sendData, 0, sendData.Length);
            } catch (Exception ex)
            {
                throw new System.Exception(ex.Message);
            }
        }

        private String recvData()
        {
            try
            {
                NetworkStream stream = symetrixConnection.GetStream();
                stream.ReadTimeout = SYMETRIX_READ_TIMEOUT;
                // new Logger().WriteToFile("Stream read: " + stream.ReadTimeout.ToString());
                // new Logger().WriteToFile("Stream can: " + stream.CanTimeout.ToString());
                // new Logger().WriteToFile("Stream write: " + stream.WriteTimeout.ToString());
                Byte[] recvData = new Byte[256];
                String responseData = String.Empty;
                int bytes = stream.Read(recvData, 0, recvData.Length);
                responseData = System.Text.Encoding.ASCII.GetString(recvData, 0, bytes);
                new Logger().WriteToFile("recvData: " + responseData.Replace("\r", ""));
                return responseData.Replace("\r", "");
            }
            catch (Exception ex)
            {
                throw new System.Exception(ex.Message);
            }
        }

        private string getPreset()
        {
            try
            {
                this.sendData("GPR" + SYMETRIX_EOF);

                String recvDataReal = this.recvData();

                // TODO - getPresetNumber not ACK

                if (recvDataReal.Length != 4 || !isValidPreset(recvDataReal))
                {
                    new Logger().WriteToFile("getPreset - recvData expected: 'Valid preset' - received: '" + recvDataReal + "'", true);
                    throw new System.Exception("getPreset - Invalid preset received from Symetrix");
                }

                return string.Format(String.Format("{0,4:0000}", recvDataReal));
            } catch (Exception ex)
            {
                throw new System.Exception("getPreset - " + ex.Message);
            }
        }

        private string setPreset(string newPreset)
        {
            try
            {
                newPreset = string.Format(String.Format("{0,4:0000}", Int16.Parse(newPreset)));

                String[] sendData = { "LP " + newPreset, "GPR", "Q!" };
                String[] recvData = { "ACK", newPreset, "ACK" };

                String recvDataReal;

                for (int i = 0; i < sendData.Length; i++)
                {
                    this.sendData(sendData[i] + SYMETRIX_EOF);

                    recvDataReal = this.recvData();

                    if (recvDataReal != recvData[i])
                    {
                        new Logger().WriteToFile("setPreset " + newPreset + "- recvData expected: '" + recvData[i] + "' - received: '" + recvDataReal + "'", true);
                        throw new System.Exception("setPreset - Invalid response from Symetrix");
                    }
                }

                return "Aca mensaje de OK";
            }
            catch (Exception ex)
            {
                throw new System.Exception("setPreset - " + ex.Message);
            }
        }

        private bool isValidToken(string token)
        {
            return token == SYMETRIX_VALID_TOKEN;
        }

        private bool isValidIP(string ip)
        {
            return SYMETRIX_ALLOWED_IPS.Contains(ip);
        }
        private bool isValidPreset(string newPreset)
        {
            int result;

            try
            {
                result = Int32.Parse(newPreset);
            }
            catch (Exception ex)
            {
                new Logger().WriteToFile(ex.Message);
                return false;
            }

            return SYMETRIX_ALLOWED_PRESETS.Contains(result);
        }
    }

    [DataContract]
    public class Preset
    {
        public Preset (int id, string name)
        {
            Id = id;
            Name = name;
        }

        [DataMember]
        public int Id;
        [DataMember]
        public string Name;
    }

    [DataContract]
    public class SymetrixInfo
    {
        public SymetrixInfo (string ip, List<Preset> presets) {
            Ip = ip;
            Presets = presets;
        }
        [DataMember]
        public string Ip { get; set; }
        //public string Port { get; set; }
        [DataMember]
        public List<Preset> Presets { get; set; }
    }
}