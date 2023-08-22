using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ByteArray : MonoBehaviour
{
    /// <summary>
    /// Ĭ�ϴ�С
    /// </summary>
    const int DEFAULT_SIZE = 1024;
    /// <summary>
    /// ��ʼ��С
    /// </summary>
    private int initSize = 0;
    /// <summary>
    /// �ֽ�����
    /// </summary>
    public byte[] bytes;
    /// <summary>
    /// Send() ��������ȡ
    /// </summary>
    public int readIndex = 0;
    /// <summary>
    /// Receive() ������д��
    /// </summary>
    public int writeIndex = 0;
    /// <summary>
    /// ����
    /// </summary>
    private int capacity = 0;

    /// <summary>
    /// ʣ��ռ�
    /// </summary>
    public int Remain { get { return capacity - writeIndex; } }
    /// <summary>
    /// ���ݳ���
    /// </summary>
    public int Length { get { return writeIndex - readIndex; } }

    /// <summary>
    /// ���캯�������������ֽ�����
    /// </summary>
    /// <param name="size">�ֽ�����Ĭ�ϴ�С</param>
    public ByteArray(int size = DEFAULT_SIZE)
    {
        bytes = new byte[size];
        capacity = size;
        initSize = size;
        readIndex = 0;
        writeIndex = 0;
    }

    public ByteArray(byte[] defaultBytes)
    {
        bytes = defaultBytes;
        capacity = defaultBytes.Length;
        initSize = defaultBytes.Length;
        readIndex = 0;
        writeIndex = 0;
    }
}
