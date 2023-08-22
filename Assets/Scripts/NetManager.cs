using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;


public static class NetManager
{
    /// <summary>
    /// �����׽���
    /// </summary>
    private static Socket socket;
    /// <summary>
    /// ���ջ�����
    /// </summary>
    public static byte[] readBuffer = new byte[1024];
    /// <summary>
    /// ��Ϣί��
    /// </summary>
    /// <param name="str"></param>
    public delegate void MsgListener(string str);
    /// <summary>
    /// �������ֵ�
    /// </summary>
    public static Dictionary<string, MsgListener> listeners = new Dictionary<string, MsgListener>();
    /// <summary>
    /// ��Ϣ�б�
    /// </summary>
    private static List<string> msgList = new List<string>();

    static int readIndex = 0;
    static int length = 0;

    /// <summary>
    /// ��Ӽ�����
    /// </summary>
    /// <param name="msgName">��Ϣ����</param>
    /// <param name="msgListener">��Ϣ������ί�У�</param>
    public static void AddListener(string msgName, MsgListener msgListener)
    {
        listeners[msgName] = msgListener;
    }

    /// <summary>
    /// ��ȡ IP ��ַ������˿ں�
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
    /// ���ӵ�������
    /// </summary>
    /// <param name="ip">IP ��ַ</param>
    /// <param name="port">�˿ں�</param>
    public static void Connect(string ip, int port)
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // �첽����
        // ���ӵ�������BeginConnect() Ϊ�첽���������ú����̷��أ�ʹ�������ִ����������
        // ��������ɵ�ʱ��������� ConnectCallBack() �������
        //socket.BeginConnect(ip, port, ConnectCallBack, socket);

        // ͬ������
        socket.Connect(ip, port);
        socket.BeginReceive(readBuffer, 0, 1024, 0, ReceiveCallback, socket);
    }
    /// <summary>
    /// ���ӵ�����˻ص�������
    /// ��������Ƿ�ɹ�������ɹ��Ϳ�ʼ�ȴ�������Ϣ��
    /// ���ʧ�ܣ����ǻ��ڿ���̨��ʾ������Ϣ��
    /// </summary>
    /// <param name="asyncResult">���첽�������ʱ�����������״̬�� IAsyncResult ���������������˿�ʼ�첽����ʱ����� state ���������� socket��</param>
    private static void ConnectCallBack(IAsyncResult asyncResult)
    {
        try
        {
            Socket socket = asyncResult.AsyncState as Socket;
            // ��������
            socket.EndConnect(asyncResult);
            Debug.Log("���ӳɹ�");
            // �첽����
            socket.BeginReceive(readBuffer, 0, 1024, SocketFlags.None, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            // ����������ʾ
            Debug.LogError("����ʧ�ܣ�������Ϣ��" + ex);
        }
    }
    /// <summary>
    /// �������ݵĻص�������
    /// �����յ�����ʱ����������ᱻ���á�
    /// ����������У����ȵ��� EndReceive() �������ղ���������ȡ���յ������ݴ�С��
    /// ������ݴ�СΪ 0��˵���ͻ��˶Ͽ������ӣ�
    /// ���򣬽����յ�������ת��Ϊ�ַ�������ӡ������
    /// Ȼ��ʼ�첽�������ݸ��ͻ��ˣ����������տͻ��˷��͵����ݡ�
    /// </summary>
    /// <param name="asyncResult">���첽�������ʱ�����������״̬�� IAsyncResult ���������������˿�ʼ�첽����ʱ����� state ���������� socket��</param>
    private static void ReceiveCallback(IAsyncResult asyncResult)
    {
        try
        {
            Socket socket = asyncResult.AsyncState as Socket;
            // ��������
            int count = socket.EndReceive(asyncResult);
            string s = Encoding.UTF8.GetString(readBuffer, 0, count);

            msgList.Add(s);

            // �ݹ�
            socket.BeginReceive(readBuffer, 0, readBuffer.Length, 0, ReceiveCallback, socket);   // 0 ��Ӧ SocketFlags.None
        }
        catch (SocketException ex)
        {
            Debug.LogError("�ͻ��˽���ʧ�ܣ�������Ϣ��" + ex);
        }
    }

    /// <summary>
    /// ����
    /// </summary>
    public static void Send(string sendStr)
    {
        if (socket == null)
            return;
        if (socket.Connected == false)
            return;

        readIndex = 0;

        // ����
        byte[] sendBytes = Encoding.UTF8.GetBytes(sendStr);
        length = sendBytes.Length;

        // �첽��������
        socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
    }

    /// <summary>
    /// �������ݵĻص�������
    /// �����ݷ������ʱ����������ᱻ���á�
    /// ����������У����� EndSend() �������Ͳ�����
    /// </summary>
    /// <param name="asyncResult">���첽�������ʱ�����������״̬�� IAsyncResult ���������������˿�ʼ�첽����ʱ����� state ���������� socket��</param>
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
            Debug.Log("���ͳɹ��������ַ����ȣ�" + count);
        }
        catch (SocketException ex)
        {
            Debug.LogError("����ʧ�ܣ�������Ϣ��" + ex);
        }
    }

    /// <summary>
    /// Update ��������Ҫ���ⲿ����
    /// </summary>
    public static void Update()
    {
        // ��Enter|0,0,0��
        // ����б��Ƿ�Ϊ��
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
