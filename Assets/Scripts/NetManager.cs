using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;


public static class NetManager
{
    /// <summary>
    /// 定义套接字
    /// </summary>
    private static Socket socket;
    /// <summary>
    /// 接收缓冲区
    /// </summary>
    public static byte[] readBuffer = new byte[1024];
    /// <summary>
    /// 消息委托
    /// </summary>
    /// <param name="str"></param>
    public delegate void MsgListener(string str);
    /// <summary>
    /// 监听者字典
    /// </summary>
    public static Dictionary<string, MsgListener> listeners = new Dictionary<string, MsgListener>();
    /// <summary>
    /// 消息列表
    /// </summary>
    private static List<string> msgList = new List<string>();

    static int readIndex = 0;
    static int length = 0;

    /// <summary>
    /// 添加监听者
    /// </summary>
    /// <param name="msgName">消息名称</param>
    /// <param name="msgListener">消息函数（委托）</param>
    public static void AddListener(string msgName, MsgListener msgListener)
    {
        listeners[msgName] = msgListener;
    }

    /// <summary>
    /// 获取 IP 地址和自身端口号
    /// </summary>
    /// <returns></returns>
    public static string GetIPEndPoint()
    {
        if (socket == null)
            return null;
        if (socket.Connected == false)
            return null;
        return socket.LocalEndPoint.ToString();
    }

    /// <summary>
    /// 连接到服务器
    /// </summary>
    /// <param name="ip">IP 地址</param>
    /// <param name="port">端口号</param>
    public static void Connect(string ip, int port)
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // 异步连接
        // 连接到本机，BeginConnect() 为异步函数，调用后立刻返回，使程序可以执行其他事情
        // 当连接完成的时候，它会调用 ConnectCallBack() 这个函数
        //socket.BeginConnect(ip, port, ConnectCallBack, socket);

        // 同步连接
        socket.Connect(ip, port);
        socket.BeginReceive(readBuffer, 0, 1024, 0, ReceiveCallback, socket);
    }
    /// <summary>
    /// 连接到服务端回调函数。
    /// 检查连接是否成功，如果成功就开始等待接收信息；
    /// 如果失败，我们会在控制台显示错误信息。
    /// </summary>
    /// <param name="asyncResult">当异步操作完成时，会包含操作状态的 IAsyncResult 对象。这个对象包含了开始异步操作时传入的 state 对象（这里是 socket）</param>
    private static void ConnectCallBack(IAsyncResult asyncResult)
    {
        try
        {
            Socket socket = asyncResult.AsyncState as Socket;
            // 结束连接
            socket.EndConnect(asyncResult);
            Debug.Log("连接成功");
            // 异步接收
            socket.BeginReceive(readBuffer, 0, 1024, SocketFlags.None, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            // 发出错误提示
            Debug.LogError("连接失败，错误信息：" + ex);
        }
    }
    /// <summary>
    /// 接收数据的回调函数；
    /// 当接收到数据时，这个函数会被调用。
    /// 在这个函数中，首先调用 EndReceive() 结束接收操作，并获取接收到的数据大小，
    /// 如果数据大小为 0，说明客户端断开了连接；
    /// 否则，将接收到的数据转换为字符串并打印出来；
    /// 然后开始异步发送数据给客户端，并继续接收客户端发送的数据。
    /// </summary>
    /// <param name="asyncResult">当异步操作完成时，会包含操作状态的 IAsyncResult 对象。这个对象包含了开始异步操作时传入的 state 对象（这里是 socket）</param>
    private static void ReceiveCallback(IAsyncResult asyncResult)
    {
        try
        {
            Socket socket = asyncResult.AsyncState as Socket;
            // 结束接收
            int count = socket.EndReceive(asyncResult);
            string s = Encoding.UTF8.GetString(readBuffer, 0, count);

            msgList.Add(s);

            // 递归
            socket.BeginReceive(readBuffer, 0, readBuffer.Length, 0, ReceiveCallback, socket);   // 0 对应 SocketFlags.None
        }
        catch (SocketException ex)
        {
            Debug.LogError("客户端接收失败，错误信息：" + ex);
        }
    }

    /// <summary>
    /// 发送
    /// </summary>
    public static void Send(string sendStr)
    {
        if (socket == null)
            return;
        if (socket.Connected == false)
            return;

        readIndex = 0;

        // 发送
        byte[] sendBytes = Encoding.UTF8.GetBytes(sendStr);
        length = sendBytes.Length;

        // 异步发送数据
        socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
    }

    /// <summary>
    /// 发送数据的回调函数；
    /// 当数据发送完成时，这个函数会被调用。
    /// 在这个函数中，调用 EndSend() 结束发送操作。
    /// </summary>
    /// <param name="asyncResult">当异步操作完成时，会包含操作状态的 IAsyncResult 对象。这个对象包含了开始异步操作时传入的 state 对象（这里是 socket）</param>
    private static void SendCallback(IAsyncResult asyncResult)
    {
        try
        {
            Socket socket = asyncResult.AsyncState as Socket;
            int count = socket.EndSend(asyncResult);
            readIndex += count;
            length -= count;
            if (length > 0)
            {
                //socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
            }
            Debug.Log("发送成功，发送字符长度：" + count);
        }
        catch (SocketException ex)
        {
            Debug.LogError("发送失败，错误信息：" + ex);
        }
    }

    /// <summary>
    /// Update 方法，需要在外部调用
    /// </summary>
    public static void Update()
    {
        // ”Enter|0,0,0“
        // 检测列表是否为空
        if (msgList.Count <= 0)
            return;

        string msg = msgList[0];
        msgList.RemoveAt(0);
        string[] split = msg.Split('|');
        string msgName = split[0];
        string msgValue = split[1];

        if (listeners.ContainsKey(msgName))
        {
            listeners[msgName](msgValue);
        }
    }
}
