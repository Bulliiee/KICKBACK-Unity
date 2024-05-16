using System;
using System.Text;
using System.Net.Sockets;
using UnityEngine;

using MessagePack;
using Highlands.Server;

public class TCPConnectionController
{
    private TcpClient _tcpClient;
    private NetworkStream _networkStream;
    
    // 호스트
    private string hostname = "k10c209.p.ssafy.io"; // ec2
    // private string hostname = "localhost"; // 로컬

    public (byte[], int) ChatIncoming()
    {
        // 데이터가 들어온 경우
        while (_networkStream != null && _networkStream.DataAvailable)
        {
            Debug.Log("Incoming from ChattingServer");
            ChatReceiver();
        }

        return (null, 0);
    }

    public (byte[], int) BusinessIncoming()
    {
        // 데이터가 들어온 경우
        while (_networkStream != null && _networkStream.DataAvailable)
        {
            Debug.Log("Incoming from BusinessServer");
            BusinessReceiver();
        }

        return (null, 0);
    }

    public void Connect(string server, int port)
    {
        try
        {
            // TCP 서버에 연결
            _tcpClient = new TcpClient(hostname, port);
            _networkStream = _tcpClient.GetStream();

            Debug.Log($"{server} connect success");
        }
        catch (Exception e)
        {
            // 연결 중 오류 발생 시
            Debug.Log($"Failed to connect server: {e.Message}");
        }
    }

    // 서버로 메세지 보내기
    public void Deliver(byte[] message)
    {
        if (_tcpClient == null) return;

        _networkStream.Write(message, 0, message.Length);
        _networkStream.Flush();
    }

    // 서버로부터 수신한 메세지 읽기
    private (byte[], int) ChatReceiver()
    {
        if (_tcpClient == null || !_tcpClient.Connected) return (null, 0);
        try
        {
            if (_networkStream == null)
            {
                _networkStream = _tcpClient.GetStream();
            }

            // 네트워크 스트림에 데이터가 있을 때까지 반복
            while (_networkStream.DataAvailable)
            {
                byte[] buffer = new byte[_tcpClient.ReceiveBufferSize];
                int bytesRead = _networkStream.Read(buffer, 0, buffer.Length); // 실제 데이터를 읽음

                if (bytesRead > 0)
                {
                    // MessagePackSerializer를 사용하여 메시지 역직렬화
                    // return MessagePackSerializer.Deserialize<Message>(buffer.AsSpan().Slice(0, bytesRead).ToArray());
                    return (buffer, bytesRead);
                }

                return (null, 0);
            }
        }
        catch(Exception e)
        {
            Debug.Log("채팅서버 응답 읽기 실패 : " + e.Message);
        }

        return (null, 0);
    }

    private (byte[], int) BusinessReceiver()
    {
        if (_tcpClient == null || !_tcpClient.Connected) return (null, 0);
        try
        {
            if (_networkStream == null)
            {
                _networkStream = _tcpClient.GetStream();
            }

            // 네트워크 스트림에 데이터가 있을 때까지 반복
            while (_networkStream.DataAvailable)
            {
                byte[] messageBuffer = new byte[4];
                int bytesRead = _networkStream.Read(messageBuffer, 0, 4); // 실제 데이터를 읽음
                Array.Reverse(messageBuffer);
                
                if (bytesRead == 4)
                {
                    int length = BitConverter.ToInt32(messageBuffer, 0);

                    byte[] buffer = new byte[length];
                    bytesRead = _networkStream.Read(buffer, 0, length);

                    if (bytesRead > 0)
                    {
                        return (buffer, length);
                    }
                }

                return (null, 0);
            }
        }
        catch(Exception e)
        {
            Debug.Log("비즈니스서버 응답 읽기 실패 : " + e.Message);
        }

        return (null, 0);
    }
    
    public void DisconnectFromServer()
    {
        // 연결 종료
        _networkStream.Close();
        _tcpClient.Close();
        _networkStream = null;
        _tcpClient = null;
    }
}