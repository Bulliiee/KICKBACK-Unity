using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Highlands.Server
{
    public class UDPConnectionController
    {
        private UdpClient _udpClient;
        private IPEndPoint _ipEndPoint;

        private string hostname = "k10c209.p.ssafy.io";

        public void Connect()
        {
            try
            {
                // UDP 서버에 연결
                _udpClient = new UdpClient(hostname, 5058);
                _ipEndPoint = new IPEndPoint(IPAddress.Any, 0);
                
                Debug.Log("LiveServer connect success");
            }
            catch (Exception e)
            {
                Debug.Log($"Failed to connect LiveServer: {e.Message}");
            }
        }

        // 메시지 수신
        public (byte[], int) LiveReceiver()
        {
            if (_udpClient == null) return (null, 0);
            try
            {
                while (_udpClient.Receive(ref _ipEndPoint).Length > 0)
                {
                    byte[] buffer = _udpClient.Receive(ref _ipEndPoint);

                    if (buffer.Length > 0)
                    {
                        return (buffer, buffer.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("라이브서버 응답 읽기 실패 : " + e.Message);
            }

            return (null, 0);
        }
        
        // 서버로 메시지 보내기
        public void Deliver(byte[] message)
        {
            if (_udpClient == null) return;

            _udpClient.Send(message, message.Length, hostname, 5058);
        }

        public void DisconnectFromServer()
        {
            // 연결 종료
            _udpClient.Close();
            _ipEndPoint = null;
        }
    }
}