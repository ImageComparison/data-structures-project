import os
import PIL
import numpy as np
from PIL import Image

runModes = ['singleIMG', 'imageDIR', 'userInput']
runMode = "singleIMG"

filename = 'img_10007.jpg'  # singleIMG only
dirname = 'profs-images'  # imageDIR default
searchForImage = False  # search for an image
queryImage = 'img_mystery.jpg'  # if so what image
verbose = True  # print steps and date during run


# do not touch
dataLoadingComplete = False
scannedImages, scannedImagesBinary = [], []
queryImageBinary, data, projectionData0 = "", "", ""
projectionData45, projectionData90, projectionData135, Th_P1 = "", "", "", ""
Th_P2, Th_P3, Th_P4, binaryProjData0, binaryProjData45, binaryProjData90, binaryProjData135 = "", "", "", "", "", "", ""

# if (verbose == True):
#     # prints raw image data in array
#     for row in range(28):
#         print(f"[{row+1}/28]", data[row])


def project_0deg(data = []):
    returnData = []
    for row in range(28):
        rowSumData = 0
        for cell in range(28):
            rowSumData += data[row][cell]
        returnData.append(rowSumData)
    return returnData
def project_45deg(data = []):
    diags = [data[::1,:].diagonal(i) for i in range(-data.shape[0]+1,data.shape[1])]
    counter = 0
    while counter < len(diags):
        diags[counter] = list(diags[counter])
        counter += 1
    counter = 0
    while counter < len(diags):
        if len(diags[counter]) == 1:
            diags.pop(counter)
        counter += 1
    return diags
def project_90deg(data = []):
    returnData = []
    for col in range(28):
        colSumData = 0
        for cell in range(28):
            colSumData += data[cell][col]
        returnData.append(colSumData)
    return returnData
def project_135deg(data = []):
    diags = [data[::-1,:].diagonal(i) for i in range(-data.shape[0]+1,data.shape[1])]
    counter = 0
    while counter < len(diags):
        diags[counter] = list(diags[counter])
        counter += 1
    counter = 0
    while counter < len(diags):
        if len(diags[counter]) == 1:
            diags.pop(counter)
        counter += 1
    return diags
# Project and array in angles 0, 45, 90, 135
def project(angle):
    global data
    # match angle:
    #     case 0:
    #         return project_0deg(data)
    #     case 45:
    #         return project_45deg(data)
    #     case 90:
    #         return project_90deg(data)
    #     case 135:
    #         return project_135deg(data)
    #     case _:
    #         print("Angle was most likely entered incorrectly!")
    #         return None

    # went back to python 3.9 which apparently does not support matching
    if angle == 0:
        return project_0deg(data)
    elif angle == 45:
        return project_45deg(data)
    elif angle == 90:
        return project_90deg(data)
    elif angle == 135:
        return project_135deg(data)
# if (verbose == True):
#     print("RAY DATA:")
#     print("<0 DEG>", projectionData0)
#     print("<45 DEG>", projectionData45)
#     print("<90 DEG>", projectionData90)
#     print("<135 DEG>", projectionData135)
#     print("")
def findProjThreshold(projection):
    sum = 0
    length = len(projection)
    for element in range(length):
        sum += projection[element]
    threshold = sum/length
    return threshold
# if (verbose == True):
#     print("THRESHOLDS:")
#     print("<0 DEG THRESHOLD>", Th_P1)
#     print("<45 DEG THRESHOLD>", Th_P2)
#     print("<90 DEG THRESHOLD>", Th_P3)
#     print("<135 DEG THRESHOLD>", Th_P4)
#     print("")
def convertProjToBinary(projection, threshold):
    returnBinaryArray = []
    length = len(projection)
    for element in range(length):
        if(projection[element] > threshold):
            returnBinaryArray.append(1)
        else:
            returnBinaryArray.append(0)
    return returnBinaryArray
