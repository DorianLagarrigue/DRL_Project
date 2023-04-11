# -*- coding: utf-8 -*-
"""
Created on Mon Nov 28 17:04:56 2022

@author: Etudiant1
"""
import tensorflow as tf
import numpy as np
from tensorflow import keras
import random
from dataclasses import dataclass
from collections import deque
import matplotlib.pyplot as plt




class Brain:
    def __init__(self, identifiant):
        self.learning_rate = 0.001
        self.discount_factor = 0.95
        
        #init  = tf.keras.initializers.HeUniform()
        self.model = keras.Sequential()
        self.target_model = keras.Sequential()
        self.replay_memory = deque(maxlen=2000)
        self.id = identifiant
        self.total_training_rewards = 0
        self.isDone = False
        self.lastAction = 0
        self.currentEnviro = []
        self.isAiControl = False
        self.trainList = []
        self.rewardList = []
        self.stepToCurve = 0
        self.batch_size = 128

    
    def InitModels(self, state_shape, action_shape, layers, memory_size, batch_size, learning_rate, discount_factor):#layers est un tableau de float (nb de neurones par layers)
        self.state_shape = int(state_shape)
        self.action_shape = int(action_shape)
        self.replay_memory = deque(maxlen=int(memory_size))
        self.batch_size = int(batch_size)
        self.learning_rate = learning_rate
        self.discount_factor = discount_factor
        
        
        
        for i in range(len(layers)):
            if i == 0:
                self.model.add(keras.layers.Dense(int(layers[i]), activation = 'relu', input_shape=(int(state_shape),) , name='layer0'))
                self.target_model.add(keras.layers.Dense(int(layers[i]), activation = 'relu', input_shape=(int(state_shape),) , name='layer0'))

            else:    
                self.model.add(keras.layers.Dense(int(layers[i]), activation = 'relu', name="layer"+str(i)))
                self.target_model.add(keras.layers.Dense(int(layers[i]), activation = 'relu', name="layer"+str(i)))

                
        self.model.add(keras.layers.Dense(int(action_shape), activation='linear', name='lastLayer'))
        self.target_model.add(keras.layers.Dense(int(action_shape), activation='linear', name='lastLayer'))

        self.model.compile(loss=tf.keras.losses.MeanSquaredError(), optimizer=tf.keras.optimizers.Adam(lr=self.learning_rate), metrics=['accuracy'])
        self.target_model.compile(loss=tf.keras.losses.MeanSquaredError(), optimizer=tf.keras.optimizers.Adam(lr=self.learning_rate), metrics=['accuracy'])
        
        self.target_model.set_weights(self.model.get_weights())

        self.lastAction = 0
        
        #_________________________TODO______________________
        #
        #if isLoadWeight:
            #self.model.load_weights(save_test_path)
        #
        #___________________________________________________
    
    
    
    def InitEpisode(self):
        self.currentEnviro = []
        self.lastAction = 0
        self.isAiControl = True
        self.isDone = False
        self.total_training_rewards = 0
    
    def AddToReplayMemory(self, actions, next_enviros):
        if self.isDone == True:
            return
        
        is_done = next_enviros[3]
        if is_done == 1:
            self.isDone = True
        
        #print(is_done)
        #CHECK ICI
        is_ai_control = next_enviros[2]
        self.isAiControl = is_ai_control != 0
            
        if self.isAiControl == False or self.currentEnviro == []:
            return
            
            
        action = actions
        reward = next_enviros[0]
        next_state = next_enviros[1]
        

        self.replay_memory.append([self.currentEnviro, self.lastAction, reward, next_state, is_done])
        self.currentEnviro = []
        self.lastAction = int(action)
        self.total_training_rewards+= reward
        
            
        return
    
    def ChooseActionNew(self, enviro, isTest, isTraining, epsilon):
        #enviro doit être un [[enviro], isAiControl, isAlreadyDone] des enviro genre une matrice
        actions = 0
        self.isAiControl = enviro[1] == 1
        if isTest or not isTraining:
                                
            if not self.isAiControl or self.isDone:
                actions = np.random.randint(low = 0, high = self.action_shape)
            else:   
                env = np.array(enviro[0])
                enviro_reshaped = env.reshape([1, env.shape[0]])#pas sûr d'ici
                predicted = self.model.predict(enviro_reshaped).flatten()
                self.lastAction = actions = np.argmax(predicted)
        else:

            if not self.isAiControl or self.isDone:
                    actions = np.random.randint(low = 0, high = self.action_shape)
            else:
                
                env = np.array(enviro[0])
                enviro_reshaped = env.reshape([1, env.shape[0]])
                self.currentEnviro = env
                                    
                random_number = np.random.rand()
                    
                if random_number <= epsilon:
                    #explore
                    
                    self.lastAction = actions = np.random.randint(low = 0, high = self.action_shape)
                else:
                    #pas sûr d'ici
                    predicted = self.model.predict(enviro_reshaped).flatten()
                    self.lastAction = actions = np.argmax(predicted)
            

        return actions
    
    def isAllDone(self):        
        return self.isDone
    
    def copyModels(self):
        self.target_model.set_weights(self.model.get_weights())

    
    def train(self):
        discount_factor = self.discount_factor
        
        MIN_REPLAY_SIZE_TRAIN = self.batch_size
        if len(self.replay_memory) <= MIN_REPLAY_SIZE_TRAIN:
            return
        
        
        mini_batch_train = random.sample(self.replay_memory, MIN_REPLAY_SIZE_TRAIN)
        current_states = np.array([transition[0] for transition in mini_batch_train])
        current_qs_list = self.model.predict(current_states)
        new_current_states = np.array([transition[3] for transition in mini_batch_train])
        future_qs_list = self.target_model.predict(new_current_states)
        #future_qs_list = model.predict(new_current_states)
        
        X_Train = []
        Y_Train = []
        
        max_future_q = 0
        
        for index, (enviro, action, reward, new_enviro, isDone) in enumerate(mini_batch_train):
            if isDone:
                max_future_q = reward
            else:
                max_future_q = reward + discount_factor * np.max(future_qs_list[index])
            
            current_qs = current_qs_list[index]
            #current_qs[action] = (1 - self.learning_rate) * current_qs[action] + self.learning_rate * max_future_q
            current_qs[action] = max_future_q
            
            
            X_Train.append(enviro)
            Y_Train.append(current_qs)
        
        
        
        
        #history = model.fit(np.array(X), np.array(Y), batch_size=batch_size, verbose=0, shuffle=True, callbacks=[cp_callback], epochs=1)
        history = self.model.fit(np.array(X_Train), np.array(Y_Train), batch_size=MIN_REPLAY_SIZE_TRAIN, verbose=0, shuffle=False, epochs=1)
        #self.model.fit
        
        
        if self.stepToCurve == 9:
            self.trainList.append(history.history["loss"][0])
            self.rewardList.append(self.total_training_rewards)
        self.stepToCurve = (self.stepToCurve+1)%10
    
    def DrawGraphs(self):
        plt.subplot(211)
        plt.plot(self.trainList, label="train", color="blue")
        plt.subplot(212)
        plt.plot(self.rewardList, label="reward", color="red")
        plt.show()


