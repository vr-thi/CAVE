﻿using System;
using UnityEngine;

namespace UnityClusterPackage
{
    class TimeSynchronizer
    {

        public static float deltaTime;
        public static float time;

        public static void Synchronize(NetworkNode node)
        {
            if (NodeInformation.type.Equals("master"))
            {
                node.BroadcastMessage(new SynchroMessage(SynchroMessageType.SetDeltaTime, Time.deltaTime));
                node.BroadcastMessage(new SynchroMessage(SynchroMessageType.SetTime, Time.time));
                deltaTime = Time.deltaTime;
                time = Time.time;

            }
            else
            {
                deltaTime = -1;
                time = -1;
                do
                {
                    SynchroMessage message = ((Client)node).WaitForNextMessage();

                    if (message.type == SynchroMessageType.SetDeltaTime)
                        deltaTime = (float)message.data;
                    else if (message.type == SynchroMessageType.SetTime)
                        time = (float)message.data;
                    else
                        throw new Exception("Received unexpected message.");

                } while (time == -1 || deltaTime == -1);
            }
        }


    }
}