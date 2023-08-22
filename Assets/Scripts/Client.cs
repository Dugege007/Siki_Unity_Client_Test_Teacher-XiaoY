using UnityEngine;
using System.Net.Sockets;
using UnityEngine.UI;
using System.Text;
using System;

public class Client : MonoBehaviour
{
    Socket socket;
    public InputField inputField;
    public Text text;

    /// <summary>
    /// 接收缓冲区
    /// </summary>
    private byte[] readBuffer = new byte[1024];

    private string receiveStr = "";

    private void Update()
    {
        text.text = "服务端收到信息：" + receiveStr;
    }

    /// <summary>
    /// 连接
    /// </summary>
    public void Connect()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // 连接到本机，BeginConnect() 为异步函数，调用后立刻返回，使程序可以执行其他事情
        // 当连接完成的时候，它会调用 ConnectCallBack() 这个函数
        socket.BeginConnect("127.0.0.1", 8888, ConnectCallBack, socket);

        //// 连接到本机 同步连接
        //socket.Connect("127.0.0.1", 8888);
    }

    /// <summary>
    /// 连接到服务端回调函数。
    /// 检查连接是否成功，如果成功就开始等待接收信息；
    /// 如果失败，我们会在控制台显示错误信息。
    /// </summary>
    /// <param name="asyncResult">当异步操作完成时，会包含操作状态的 IAsyncResult 对象。这个对象包含了开始异步操作时传入的 state 对象（这里是 socket）</param>
    private void ConnectCallBack(IAsyncResult asyncResult)
    {
        try
        {
            //Debug.Log(ar.AsyncState);   // ar.AsyncState 为 socket.BeginConnect() 中 socket 的值

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
    private void ReceiveCallback(IAsyncResult asyncResult)
    {
        try
        {
            Socket socket = asyncResult.AsyncState as Socket;
            // 结束接收
            int count = socket.EndReceive(asyncResult);
            //receiveStr = Encoding.UTF8.GetString(readBuffer, 0, count);
            string s = Encoding.UTF8.GetString(readBuffer, 0, count);
            receiveStr = s + "\n" + receiveStr;

            // Unity 中的 UI 只能在主线程中访问，下面这句可写到 Update() 中
            //text.text = receiveStr;

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
    public void Send()
    {
        string sendStr = inputField.text;
        byte[] sendBytes = Encoding.UTF8.GetBytes(sendStr);

        // 异步发送数据
        socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);

        //// 将 inputField 中的文本发送到服务器
        //socket.Send(sendBytes);

        //// 接收数据 同步接收
        //byte[] readBuffer = new byte[1024];
        //int count = socket.Receive(readBuffer);
        //string receiveStr = Encoding.UTF8.GetString(readBuffer, 0, count);
        //text.text = receiveStr;
    }

    /// <summary>
    /// 发送数据的回调函数；
    /// 当数据发送完成时，这个函数会被调用。
    /// 在这个函数中，调用 EndSend() 结束发送操作。
    /// </summary>
    /// <param name="asyncResult">当异步操作完成时，会包含操作状态的 IAsyncResult 对象。这个对象包含了开始异步操作时传入的 state 对象（这里是 socket）</param>
    private void SendCallback(IAsyncResult asyncResult)
    {
        try
        {
            Socket socket = asyncResult.AsyncState as Socket;
            int count = socket.EndSend(asyncResult);
            Debug.Log("发送成功，发送字符长度：" + count);
        }
        catch (SocketException ex)
        {
            Debug.LogError("发送失败，错误信息：" + ex);
        }
    }
}
