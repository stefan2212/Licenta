import socket
from threading import Thread

import client
import database
import Crypto
from Crypto.PublicKey import RSA
from Crypto import Random




class server:
    def __init__(self):
        self.x = client.Vault()
        self.db = database.Database()
        random_generator=Random.new().read
        key = RSA.generate(2048,random_generator)
        publicKey = key.publickey()
        HOST, PORT = "localhost", 8888
        s = socket.socket()
        s.bind((HOST, PORT))
        s.listen(6)
        while True:
            self.c, addr = s.accept()
            i = 0
            while True:
                dataLen = self.c.recv(2)
                if not dataLen:
                    self.c.close()
                    break
                data = self.c.recv(int(dataLen))
                i += 1
                if not data:
                    self.c.close()
                    break
                #print(str(data))
                if str(data)[2:-1] == "LoginWithUser":
                    usernameLen = self.c.recv(2)
                    self.username = self.c.recv(int(usernameLen))
                    passwordLen = self.c.recv(2)
                    password = self.c.recv(int(passwordLen))
                    self.answer = self.login(self.username, password)
                    self.c.send(str(self.answer).encode("utf-8"))
                elif str(data)[2:-1] == "RegisterWithUser":
                    usernameLen = self.c.recv(2)
                    username = self.c.recv(int(usernameLen))
                    passwordLen = self.c.recv(2)
                    password = self.c.recv(int(passwordLen))
                    self.answer = self.login(username, password)
                elif str(data)[2:-1] == "ExportMyPasswords":
                    titleLen = self.c.recv(2)
                    title = self.c.recv(int(titleLen))
                    usernameLen = self.c.recv(2)
                    username = self.c.recv(int(usernameLen))
                    passwordLen = self.c.recv(2)
                    password = self.c.recv(int(passwordLen))
                    self.exportData(self.username, title, username, password)
                elif str(data)[2:-1] == "ImportMyPasswords":
                    self.importData(self.username)

    def importData(self, username):
        data = self.x.readSecret(str(username)[2:-1])
        numberOfItem = len(data)
        self.c.send(str(numberOfItem).encode("utf-8"))
        for item in data:
            #print("Sending " + str(item))
            self.c.send(str(item).encode("utf-8"))
            self.c.send(str(data[item][0]).encode("utf-8"))
            #print("Sending " + str(data[item][0]))
            self.c.send(str(data[item][1]).encode("utf-8"))
            #print("Sending " + str(data[item][1]))
        return

    def exportData(self, account, title, username, password):
        self.x.writeSecret(str(self.username)[2:-1], str(title)[2:-1], (str(username)[2:-1], str(password)[2:-1]))
        return

    def register(self, username, password):
        self.db.insert_user(str(username)[2:-1], str(password)[2:-1])

        return

    def login(self, username, password):
        return self.db.verify_user(str(username)[2:-1], str(password)[2:-1])

serv = server()