# if (verbose == True):
#     print("RAY DATA:")
#     print("<0 DEG BINARY>", binaryProjData0)
#     print("<45 DEG BINARY>", binaryProjData45)
#     print("<90 DEG BINARY>", binaryProjData90)
#     print("<135 DEG BINARY>", binaryProjData135)
#     print("")
def analyzeImage(filename):
    global queryImageBinary, dataLoadingComplete, scannedImagesBinary, runMode, data, projectionData0, projectionData45, projectionData90, projectionData135, Th_P1, Th_P2, Th_P3, Th_P4, binaryProjData0, binaryProjData45, binaryProjData90, binaryProjData135
    if runMode == "singleIMG":  # PRINT IF RUN PROGRAM ON A SINGULAR IMAGE FILE
        print(f"Analysing {filename}")

    # OPEN FILE AND GET IMAGE DATA
    img = PIL.Image.open(filename)
    pix = img.load()

    # SAVE GRAYSCALE DATA TO A LIST
    # counter = 1
    data = []
    for y in range(28):
        rowData = []
        for x in range(28):
            p = pix[x, y]
            rowData.append(p)
            # counter += 1
        data.append(rowData)

    data = np.array(data)

    # GET PROJECTIONS FROM IMAGE DATA ARRAY
    projectionData0 = project(0)
    projectionData45 = project(45)
    projectionData90 = project(90)
    projectionData135 = project(135)

    # FIND AVERAGES (AKA. THRESHOLDS) OF THE
    # FOUR PROJECTION ARRAYS (PYTHON LISTS)
    Th_P1 = findProjThreshold(projectionData0)
    # Th_P2 = findProjThreshold(projectionData45)
    Th_P3 = findProjThreshold(projectionData90)
    # Th_P4 = findProjThreshold(projectionData135)

    # LOOPS THROUGH THE FOUR ARRAYS AND FOR EACH
    # VALUE IF IT IS LOWER THAN THE ARRAY THRESHOLD
    # ASSIGNS ZERO, OTHERWISE ASSIGNS ONE (BINARY)
    binaryProjData0 = convertProjToBinary(projectionData0, Th_P1)
    # binaryProjData45 = convertProjToBinary(projectionData45, Th_P2)
    binaryProjData90 = convertProjToBinary(projectionData90, Th_P3)
    # binaryProjData135 = convertProjToBinary(projectionData135, Th_P4)

    # CONVERTS THE BINARY PROJECTION LISTS INTRO THEIR
    # STRING FORMS AND REMOVES SQUARE BRACKETS AROUND THEM
    binaryProjData0 = str(binaryProjData0)[1:len(binaryProjData0) - 1]
    # binaryProjData45 = str(binaryProjData45)[1:len(binaryProjData45) - 1]
    binaryProjData90 = str(binaryProjData90)[1:len(binaryProjData90) - 1]
    # binaryProjData135 = str(binaryProjData135)[1:len(binaryProjData135) - 1]

    # CONCATENATE (ADDS) THE FOUR PROJECTION STRINGS TOGETHER, REMOVING A TRAILING COMMA AT THE END OF THE LIST {?}
    concatenatedBinaryProj = binaryProjData0 + " " + binaryProjData45 + binaryProjData90 + " " + binaryProjData135[:-1]

    # REMOVE THIS AND USE THE ONE ABOVE !!
    # concatenatedBinaryProj = binaryProjData0 + " " + binaryProjData90

    # REMOVES THE COMMA AND SPACE SEPARATING EACH OF THE OTHER LIST ELEMENTS
    concatenatedBinaryProj = concatenatedBinaryProj.replace(", ", "")

    if runMode == "imageDIR":
        if(verbose == True):
            print(f"Analyzed {filename}: {concatenatedBinaryProj}")

        if(dataLoadingComplete == False):
            scannedImages.append(filename)
            scannedImagesBinary.append(concatenatedBinaryProj)
        else:
            queryImageBinary = concatenatedBinaryProj

    if runMode == "singleIMG":
        print(f"Binary: {concatenatedBinaryProj}")
        print("Done!")
def searchForImage():
    global queryImageBinary, queryImage, scannedImages, scannedImagesBinary

    # return hammingSearch(queryImageBinary, scannedImagesBinary)

    # return hammingSearch("1010", "1001")

    # # converts query image binary string into binary
    # qIBinary = bytes(queryImageBinary)
    #
    # # converts data set into binary
    # dataBinary = []
    # for binary in scannedImagesBinary:
    #     dataBinary.append(bytes(binary))
    #
    # # RIGHT NOW THIS CHECKS ONLY FOR AN EXACT MATCH !
    # print("RIGHT NOW THIS CHECKS ONLY FOR AN EXACT MATCH !")
    # counter = 0
    # for binary in dataBinary:
    #     if binary == qIBinary:
    #         print(f"Exact match found with file: {scannedImages[counter]}")
    #     counter += 1

# old code !!!
# def hammingSearch(query, data):
#     hammingArray = []
#     queryArray = []
#     dataChars = []
#     total = len(query)
#
#     counter = 0
#     while counter < len(query):
#         queryArray.append(query[counter])
#         counter += 1
#     counter = 0
#     while counter < len(data):  # for everyone in set
#         datat = data[counter]
#         dataArray = []
#         intCounter = 0
#         while intCounter < len(datat):  # for every char in element
#             dataArray.append(datat[intCounter])
#             intCounter += 1
#         dataChars.append(dataArray)
#         counter += 1
#     counter = 0
#     while counter < len(data):
#         intCounter = 0
#         score = 0
#         while intCounter < len(query):
#             if data[counter][intCounter] == queryArray[intCounter]:
#                 score += 1
#             intCounter += 1
#         hammingArray.append(round(float(score)/float(total), 4)*100)
#         counter += 1
#     return hammingArray

def hamming_distance(str1, str2):
    length = len(str1)
    if length != len(str2):
        return None
    d = 0
    for c1, c2 in zip(str1, str2):  # zip() Iterate over several iterables in parallel
        if c1 != c2:
            d = d + 1
    return 1-float(d/length)

