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
    // private string hostname = "k10c209.p.ssafy.io"; // 로컬
    // private int port = 1370;

    public byte[] Incoming()
    {
        // 데이터가 들어온 경우
        while (_networkStream != null && _networkStream.DataAvailable)
        {
            Debug.Log("Chatting Incoming");
            Receiver();
        }

        return null;
    }

    public void Connect(int port)
    {
        try
        {
            // TCP 서버에 연결
            _tcpClient = new TcpClient(hostname, port);
            _networkStream = _tcpClient.GetStream();

            Debug.Log("ChattingServer connect complete");
        }
        catch (Exception e)
        {
            // 연결 중 오류 발생 시
            Debug.Log($"Failed to connect to the ChattingServer: {e.Message}");
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
    private byte[] Receiver()
    {
        if (_tcpClient == null || !_tcpClient.Connected) return null;
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
                    // return MessagePackSerializer.Deserialize<ChattingMessage>(buffer.AsSpan().Slice(0, bytesRead).ToArray());
                    return buffer;
                }

                return null;
            }
        }
        catch(Exception e)
        {
            Debug.Log("응답 읽기 실패 : " + e.Message);
        }

        return null;
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