using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using Google.Protobuf;
using System.Net;
using System.Threading;
using Unity.Threading;

public static class NetWork
{
    public static string EVENT_RECEIVE = "ReceiveMsg";
    static Socket socket;
    static Thread thread;
    //static List<ArraySegment<byte>> buffers = new List<ArraySegment<byte>>();
    static byte[] bufferEx = new byte[9999];
    static EasyThread easyThread = Unity.Threading.EasyThread.GetInstance();

    public static void Init(string ipStr = "")
    {
#if UNITY_EDITOR
        //ipStr = "192.168.124.3";
#endif
        IPAddress ip = IPAddress.Parse(ipStr);
        int port = 9091;

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //for (int i = 0; i < 10; i++)
        {
            try
            {
                socket.Connect(ip, port); //配置服务器IP与端口  
                Debug.Log("连接服务器成功" + port);
                thread = new Thread(StartReceive);
                thread.IsBackground = true;

                //thread.Start(socket);

                easyThread.StartNewThread(thread, socket);
                easyThread.mainRemote.On<string, byte[]>(EVENT_RECEIVE, (name, bodyBuffer) =>
                {
                    ProtoHelper.OnReceiveMsg(name, bodyBuffer);
                });
                easyThread.childRemote.On<string, byte[]>(EVENT_RECEIVE, (name, bodyBuffer) =>
                {
                    ProtoHelper.OnReceiveMsg(name, bodyBuffer);
                });
                GameManager.Singleton.gameWindow.SetEnterUI(false);
            }
            catch (Exception e)
            {
                GameManager.Singleton.gameWindow.SetEnterUI(true);
                GameManager.Singleton.gameWindow.ShowTip("连接服务器失败！");
                Debug.LogError("连接服务器失败！" + e);
            }
        }
    }

    /// <summary>
    /// 开启接收
    /// </summary>
    /// <param name="obj"></param>
    private static void StartReceive(object obj)
    {
        Socket receiveSocket = obj as Socket;
        byte[] bufferRead = new byte[9999];
        int bufferExLen = 0;
        while (true)
        {
            try
            {
                byte[] buffer = new byte[1024];
                SocketReadWithLength(receiveSocket, buffer, 2);
                int len1 = (buffer[0] << 8) | buffer[1];
                SocketReadWithLength(receiveSocket, buffer, len1);
                int len2 = (buffer[0] << 8) | buffer[1];
                string name = Encoding.UTF8.GetString(buffer, 2, len2);
                byte[] body = buffer.Skip(2 + len2).Take(len1 - len2 - 2).ToArray();
                easyThread.mainRemote.Send(EVENT_RECEIVE, name, body);

                //int result = receiveSocket.Receive(bufferRead);
                //Debug.Log(result);
                //if (result == 0)
                //{
                //    continue;
                //}
                //else
                //{
                //    byte[] buffer = new byte[20000];
                //    if(bufferExLen > 0)
                //    {
                //        Array.Copy(bufferEx, 0, buffer, 0, bufferExLen);
                //    }
                //    Array.Copy(bufferRead, 0, buffer, bufferExLen, result);
                //    int totalLen = bufferExLen + result;

                //    Debug.Log("" + buffer[0] + "," + buffer[1] + "," + buffer[2] + "," + buffer[3] + "," + buffer[4] + "," + buffer[5] +"," + buffer[6] + "," + buffer[7] + "," + buffer[8] + "," + buffer[9] + "," + buffer[10] + "," + buffer[11] + "," + buffer[12]);
                //    int len1 = (buffer[0] << 8) | buffer[1];
                //    int len2 = (buffer[2] << 8) | buffer[3];
                //    if (totalLen < len1 + 2)
                //    {
                //        Array.Copy(buffer, 0, bufferEx, 0, totalLen);
                //        bufferExLen = totalLen;
                //        continue;
                //    }
                //    //Debug.Log("" + len1 + "," + len2);
                //    int nameLen = len2;
                //    int bodyLen = len1 - len2 - 2;
                //    //Debug.Log("nameLen, bodyLen:" + nameLen + "," + bodyLen);
                //    byte[] nameBuffer = new byte[nameLen];
                //    byte[] bodyBuffer = new byte[bodyLen];
                //    Array.Copy(buffer, 4, nameBuffer, 0, nameLen);
                //    Array.Copy(buffer, 4 + nameLen, bodyBuffer, 0, bodyLen);
                //    if(totalLen > len1 + 2)
                //    {
                //        Array.Copy(buffer, 2 + len1, bufferEx, 0, totalLen - len1 - 2);
                //        bufferExLen = totalLen - len1 - 2;
                //    }
                //    else if(totalLen == len1 + 2)
                //    {
                //        bufferExLen = 0;
                //    }
                //    string name = Encoding.ASCII.GetString(nameBuffer);
                //    Debug.Log(name);


                //    easyThread.mainRemote.Send(EVENT_RECEIVE, name, bodyBuffer);
                //    //ProtoHelper.OnReceiveMsg(name, bodyBuffer);
                //}

            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                break;
            }
        }
    }

    public static int SocketReadWithLength(Socket socket, byte[] buffer)
    {
        return SocketReadWithLength(socket, buffer, buffer.Length);
    }

    private static int SocketReadWithLength(Socket socket, byte[] buffer, int length)
    {
        int n = 0;
        while(true)
        {
            int num = socket.Receive(buffer, n, length - n, SocketFlags.None);
            if(num == -1)
            {
                break;
            }
            n += num;
            if(n == buffer.Length)
            {
                break;
            }
            if (n == length)
            {
                break;
            }
        }
        return n;
    }

    public static void Dispose()
    {
        try
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            thread.Abort();
            easyThread.StopThread();
            thread = null;
            Debug.Log("关闭与远程服务器的连接!");
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public static void Send(byte[] buffer)
    {
        socket.Send(buffer);
    }

}