def startProgram(runMode, runModes):
    global scannedImages, dataLoadingComplete, queryImage, searchForImage, filename, dirname
    validMode = False
    for x in runModes:
        if runMode == x:
            validMode = True
    if validMode:
        if runMode == "singleIMG":
            analyzeImage(filename)
            if searchForImage == True:
                dataLoadingComplete = True
                analyzeImage(queryImage)
                searchForImage()
        elif runMode == "imageDIR":
            path = "images/" + dirname + "/"
            originalPath = path
            filesInDIR = []
            interiorDIRS = []
            for file in os.listdir(path):
                filename = os.fsdecode(file)
                if filename.find(".") == -1:
                    interiorDIRS.append(filename)
                else:
                    filesInDIR.append(filename)
                    print(filename)

                # SCANS IMAGES IN DIRS INSIDE DIR
                for dir in interiorDIRS:
                    path = originalPath
                    path = path + dir + "/"
                    for file in os.listdir(path):
                        filepath = path + os.fsdecode(file)
                        analyzeImage(filepath)

                # SCANS IMAGES IN DIR
                for file in filesInDIR:
                    analyzeImage(file)

            print(f"Finished scanning {len(scannedImages)} images!")

            # SEARCHES FOR IMAGES LIKE QUERY IMAGE
            if searchForImage == True:
                dataLoadingComplete = True
                analyzeImage(queryImage)
                searchForImage()

        elif runMode == "userInput":
            # RUN DEFAULT FILES?
            providedFilesInputIsValid = False
            providedFalseInput = input("1) Do you want to run the professors files: y/n")
            if providedFalseInput == "y" or providedFalseInput == "n":
                providedFilesInputIsValid = True
            while providedFilesInputIsValid != True:
                providedFalseInput = input("1) Do you want to run the professors files: y/n")
                if providedFalseInput == "y" or providedFalseInput == "n":
                    providedFilesInputIsValid = True

            if providedFalseInput == "n":
                # SINGLE IMAGE OR DIRECTORY ?
                runModeInputIsValid = False
                print("\n2) Do you want to run a.. ")
                print("a) single image")
                print("b) or a directory of images")
                runModeInput = input("")
                if runModeInput == "a" or runModeInput == "b":
                    runModeInputIsValid = True
                while runModeInputIsValid != True:
                    print("2) Do you want to run a.. ")
                    print("a) single image")
                    print("b) or a directory of images")
                    runModeInput = input("")
                    if runModeInput == "a" or runModeInput == "b":
                        runModeInputIsValid = True

                # WHAT IS THE FILENAME OF THE TARGET IMAGE ?
                targetImageFileInputIsValid = False
                print("\n3) What is the filename of the query image..")
                targetImageFileInput = input("Note: it should be located in the 'single-images' directory: ")
                filePath = "images/single-images/" + targetImageFileInput
                if os.path.isfile(filePath):
                    targetImageFileInputIsValid = True
                while targetImageFileInputIsValid != True:
                    print("3) What is the filename of the query image..")
                    targetImageFileInput = input("Note: it should be located in the 'single-images' directory: ")
                    filePath = "images/single-images/" + targetImageFileInput
                    if os.path.isfile(filePath):
                        targetImageFileInputIsValid = True

                # DO YOU HAVE A QUERY IMAGE ?
                queryImageInputIsValid = False
                queryImageInput = input("\n4) Do you have a query image: y/n")
                if queryImageInput == "y" or queryImageInput == "n":
                    queryImageInputIsValid = True
                while queryImageInputIsValid != True:
                    queryImageInput = input("4) Do you have a query image: y/n")
                    if queryImageInput == "y" or queryImageInput == "n":
                        queryImageInputIsValid = True

                if queryImageInput == "y":
                    # WHAT IS THE FILENAME OF THE QUERY IMAGE ?
                    queryImageFileInputIsValid = False
                    print("\n5) What is the filename of the query image..")
                    queryImageFileInput = input("Note: it should be located in the 'queryIMGs' directory: ")
                    filePath = "queryIMGs/" + queryImageFileInput
                    if os.path.isfile(filePath):
                        queryImageFileInputIsValid = True
                    while queryImageFileInputIsValid != True:
                        print("5) What is the filename of the query image..")
                        queryImageFileInput = input("Note: it should be located in the 'queryIMGs' directory: ")
                        filePath = "queryIMGs/" + queryImageFileInput
                        if os.path.isfile(filePath):
                            queryImageFileInputIsValid = True

                print("action here")
            else:
                dirname = 'profs-images'
                startProgram("imageDIR", runModes)
    else:
        print("Run mode was set incorrectly!")


startProgram(runMode, runModes)


# x = 28
# arr = np.arange(784).reshape(x, x)
# print(np.diagonal(arr, 0, 0, 1), "\n")
