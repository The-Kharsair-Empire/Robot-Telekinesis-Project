import socket
import time

def parseSentMsg(pose):
    return "({}, {}, {}, {}, {}, {})".format(pose[0], pose[1], pose[2], pose[3], pose[4], pose[5])

def main():
    HOST = "192.168.0.192"  # The remote host
    PORT = 21  # The same port as used by the server

    count = 0
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    s.bind((HOST, PORT))  # Bind to the port
    s.listen(5)  # Now wait for client connection.

    startPos = [0.339, 0.053, 0.344, 1.901, -2.635, 2.397]
    while True:
        c, addr = s.accept()
        helloMsgFromRobot = c.recv(1024)
        print(helloMsgFromRobot.decode())
        while True:
            c.send(parseSentMsg(startPos).encode())
            startPos[0] += 0.01
            # robot_pose = c.recv(1024)
            # print(robot_pose)
            time.sleep(0.5)
            input()

        c.close()

    s.close()

if __name__ == '__main__':
    main()
