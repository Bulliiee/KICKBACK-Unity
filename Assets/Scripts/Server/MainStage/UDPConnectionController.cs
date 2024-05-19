using System;
using System.Net;
using System.Net.Sockets;
using MessagePack;
using UnityEngine;

namespace Highlands.Server
{
    public class UDPConnectionController
    {
        private UdpClient _udpClient;
        private IPEndPoint _ipEndPoint;

        private string hostname = "k10c209.p.ssafy.io";
        // private string hostname = "localhost";

        
        // 도메인 -> IPEndPoint
        private IPEndPoint CreateIPEndPointFromDomain(string hostname, int port)
        {
            // 도메인 이름으로부터 IP 주소를 가져오기
            IPAddress[] addresses = Dns.GetHostAddresses(hostname);

            if (addresses.Length == 0)
            {
                throw new ArgumentException("해당 호스트 이름에 대한 IP 주소를 찾을 수 없습니다.", nameof(hostname));
            }

            // 첫 번째 IP 주소를 사용하여 IPEndPoint를 생성
            // 주로 IPv4 주소를 사용하지만, 특정 도메인이 IPv6 주소만 가지고 있을 경우를 대비하여 체크
            foreach (var address in addresses)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork) // IPv4 주소
                {
                    return new IPEndPoint(address, port);
                }
            }

            // IPv4 주소가 없는 경우, IPv6 주소를 대신 사용하는 상황에 대비한 코드
            foreach (var address in addresses)
            {
                if (address.AddressFamily == AddressFamily.InterNetworkV6) // IPv6 주소
                {
                    return new IPEndPoint(address, port);
                }
            }

            throw new ArgumentException("유효한 IPv4 또는 IPv6 주소를 찾을 수 없습니다.", nameof(hostname));
        }

        // UDP 연결 정보 설정
        public void Connect()
        {
            try
            {
                // UDP 서버 설정
                _udpClient = new UdpClient();
                _ipEndPoint = CreateIPEndPointFromDomain(hostname, 5058);
            }
            catch (Exception e)
            {
                Debug.Log($"Failed to connect LiveServer: {e.Message}");
            }
        }

        // 메시지 전송
        public void Send(byte[] message)
        {
            _udpClient.Send(message, message.Length, _ipEndPoint);
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

        public void DisconnectFromServer()
        {
            // 연결 종료
            _udpClient.Close();
            _ipEndPoint = null;
        }
    }
}