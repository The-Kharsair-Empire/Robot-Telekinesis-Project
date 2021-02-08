import java.io.*;
import java.net.ServerSocket;
import java.util.concurrent.LinkedBlockingDeque;

public class Server {
    private final static int robot_port = 21;
    private final static int unity_port = 27;

    public static void main(String[] args) throws IOException, InterruptedException {
        Server relayServer = new Server(robot_port, unity_port);
    }

    public Server(int robot_port, int unity_port) throws IOException, InterruptedException {
        ServerSocket robotServerSocket = new ServerSocket(robot_port);

        ServerSocket unityServerSocket = new ServerSocket(unity_port);
        System.out.println("Server start at port: " + robot_port);
        System.out.println("Server start at port: " + unity_port);

        BlockingQueue posCmdQueue = new BlockingQueue();
        BlockingQueue jointStateQueue = new BlockingQueue();

        Thread robotThread = new Thread(new RobotClientRunnable(robotServerSocket, posCmdQueue, jointStateQueue));

        Thread unityThread = new Thread(new UnityClientRunnable(unityServerSocket, posCmdQueue, jointStateQueue));

        robotThread.start();
        unityThread.start();

        robotThread.join();
        unityThread.join();


        System.out.println("both thread finish executing");
    }


}
