using System.Net.Sockets;
using MelonLoader;

/*
 * LiveSplitClient - This class is a MelonLoader Mod class used to connect and run to a LiveSplit Server 
 *      LiveSplit Server - https://github.com/LiveSplit/LiveSplit.Server
 */
public class LiveSplitClient
{
    //TCP Client used to connect to LiveSplit Server
    TcpClient client = new TcpClient();

    //NetworkStream used to send messages to Server
    NetworkStream? stream;

    //Bool used if the log should be printed or not
    bool log = true;

    /* 
     * Basic Constructor
     */
    public LiveSplitClient()
    {
        MelonLogger.Msg(System.ConsoleColor.Green, "Attempting to connect to LiveSplit Server");
        client.Connect("localhost", 16834);

        MelonLogger.Msg(System.ConsoleColor.Green, "Connection succesful to LiveSplit Server");
        stream = client.GetStream();
    }

    /* 
     * Constructor with ip and port if a specific ip or port is needed
     */
    public LiveSplitClient(string ip, int port)
    {
        MelonLogger.Msg(System.ConsoleColor.Green, "Attempting to connect to LiveSplit Server");
        client.Connect(ip, port);

        MelonLogger.Msg(System.ConsoleColor.Green, "Connection succesful to LiveSplit Server");
        stream = client.GetStream();
    }

    /* 
     * Constructor with ip, port and log if you want to log commands or not
     */
    public LiveSplitClient(string ip, int port, bool log)
    {
        MelonLogger.Msg(System.ConsoleColor.Green, "Attempting to connect to LiveSplit Server");
        client.Connect(ip, port);

        MelonLogger.Msg(System.ConsoleColor.Green, "Connection succesful to LiveSplit Server");
        stream = client.GetStream();

        this.log = log;
    }

    /* 
     * CloseClient - Used to close the client after use
     */
    public void CloseClient()
    {
        client.Close();
    }

    /* 
     * PrintLog - Used to print message attempts and if it was successful or failed
     */
    public void PrintLog(bool attempt, bool fail, string cmd)
    {
        if (log)
        {
            if (attempt)
            {
                MelonLogger.Msg(System.ConsoleColor.Green, "Attempting to send '" + cmd + "' command to LiveSplit Server");
                return;
            }

            if (!fail)
                MelonLogger.Msg(System.ConsoleColor.Green, "Succesfully sent '" + cmd + "' command to LiveSplit Server");
            else
                MelonLogger.Msg(System.ConsoleColor.Red, "Failed to send '" + cmd + "' command to LiveSplit Server");
        }
    }

    /* 
     * SendCmd - Attempts to send a command using the argument cmd
     * Possible LiveSplit Server commands here -> https://github.com/LiveSplit/LiveSplit.Server#commands
     */
    public void SendCmd(string cmd)
    {
        PrintLog(true, false, cmd);
        byte[] msg = System.Text.Encoding.ASCII.GetBytes(cmd + "\r\n");

        if (stream == null)
        {
            PrintLog(false, true, cmd);
            return;
        }

        stream.Write(msg, 0, msg.Length);
        PrintLog(false, false, cmd);
    }

    //Below are the most used commands youll send, however you can always use the SendCmd method to send any of these

    /* 
     * StartTimer - Attempts to send the 'starttimer' cmd to the LiveSplit Server
     */
    public void StartTimer()
    {
        PrintLog(true, false, "starttimer");
        byte[] msg = System.Text.Encoding.ASCII.GetBytes("starttimer\r\n");

        if (stream == null)
        {
            PrintLog(false, true, "starttimer");
            return;
        }

        stream.Write(msg, 0, msg.Length);
        PrintLog(false, false, "starttimer");
    }

    /* 
     * StartOrSplitTimer - Attempts to send the 'startorsplit' cmd to the LiveSplit Server
     */
    public void StartOrSplitTimer()
    {
        PrintLog(true, false, "startorsplit");
        byte[] msg = System.Text.Encoding.ASCII.GetBytes("startorsplit\r\n");

        if (stream == null)
        {
            PrintLog(false, true, "startorsplit");
            return;
        }

        stream.Write(msg, 0, msg.Length);
        PrintLog(false, false, "startorsplit");
    }

    /* 
     * SplitTimer - Attempts to send the 'split' cmd to the LiveSplit Server
     */
    public void SplitTimer()
    {
        PrintLog(true, false, "split");
        byte[] msg = System.Text.Encoding.ASCII.GetBytes("split\r\n");

        if (stream == null)
        {
            PrintLog(false, true, "split");
            return;
        }

        stream.Write(msg, 0, msg.Length);
        PrintLog(false, false, "split");
    }

    /* 
     * UnSplitTimer - Attempts to send the 'unsplit' cmd to the LiveSplit Server
     */
    public void UnSplitTimer()
    {
        PrintLog(true, false, "unsplit");
        byte[] msg = System.Text.Encoding.ASCII.GetBytes("unsplit\r\n");

        if (stream == null)
        {
            PrintLog(false, true, "unsplit");
            return;
        }

        stream.Write(msg, 0, msg.Length);
        PrintLog(false, false, "unsplit");
    }

    /* 
     * SkipSplitTimer - Attempts to send the 'skipsplit' cmd to the LiveSplit Server
     */
    public void SkipSplitTimer()
    {
        PrintLog(true, false, "skipsplit");
        byte[] msg = System.Text.Encoding.ASCII.GetBytes("skipsplit\r\n");

        if (stream == null)
        {
            PrintLog(false, true, "skipsplit");
            return;
        }

        stream.Write(msg, 0, msg.Length);
        PrintLog(false, false, "skipsplit");
    }

    /* 
     * PauseTimer - Attempts to send the 'pause' cmd to the LiveSplit Server
     */
    public void PauseTimer()
    {
        PrintLog(true, false, "pause");
        byte[] msg = System.Text.Encoding.ASCII.GetBytes("pause\r\n");

        if (stream == null)
        {
            PrintLog(false, true, "pause");
            return;
        }

        stream.Write(msg, 0, msg.Length);
        PrintLog(false, false, "pause");
    }

    /* 
     * ResumeTimer - Attempts to send the 'resume' cmd to the LiveSplit Server
     */
    public void ResumeTimer()
    {
        PrintLog(true, false, "resume");
        byte[] msg = System.Text.Encoding.ASCII.GetBytes("resume\r\n");

        if (stream == null)
        {
            PrintLog(false, true, "resume");
            return;
        }

        stream.Write(msg, 0, msg.Length);
        PrintLog(false, false, "resume");
    }

    /* 
     * ResetTimer - Attempts to send the 'reset' cmd to the LiveSplit Server
     */
    public void ResetTimer()
    {
        PrintLog(true, false, "reset");
        byte[] msg = System.Text.Encoding.ASCII.GetBytes("reset\r\n");

        if (stream == null)
        {
            PrintLog(false, true, "reset");
        }

        stream.Write(msg, 0, msg.Length);
        PrintLog(false, false, "reset");
    }
}