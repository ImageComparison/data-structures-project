import numpy as np

x = 4
data = np.arange(16).reshape(x, x)
print(data, "\n")

def project_45deg(data):
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

def project_135deg(data):
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

print(f"45 deg: {project_45deg(data)}")
print(f"135 deg: {project_135deg(data)}")