import hvac


class Vault:
    def __init__(self):
        self.client = hvac.Client(url='http://localhost:8200', token='8ddaa675-5efa-ab3b-8055-46075af1a0da')
        # self.client.write('secret/stefan.cernescu', wrap_ttl=None,  key=['secret1','secret2'])
        # print(self.loadSecretData(self.client.read('secret/stefan.cernescu')))

    def loadSecretData(self, jsonData):
        return jsonData['data']

    def writeNewData(self, buffer, dataKey, dataValue):
        dataToWrite = buffer
        newData = {dataKey: dataValue}
        dataToWrite.update(newData)
        return dataToWrite

    def writeSecret(self, secret, key, value):
        data = self.client.read('secret/' + secret)
        data = self.loadSecretData(data)
        newData = self.writeNewData(data, key, value)
        print(newData)
        self.client.write('secret/' + secret, wrap_ttl=None, **newData)

    def writeFirstSecret(self, username):
        self.client.write('secret/' + username, default=['value1', 'value2'])

    def readSecret(self, username):
        data = self.client.read('secret/' + username)['data']
        return data


x = Vault()
# x.writeSecret('stefan.cernescu','League',['username','password'])
print(x.readSecret("stefan.cernescu"))
