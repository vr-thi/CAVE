﻿using System;
using System.Threading;
using AwesomeSockets.Sockets;
using UnityEngine;
using UnityEngine.UI;

namespace UnityClusterPackage
{
    public class Client : NetworkNode
    {

        public override void Connect()
        {
            int tryCounter = 0;
            nextTry:
            try
            {
                connections.Add(AweSock.TcpConnect(NodeInformation.serverIp, NodeInformation.serverPort + 1));
            }
            catch (Exception e)
            {
                Debug.Log("Could not connect to server. Trying again.");

                if (++tryCounter >= 10)
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                    return;
                }

                Thread.Sleep(500);
                goto nextTry;
            }
            InitializeSelf();
        }

        void InitializeSelf()
        {
            ParticleSynchronizer.InitializeFromClient(this);
            RigidBodySynchronizer.InitializeFromClient(this);
        }

        public override void FinishFrame()
        {
            BroadcastMessage(new SynchroMessage(SynchroMessageType.FinishedRendering, 0));
        }

        public SynchroMessage WaitForNextMessage()
        {
            return WaitForNextMessage(connections[0]);
        }

    }
}