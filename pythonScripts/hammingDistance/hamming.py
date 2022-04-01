from random import randint
import os

# Based off of the example at: https://stackoverflow.com/a/5137509
# It ensures no matter what the CWD (current working directory) is, files intended will be found.
dir_path = os.path.dirname(os.path.realpath(__file__))

def generate_random_bitstring(length):
    counter = 0
    string = ""
    while counter < length:
        # A round about way of generating a random bit string
        string += str(round(randint(0, 10) / 10))
        counter += 1
    return string

def write_bitstring_to_file(num, length):
    counter = 1
    f = open(os.path.join(dir_path, 'data'), "a")
    f.write(generate_random_bitstring(length))
    while counter < num:
        f.write(f"\n{generate_random_bitstring(length)}")
        counter += 1
    f.close()

def readArray():
    f = open(os.path.join(dir_path, 'data'), "r")
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


# Compare 2 bit strings, find how many bits are the same between them
def hamming(a, b):
    counter = 0
    distance = 0
    while counter < len(a):
        if a[counter] == b[counter]:
            distance += 1
        counter += 1
    return distance


# Finds how similar many binary strings are to each other
# As in how many bits are the same between the query bit string and the data set of bit strings
# @returns an array of the hamming distances (similarities between bit strings from query) in percentages,
#          100% means all the bits in the bit string are the same
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
# write_bitstring_to_file(10,length) # populate new data (rewrites file)
dataArray = readArray()
len(dataArray)
queryString = generate_random_bitstring(length)
print(hammingSearch(queryString, dataArray))