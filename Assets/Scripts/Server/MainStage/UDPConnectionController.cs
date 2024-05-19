using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
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
        // 메시지 수신 (비동기 방식으로 변경)
        public IEnumerator LiveReceiverCoroutine()
        {
            if (_udpClient == null) yield break;

            while (true)
            {
                var asyncResult = _udpClient.BeginReceive(null, null);

                while (!asyncResult.IsCompleted)
                {
                    yield return null; // 데이터가 도착할 때까지 다음 프레임까지 대기
                }

                try
                {
                    var receivedData = _udpClient.EndReceive(asyncResult, ref _ipEndPoint);
                    if (receivedData.Length > 0)
                    {
                        Debug.Log("요청 on");
                        MessageHandler.UnPackUDPMessage(receivedData, receivedData.Length);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("라이브서버 응답 읽기 실패 : " + e.Message);
                }
            }
        }
        // public async Task<(byte[], int)> LiveReceiverAsync()
        // {
        //     if (_udpClient == null) return (null, 0);
        //     try
        //     {
        //         // 비동기적으로 데이터 수신
        //         var udpReceiveResult = await _udpClient.ReceiveAsync();
        //         byte[] buffer = udpReceiveResult.Buffer;
        //
        //         if (buffer.Length > 0)
        //         {
        //             return (buffer, buffer.Length);
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.Log("라이브서버 응답 읽기 실패 : " + e.Message);
        //     }
        //     return (null, 0);
        // }

        // public (byte[], int) LiveReceiver()
        // {
        //     if (_udpClient == null) return (null, 0);
        //     try
        //     {
        //         while (_udpClient.Receive(ref _ipEndPoint).Length > 0)
        //         {
        //             byte[] buffer = _udpClient.Receive(ref _ipEndPoint);
        //
        //             if (buffer.Length > 0)
        //             {
        //                 return (buffer, buffer.Length);
        //             }
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.Log("라이브서버 응답 읽기 실패 : " + e.Message);
        //     }
        //
        //     return (null, 0);
        // }

        public void DisconnectFromServer()
        {
            // 연결 종료
            _udpClient.Close();
            _ipEndPoint = null;
        }
    }
}