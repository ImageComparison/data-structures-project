import os

import cffi
import PIL
from PIL import Image

# NOTES/REMINDERS
#  - change code for 45 deg projection

# SETTINGS

runModes = ['singleIMG', 'imageDIR', 'userInput']
runMode = "imageDIR"  # userInput is not yet working

# [runMode:singleIMG] assumed to be in same dir as main.py
filename = 'img_10007.jpg'

# [runMode:imageDIR] assumed to be in images dir
dirname = 'profs-images'

# assumed to be in same dir as main.py
searchForImage = True
queryImage = 'img_mystery.jpg'

# SETTINGS: BROADCASTS
printRawImageArrays = False
printImageRayArrays = False
printThresholdValues = False
printBinaryArrays = False
printResultsForEachImage = False

# PROGRAM FUNCTIONS
dataLoadingComplete = False
scannedImages, scannedImagesBinary = [], []
queryImageBinary, data, projectionData0, projectionData45, projectionData90, projectionData135, Th_P1, Th_P2, Th_P3, Th_P4, binaryProjData0, binaryProjData45, binaryProjData90, binaryProjData135 = "", "", "", "", "", "", "", "", "", "", "", "", "", ""
if (printRawImageArrays == True):
    # prints raw image data in array
    for row in range(28):
        print(f"[{row+1}/28]", data[row])
def project(angle):
    global data
    if (angle == 0 or angle == 45 or angle == 90 or angle == 135):
        returnData = []
        if(angle == 0):
            for row in range(28):
                rowSumData = 0
                for cell in range(28):
                    rowSumData += data[row][cell]
                returnData.append(rowSumData)
            return returnData
        elif(angle == 45):
            k = 0
            raySumData = 0
            rot_data = list(reversed(list(zip(*data))))
            while (k <= 28 + 28 - 2):
                j = 0
                while (j <= k):
                    i = k - j
                    if (i < 28 and j < 28):
                        raySumData += rot_data[i][j]
                    j += 1
                returnData.append(raySumData)
                k += 1
            return returnData
        elif(angle == 90):
            for col in range(28):
                colSumData = 0
                for cell in range(28):
                    colSumData += data[cell][col]
                returnData.append(colSumData)
            return returnData
        elif(angle == 135):
            k = 0
            raySumData = 0
            while (k <= 28 + 28 - 2):
                j = 0
                while (j <= k):
                    i = k - j
                    if (i < 28 and j < 28):
                        raySumData += data[i][j]
                    j += 1
                returnData.append(raySumData)
                k += 1
            return returnData
    else:
        print("Angle was most likely entered incorrectly!")
        return None
if (printImageRayArrays == True):
    print("RAY DATA:")
    print("<0 DEG>", projectionData0)
    print("<45 DEG>", projectionData45)
    print("<90 DEG>", projectionData90)
    print("<135 DEG>", projectionData135)
    print("")
def findProjThreshold(projection):
    sum = 0
    length = len(projection)
    for element in range(length):
        sum += projection[element]
    threshold = sum/length
    return threshold
if (printThresholdValues == True):
    print("THRESHOLDS:")
    print("<0 DEG THRESHOLD>", Th_P1)
    print("<45 DEG THRESHOLD>", Th_P2)
    print("<90 DEG THRESHOLD>", Th_P3)
    print("<135 DEG THRESHOLD>", Th_P4)
    print("")
def convertProjToBinary(projection, threshold):
    returnBinaryArray = []
    length = len(projection)
    for element in range(length):
        if(projection[element] <= threshold):
            returnBinaryArray.append(0)
        else:
            returnBinaryArray.append(1)
    return returnBinaryArray
if (printBinaryArrays == True):
    print("RAY DATA:")
    print("<0 DEG BINARY>", binaryProjData0)
    print("<45 DEG BINARY>", binaryProjData45)
    print("<90 DEG BINARY>", binaryProjData90)
    print("<135 DEG BINARY>", binaryProjData135)
    print("")
