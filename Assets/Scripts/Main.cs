using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    /// <summary>
    /// Player Ԥ����
    /// </summary>
    public GameObject playerPrefab;
    /// <summary>
    /// �Լ��� Player
    /// </summary>
    public BasePlayer myPlayer;

    public Dictionary<string, BasePlayer> otherPlayers = new Dictionary<string, BasePlayer>();

    private void Start()
    {
        // ���� Json
        MsgBase msg = new MsgBase();
        msg.x = 1;
        msg.y = 2;
        msg.z = 3;
        string s = JsonUtility.ToJson(msg);

        NetManager.AddListener("Move", OnMove);
        NetManager.AddListener("List", OnList);
        NetManager.AddListener("Enter", OnEnter);
        NetManager.Connect("127.0.0.1", 8888);
        // ��ӽ�ɫ
        GameObject playerObj = Instantiate(playerPrefab);
        float x = Random.Range(-3, 3);
        float z = Random.Range(-3, 3);
        playerObj.transform.position = new Vector3(x, 0, z);
        // ��ӽű�
        myPlayer = playerObj.AddComponent<CtrlPlayer>();
        myPlayer.ipEndPoint = NetManager.GetIPEndPoint();
        // ����Э��
        Vector3 pos = playerObj.transform.position;
        string sendStr = "Enter|";
        sendStr += NetManager.GetIPEndPoint() + ",";
        sendStr += pos.x + ",";
        sendStr += pos.y + ",";
        sendStr += pos.z + ",";
        sendStr += transform.eulerAngles.y + ",";
        NetManager.Send(sendStr);
        NetManager.Send("List|");
    }

    private void Update()
    {
        NetManager.Update();
    }

    /// <summary>
    /// ����Э��
    /// </summary>
    /// <param name="msgValue">��Ϣ</param>
    private void OnEnter(string msgValue)
    {
        string[] split = msgValue.Split(',');
        string ipEndPoint = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        float eulerY = float.Parse(split[4]);

        // ���ɺ����� Player
        if (ipEndPoint == NetManager.GetIPEndPoint())
            return;
        GameObject player = Instantiate(playerPrefab);
        player.transform.position = new Vector3(x, y, z);
        player.transform.eulerAngles = new Vector3(0, eulerY, 0);
        BasePlayer syncPlayer = player.AddComponent<SyncPlsyer>();
        otherPlayers.Add(ipEndPoint, syncPlayer);
    }

    private void OnList(string msgValue)
    {
        string[] split = msgValue.Split(",");
        // ����֮ǰ�Ŀͻ�������
        int count = split.Length / 5;
        for (int i = 0; i < count; i++)
        {
            string ipEndPoint = split[i * 5];
            float x = float.Parse(split[i * 5 + 1]);
            float y = float.Parse(split[i * 5 + 2]);
            float z = float.Parse(split[i * 5 + 3]);
            float eulerY = float.Parse(split[i * 5 + 4]);

            if (ipEndPoint == NetManager.GetIPEndPoint())
                return;

            GameObject player = Instantiate(playerPrefab);
            player.transform.position = new Vector3(x, y, z);
            player.transform.eulerAngles = new Vector3(0, eulerY, 0);
            BasePlayer syncPlayer = player.AddComponent<SyncPlsyer>();
            syncPlayer.ipEndPoint = ipEndPoint;
            otherPlayers.Add(ipEndPoint, syncPlayer);
        }
    }

    private void OnMove(string msgValue)
    {
        // ����Э��
        string[] split = msgValue.Split(',');
        string ipEndPoint = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);

        // �ƶ�
        if (otherPlayers.ContainsKey(ipEndPoint) == false)
            return;

        BasePlayer basePlayer = otherPlayers[ipEndPoint];
        basePlayer.MoveTo(new Vector3(x, y, z));
    }
}
