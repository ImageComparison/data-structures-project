from random import randint


def genRandBinary(length):
    counter = 0
    string = ""
    while counter < length:
        rng = randint(0, 10)
        if rng <= 5:
            randNum = "0"
        else:
            randNum = "1"
        string = string + randNum
        counter += 1
    return string

def fillAndSaveArray(num, length):
    counter = 1
    f = open("data", "a")
    f.write(genRandBinary(length))
    while counter < num:
        f.write(f"\n{genRandBinary(length)}")
        counter += 1
    f.close()

def readArray():
    f = open("data", "r")
    returnVal = f.read()
    f.close()
    returnVal = returnVal.split("\n")
    return returnVal

def hammingSearch(query, data):
    hammingArray = []
    queryArray = []
    dataChars = []
    total = len(query)

    counter = 0
    while counter < len(query):
        queryArray.append(query[counter])
        counter += 1
    counter = 0
    while counter < len(data):  # for everyone in set
        datat = data[counter]
        dataArray = []
        intCounter = 0
        while intCounter < len(datat):  # for every char in element
            dataArray.append(datat[intCounter])
            intCounter += 1
        dataChars.append(dataArray)
        counter += 1
    counter = 0
    while counter < len(data):
        intCounter = 0
        score = 0
        while intCounter < len(query):
            if data[counter][intCounter] == queryArray[intCounter]:
                score += 1
            intCounter += 1
        hammingArray.append(round(float(score)/float(total), 4)*100)
        counter += 1
    return hammingArray


length = 100
# fillAndSaveArray(10,length) # populate new data (rewrites file)
dataArray = readArray()
len(dataArray)
queryString = genRandBinary(length)
print(hammingSearch(queryString, dataArray))