def analyzeImage(filename):
    global queryImageBinary, dataLoadingComplete, scannedImagesBinary, runMode, data, projectionData0, projectionData45, projectionData90, projectionData135, Th_P1, Th_P2, Th_P3, Th_P4, binaryProjData0, binaryProjData45, binaryProjData90, binaryProjData135
    # RUN PROGRAM ON A SINGULAR IMAGE FILE
    if runMode == "singleIMG":
        print(f"Analysing {filename}")

    # OPEN FILE AND GET IMAGE DATA
    img = PIL.Image.open(filename)
    pix = img.load()

    # SAVE GRAYSCALE DATA TO AN ARRAY
    counter = 1
    data = []
    for y in range(28):
        rowData = []
        for x in range(28):
            p = pix[x, y]
            rowData.append(p)
            counter += 1
        data.append(rowData)

    # GET PROJECTIONS FROM IMAGE DATA ARRAY
    projectionData0 = project(0)
    projectionData45 = project(45)
    projectionData90 = project(90)
    projectionData135 = project(135)

    # FIND AVERAGES (AKA. THRESHOLDS) OF THE
    # FOUR PROJECTION ARRAYS (PYTHON LISTS)
    Th_P1 = findProjThreshold(projectionData0)
    Th_P2 = findProjThreshold(projectionData45)
    Th_P3 = findProjThreshold(projectionData90)
    Th_P4 = findProjThreshold(projectionData135)

    # LOOPS THROUGH THE FOUR ARRAYS AND FOR EACH
    # VALUE IF IT IS LOWER THAN THE ARRAY THRESHOLD
    # ASSIGNS ZERO, OTHERWISE ASSIGNS ONE (BINARY)
    binaryProjData0 = convertProjToBinary(projectionData0, Th_P1)
    binaryProjData45 = convertProjToBinary(projectionData45, Th_P2)
    binaryProjData90 = convertProjToBinary(projectionData90, Th_P3)
    binaryProjData135 = convertProjToBinary(projectionData135, Th_P4)

    # CONVERTS THE BINARY PROJECTION LISTS INTRO THEIR
    # STRING FORMS AND REMOVES SQUARE BRACKETS AROUND THEM
    binaryProjData0 = str(binaryProjData0)[1:len(binaryProjData0) - 1]
    binaryProjData45 = str(binaryProjData45)[1:len(binaryProjData45) - 1]
    binaryProjData90 = str(binaryProjData90)[1:len(binaryProjData90) - 1]
    binaryProjData135 = str(binaryProjData135)[1:len(binaryProjData135) - 1]

    # CONCATENATE (ADDS) THE FOUR PROJECTION STRINGS TOGETHER, REMOVING A TRAILING COMMA AT THE END OF THE LIST {?}
    concatenatedBinaryProj = binaryProjData0 + " " + binaryProjData45 + " " + binaryProjData90 + " " + binaryProjData135[
                                                                                                       :-1]  # REMOVES TRAILING COMMA
    # REMOVES THE COMMA AND SPACE SEPARATING EACH OF THE OTHER LIST ELEMENTS
    concatenatedBinaryProj = concatenatedBinaryProj.replace(", ", "")

    if runMode == "imageDIR":
        if(printResultsForEachImage == True):
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
    # converts query image binary string into binary
    qIBinary = bytes(queryImageBinary)

    # converts data set into binary
    dataBinary = []
    for binary in scannedImagesBinary:
        dataBinary.append(bytes(binary))

    # RIGHT NOW THIS CHECKS ONLY FOR AN EXACT MATCH !
    print("RIGHT NOW THIS CHECKS ONLY FOR AN EXACT MATCH !")
    counter = 0
    for binary in dataBinary:
        if binary == qIBinary:
            print(f"Exact match found with file: {scannedImages[counter]}")
        counter += 1
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
            providedFalseInput = input("Do you want to run the professors files: y/n")
            while providedFalseInput != "y" or providedFalseInput != "n":
                providedFalseInput = input("Do you want to run the professors files: y/n")

            print("Do you want to run a.. ")
            print("1) single image")
            print("2) or a directory of images")
            runModeInput = input("")
            while runModeInput != "1" or runModeInput != "2":
                print("Do you want to run a.. ")
                print("1) single image")
                print("2) or a directory of images")
                runModeInput = input("")

            print("Is it located in.. ")
            print("1) the same dir as main.py")
            print("2) or is it in images dir")
            targetLocInput = input("")
            while targetLocInput != "1" or targetLocInput != "2":
                print("Is it located in.. ")
                print("1) the same dir as main.py")
                print("2) or is it in images dir")
                targetLocInput = input("")
    else:
        print("Run mode was set incorrectly!")

startProgram(runMode, runModes)