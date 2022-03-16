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

# Seperation of Concerns
# To array is a common function that is used in many places
# It converts iterable objects to arrays
# e.g. a string to an array of chars
def toArray(input = []):
    result = []
    counter = 0
    while counter < len(input):
        result.append(input[counter])
        counter += 1


# Compare 2 bit strings, find how many bits are different between them
def hamming(a, b):
    counter = 0
    distance = 0
    while counter < len(a):
        if a[counter] == b[counter]:
            distance += 1
        counter += 1
    return distance


# Finds how different, many binary strings are from each other
# As in how many bits are different between the query bit string and the data set of bit strings
# @returns an array of the hamming distances (distance of bit strings from query) in percentages,
#          100% means all the bits in the bit string are different
def hammingSearch(query, dataset):
    hamming_array = []
    total = len(query)

    i = 0
    while i < len(dataset):
        distance = hamming(query, dataset[i])
        hamming_array.append(round(float(distance) / float(total), 4) * 100)
        i += 1

    return hamming_array


length = 100
# fillAndSaveArray(10,length) # populate new data (rewrites file)
dataArray = readArray()
len(dataArray)
queryString = genRandBinary(length)
print(hammingSearch(queryString, dataArray))