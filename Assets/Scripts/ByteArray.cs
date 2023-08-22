using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ByteArray : MonoBehaviour
{
    /// <summary>
    /// 默认大小
    /// </summary>
    const int DEFAULT_SIZE = 1024;
    /// <summary>
    /// 初始大小
    /// </summary>
    private int initSize = 0;
    /// <summary>
    /// 字节数组
    /// </summary>
    public byte[] bytes;
    /// <summary>
    /// Send() 会从这里读取
    /// </summary>
    public int readIndex = 0;
    /// <summary>
    /// Receive() 从这里写入
    /// </summary>
    public int writeIndex = 0;
    /// <summary>
    /// 容量
    /// </summary>
    private int capacity = 0;

    /// <summary>
    /// 剩余空间
    /// </summary>
    public int Remain { get { return capacity - writeIndex; } }
    /// <summary>
    /// 数据长度
    /// </summary>
    public int Length { get { return writeIndex - readIndex; } }

    /// <summary>
    /// 构造函数，用来创建字节数组
    /// </summary>
    /// <param name="size">字节数组默认大小</param>
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
