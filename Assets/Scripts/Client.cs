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
    /// ���ջ�����
    /// </summary>
    private byte[] readBuffer = new byte[1024];

    private string receiveStr = "";

    private void Update()
    {
        text.text = "������յ���Ϣ��" + receiveStr;
    }

    /// <summary>
    /// ����
    /// </summary>
    public void Connect()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // ���ӵ�������BeginConnect() Ϊ�첽���������ú����̷��أ�ʹ�������ִ����������
        // ��������ɵ�ʱ��������� ConnectCallBack() �������
        socket.BeginConnect("127.0.0.1", 8888, ConnectCallBack, socket);

        //// ���ӵ����� ͬ������
        //socket.Connect("127.0.0.1", 8888);
    }

    /// <summary>
    /// ���ӵ�����˻ص�������
    /// ��������Ƿ�ɹ�������ɹ��Ϳ�ʼ�ȴ�������Ϣ��
    /// ���ʧ�ܣ����ǻ��ڿ���̨��ʾ������Ϣ��
    /// </summary>
    /// <param name="asyncResult">���첽�������ʱ�����������״̬�� IAsyncResult ���������������˿�ʼ�첽����ʱ����� state ���������� socket��</param>
    private void ConnectCallBack(IAsyncResult asyncResult)
    {
        try
        {
            //Debug.Log(ar.AsyncState);   // ar.AsyncState Ϊ socket.BeginConnect() �� socket ��ֵ

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
    private void ReceiveCallback(IAsyncResult asyncResult)
    {
        try
        {
            Socket socket = asyncResult.AsyncState as Socket;
            // ��������
            int count = socket.EndReceive(asyncResult);
            //receiveStr = Encoding.UTF8.GetString(readBuffer, 0, count);
            string s = Encoding.UTF8.GetString(readBuffer, 0, count);
            receiveStr = s + "\n" + receiveStr;

            // Unity �е� UI ֻ�������߳��з��ʣ���������д�� Update() ��
            //text.text = receiveStr;

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
    public void Send()
    {
        string sendStr = inputField.text;
        byte[] sendBytes = Encoding.UTF8.GetBytes(sendStr);

        // �첽��������
        socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);

        //// �� inputField �е��ı����͵�������
        //socket.Send(sendBytes);

        //// �������� ͬ������
        //byte[] readBuffer = new byte[1024];
        //int count = socket.Receive(readBuffer);
        //string receiveStr = Encoding.UTF8.GetString(readBuffer, 0, count);
        //text.text = receiveStr;
    }

    /// <summary>
    /// �������ݵĻص�������
    /// �����ݷ������ʱ����������ᱻ���á�
    /// ����������У����� EndSend() �������Ͳ�����
    /// </summary>
    /// <param name="asyncResult">���첽�������ʱ�����������״̬�� IAsyncResult ���������������˿�ʼ�첽����ʱ����� state ���������� socket��</param>
    private void SendCallback(IAsyncResult asyncResult)
    {
        try
        {
            Socket socket = asyncResult.AsyncState as Socket;
            int count = socket.EndSend(asyncResult);
            Debug.Log("���ͳɹ��������ַ����ȣ�" + count);
        }
        catch (SocketException ex)
        {
            Debug.LogError("����ʧ�ܣ�������Ϣ��" + ex);
        }
    }
}
