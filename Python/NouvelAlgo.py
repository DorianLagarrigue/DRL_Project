# -*- coding: utf-8 -*-
"""
Created on Fri Dec  2 16:32:12 2022

@author: Etudiant1
"""

import TestCommunication as comm
import Brain as brain
import os
import tensorflow as tf
import numpy as np
from tensorflow import keras

import matplotlib.pyplot as plt


from collections import deque
#import time
import random
import gc

isTraining = False
train_episodes = 0
test_frequence = 0
nbBrains = 0
AllModels = []



class ProtocoleRec():
    eInitParametre = 0.0
    eState = 1.0
    eNextState = 2.0
    eReadyToStart = 3.0

protocoleRec = ProtocoleRec() 


def UnpackStateMsg(Msg):
    flag = Msg.pop(0)
    #print("Flag: "+ str(protocoleRec.eState))
    if flag == protocoleRec.eInitParametre:
        return Msg
    if flag == protocoleRec.eState:
        return [flag, Msg]
    if flag == protocoleRec.eNextState:
        return [flag, Msg]
    if flag == protocoleRec.eReadyToStart:
        print("Ready to Start")
    return

def Init(parameters):
    global train_episodes
    global test_frequence
    global AllModels
    global nbBrains
    
    print(parameters)
    
    ChooseGameMode(parameters.pop(0))
    nbBrains = int(parameters.pop(0))
    
    train_episodes = int(parameters.pop(0))
    test_frequence = int(parameters.pop(0))
        
    i = 0
    for i in range(nbBrains):
        AllModels.append(brain.Brain(i))
        
        action_shape = parameters.pop(0)
        state_shape = parameters.pop(0)
        
        learning_rate = parameters.pop(0)
        discount_factor = parameters.pop(0)
        
        memory_size = parameters.pop(0)
        batch_size = parameters.pop(0)
        
        layers = []
        while(parameters[0] != -1):
            layers.append(parameters.pop(0))
        
        parameters.pop(0)
        
        AllModels[i].InitModels(state_shape, action_shape, layers, memory_size, batch_size, learning_rate, discount_factor)

    
    



def ChooseGameMode(gm):
    global isLoadWeight
    global isTraining
    if gm == 0.0:
        isLoadWeight = False
        isTraining = True
        return
    if gm == 1.0:
        isLoadWeight = True
        isTraining = True
        return
    if gm == 1.0:
        isLoadWeight = True
        isTraining = False
        return

def GetEnviro():


    float_state = comm.ReceiveMsg()
    msg = UnpackStateMsg(float_state)
    
    flag = msg[0]
    enviro = msg[1]
    if flag != 1.0:
        print("erreur ordre message sate attendu")
        return ["error"]
    
    nbBrain = len(AllModels)
    AllEnviro = []
    for i in range(nbBrain):
        agentEnviro = []
        env = []
        for j in range(AllModels[i].state_shape):
            env.append(enviro.pop(0))
        isAiControl = int(enviro.pop(0))
        agentEnviro.append(np.array(env))
        agentEnviro.append(isAiControl)
        AllEnviro.append(agentEnviro)
        
    
    return AllEnviro


def ChooseAllAction(enviro, epsilon, isTest, isTraining):
    
    actions = []
    
    for i in range(len(AllModels)):
        actions.append(AllModels[i].ChooseActionNew(enviro[i], isTest, isTraining, epsilon))
    
    return actions


def GetNewEnviro():
    float_state = comm.ReceiveMsg()
    msg = UnpackStateMsg(float_state)
    
    flag = msg[0]
    enviro = msg[1]
    if flag != 2.0:
        print("erreur ordre message new sate attendu")
        return ["error"]
    
    nbBrain = len(AllModels)
    AllEnviro = []
    for i in range(nbBrain):
        agentEnviro = []
        env = []
        reward = enviro.pop(0)
        for j in range(AllModels[i].state_shape):
            env.append(enviro.pop(0))
        isAiControl = int(enviro.pop(0))
        isDone = int(enviro.pop(0))
        agentEnviro.append(reward)
        agentEnviro.append(np.array(env))
        agentEnviro.append(isAiControl)
        agentEnviro.append(isDone)
            
        AllEnviro.append(agentEnviro)
        
    
    return AllEnviro


def main():
    
    global AllModels
    global train_episodes
    global test_frequence
    global nbBrains
    
    epsilon = 1
    min_epsilon = 0.01
    decay = 0.995
    
    #A modifier, je ne sais pas pourquoi j'ai un start ici, il faudrait un autre message
    comm.SendMessage(comm.protocoleSend.eStart, 0)
    
    #Attendre le message du gamemode
    float_array = comm.ReceiveMsg()
    parameters = UnpackStateMsg(float_array)
    Init(parameters)
    
    
    
    #Init Algo
    steps_to_update_target_model = 0
    is_test_episode = False
    
    for episode in range(train_episodes):
        is_all_done = False
        
        i=0
        for i in range(nbBrains):
            AllModels[i].InitEpisode()
        
        comm.SendMessage(comm.ProtocoleSend.eStart, 0)
        
        is_test_episode = episode%test_frequence == 0 and episode != 0
        if is_test_episode:
            print("TEST"+str(episode))
        
        while(is_all_done == False):
            #Attendre état du jeu
            steps_to_update_target_model += 1
            all_enviros = GetEnviro()
            if all_enviros[0] == "error":
                return
            
            actions = []
            i=0
            for i in range(nbBrains):
                action = AllModels[i].ChooseActionNew(all_enviros[i], is_test_episode, isTraining, epsilon)
                actions.append(action)
            comm.SendMessage(comm.ProtocoleSend.eAction, actions)
            
            #Attendre new state
            all_new_state = GetNewEnviro()
            if all_new_state[0] == "error":
                return
            
            
            
            
            if isTraining == True:
                i=0
                for i in range(nbBrains):
                    AllModels[i].AddToReplayMemory(actions[i], all_new_state[i])
            
            i = 0
            is_all_done = True
            for i in range(nbBrains):
                is_all_done = is_all_done and AllModels[i].isAllDone()
            
            
            if is_all_done == True:
                #Done
                if isTraining == True:
                    #reward list append
                    if is_test_episode == True:
                        #Save model
                        i = 0
                
                    if steps_to_update_target_model >= 100:
                        i=0
                        for i in range(nbBrains):
                            AllModels[i].copyModels()
                            steps_to_update_target_model = 0
                            break
            else:
                i=0
                comm.SendMessage(comm.ProtocoleSend.eNextStep, 0)
         
        if is_test_episode == False and isTraining == True:
            i=0
            for i in range(nbBrains):
                AllModels[i].train()
            
            if(epsilon >= min_epsilon):
                epsilon *= decay
        if episode < train_episodes-1:
            i=0
            comm.SendMessage(comm.ProtocoleSend.eNextLoop, 0)
            float_Continue = comm.ReceiveMsg()
            UnpackStateMsg(float_Continue)
            del float_Continue
        
        keras.backend.clear_session()
        gc.collect()    
    
    comm.SendMessage(comm.ProtocoleSend.eStop, 0)
    
    for i in range(len(AllModels)):
        AllModels[i].DrawGraphs()
    
            
main()