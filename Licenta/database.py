import psycopg2 as p
import sqlite3
import hashlib

class Database:
    def __init__(self):
        self.conn = p.connect(database='postgres', user='postgres', password='root')
        self.c = self.conn.cursor()

    def create_table(self):
        self.c.execute(
            "CREATE TABLE USERS (ID serial  PRIMARY KEY  , USERNAME VARCHAR(255), PASSWORD text)")
        self.conn.commit()


    def insert_user(self,username,password):
        #password = self.hash_password(password)
        if self.user_exists(username) == 0:
            print(password)
            self.c.execute('INSERT into USERS (USERNAME,PASSWORD) VALUES (%s,%s)',
                           (username, password))
            self.conn.commit()
        return

    # def verify_user(self,username,password):
    #     password=self.hash_password(password)
    #     self.c.execute('SELECT COUNT(*) FROM USERS WHERE USERNAME=%s AND PASSWORD=%s'
    #                    , (username,password,))
    #     data = self.c.fetchall()
    #     return data[0][0]

    def verify_user(self,username,password):
       #password=self.hash_password(password)
        self.c.execute('SELECT * FROM USERS')
        data=self.c.fetchall()
        for item in data:
            print (item)
            print (password)
            if item[1] == username and item[2]==password:
                return 1
        return 0

    def user_exists(self,username):
        self.c.execute('SELECT COUNT(*) FROM USERS WHERE USERNAME=%s'
                       ,(username,))
        data = self.c.fetchall()
        return data[0][0]

    def hash_password(self,password):
        m = hashlib.sha1()
        m.update(password.encode('utf-8'))
        return  m.digest()

    def close_connection(self):
        self.conn.close()

    def drop_database(self):
        self.c.execute('Drop table users')
        self.conn.commit()
    def delete_all(self):
        self.c.execute('delete from users')
        self.conn.commit()


x = Database()
#x.drop_database()
#x.create_table()
x.insert_user('stefan.cernescu','stefancernescu')
print(x.verify_user('stefan.cernescu','stefancernescu'))
#x.delete_all()
