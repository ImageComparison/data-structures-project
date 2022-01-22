import cffi

# IGNORE THIS DEF LOAD(SELF) I STOLE IT FROM PIL SOURCE CODE
# TRYING TO UNDERSTAND HOW WE COULD DO IT FROM SCRATCH ALTHOUGH
# IT LOOKS LIKE THEY DO IT IN C NOT PYTHON AND USE CFFI
def load(self):
    if self.im and self.palette and self.palette.dirty:
        mode, arr = self.palette.getdata()
        if mode == "RGBA":
            mode = "RGB"
            self.info["transparency"] = arr[3::4]
            arr = bytes(
                value for (index, value) in enumerate(arr) if index % 4 != 3
            )
        palette_length = self.im.putpalette(mode, arr)
        self.palette.dirty = 0
        self.palette.rawmode = None
        if "transparency" in self.info and mode in ("LA", "PA"):
            if isinstance(self.info["transparency"], int):
                self.im.putpalettealpha(self.info["transparency"], 0)
            else:
                self.im.putpalettealphas(self.info["transparency"])
            self.palette.mode = "RGBA"
        else:
            self.palette.mode = "RGB"
            self.palette.palette = self.im.getpalette()[: palette_length * 3]

    if self.im:
        if cffi and USE_CFFI_ACCESS:
            if self.pyaccess:
                return self.pyaccess
            from . import PyAccess

            self.pyaccess = PyAccess.new(self, self.readonly)
            if self.pyaccess:
                return self.pyaccess
        return self.im.pixel_access(self.readonly)

# SETTINGS
filename = 'img_10007.jpg'
printRawImageArray = False
printImageRayArrays = True
printThresholdValues = True
printBinaryArrays = False

import PIL
from PIL import Image
print(f"Analysing {filename}")
img = PIL.Image.open(filename)
# img = PIL.Image.open('img_10019.jpg')
pix = img.load()
counter = 1
data = []
for y in range(28):
    rowData = []
    for x in range(28):
        p = pix[x,y]
        # if (p != 0):
        #     print(f"[{counter}/784]",p)
        rowData.append(p)
        counter += 1
    data.append(rowData)

if (printRawImageArray == True):
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

projectionData0 = project(0)
projectionData45 = project(45)
projectionData90 = project(90)
projectionData135 = project(135)
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


Th_P1 = findProjThreshold(projectionData0)
Th_P2 = findProjThreshold(projectionData45)
Th_P3 = findProjThreshold(projectionData90)
Th_P4 = findProjThreshold(projectionData135)
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


binaryProjData0 = convertProjToBinary(projectionData0, Th_P1)
binaryProjData45 = convertProjToBinary(projectionData45, Th_P2)
binaryProjData90 = convertProjToBinary(projectionData90, Th_P3)
binaryProjData135 = convertProjToBinary(projectionData135, Th_P4)
if (printBinaryArrays == True):
    print("RAY DATA:")
    print("<0 DEG BINARY>", binaryProjData0)
    print("<45 DEG BINARY>", binaryProjData45)
    print("<90 DEG BINARY>", binaryProjData90)
    print("<135 DEG BINARY>", binaryProjData135)
    print("")

binaryProjData0 = str(binaryProjData0)[1:len(binaryProjData0)-1]
binaryProjData45 = str(binaryProjData45)[1:len(binaryProjData45)-1]
binaryProjData90 = str(binaryProjData90)[1:len(binaryProjData90)-1]
binaryProjData135 = str(binaryProjData135)[1:len(binaryProjData135)-1]
concatenatedBinaryProj = binaryProjData0 + " " + binaryProjData45 + " " + binaryProjData90 + " " + binaryProjData135
concatenatedBinaryProj = concatenatedBinaryProj.replace(", ", " ")  # replaces separating python list commas with spaces
concatenatedBinaryProj = concatenatedBinaryProj[:-1]  # removes trailing comma
print(f"concatenatedBinaryProj: {concatenatedBinaryProj}")
print("Done!")
