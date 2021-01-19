import struct
import sys

startIndex = sys.argv[1]
takeLength = sys.argv[2]
bytesArray = sys.argv[3]

def BigEndian2LittleEndian(data, startIndex, takeLength):
    slice = [0] * takeLength
    for i in range(takeLength):
        slice[i] = data[startIndex + i]
    slice = bytes(slice)

    lEndian = [0] * takeLength
    for i in range(0, takeLength, 8):
        for j in range(7, 0 - 1, -1):
            lEndian[i + (7 - j)] = slice[i + j]

    lEndian = bytes(lEndian)

    values = []

    for i in range(take // 8):
        values.append(struct.unpack('d', lEndian[i * 8:i * 8 + 8])[0])

    return values

Six_Tuple = BigEndian2LittleEndian(bytesArray, startIndex, takeLength)
print(Six_Tuple[0], Six_Tuple[1], Six_Tuple[2], Six_Tuple[3], Six_Tuple[4], Six_Tuple[5])