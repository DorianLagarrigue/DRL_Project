# -*- coding: utf-8 -*-
"""
Created on Mon Oct 17 10:07:49 2022

@author: Etudiant1
"""
import array
import struct
import io;
import gc
import socket
from enum import Enum

    

class ProtocoleSend():
    eStart = 0
    eAction = 1
    eNextStep = 2
    eNextLoop = 3
    eStop = 4




def ReceiveMsg():
    isMsgReceive = False
    
    while not(isMsgReceive):
        
        receivedData = sock.recv(10000)#receiveing data in Byte from C#
        if len(receivedData):
            lengthMsg = len(receivedData)
            i = 0
            float_array = []
            while(i < lengthMsg):
                floatNumber = struct.unpack('<f',receivedData[i:i+4])
                float_array.append(floatNumber[0])
                i+=4
            
            isMsgReceive = True
        del receivedData
        
    return float_array
            


def SendMessage(protocole, action):
    #print(action)
    data = []
    data.append(protocole)
    if protocole == protocoleSend.eAction:
        for i in range(len(action)):
            data.append(action[i])
    
    sock.sendall(bytes(data), 0)
    
    del data
    
def SendMessageSingle(protocole, action):
    data = []
    data.append(protocole)
    if protocole == protocoleSend.eAction:
        data.append(action)
    sock.sendall(bytes(data),0)
    del data



#import time

protocoleSend = ProtocoleSend()
host, port = "127.0.0.1", 666
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect((host, port))




    
    
    