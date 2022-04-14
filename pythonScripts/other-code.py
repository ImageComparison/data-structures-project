import numpy as np
import skimage.transform as ski

def create_code(image):
    #Radon method
    code = np.array([])
    for i in range(0,180,15): # Specifies the start, stop, step for projection degrees
        projection = ski.radon(image,[i],circle=True,preserve_range=True)
        threshold = np.mean(projection)
        for i in range(0,projection.size,1):
            if (projection[i]>=threshold):
                projection[i] = 1
            else:
                projection[i] = 0

        code = np.append(code,projection)

    # converts code from array to string
    codeString = ''
    for i in range(0,code.size,1):
        codeString += str(int(code[i]))

    return