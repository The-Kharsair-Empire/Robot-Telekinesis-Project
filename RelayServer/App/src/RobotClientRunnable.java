import java.io.*;
import java.net.ServerSocket;
import java.net.Socket;
import java.nio.charset.StandardCharsets;

public class RobotClientRunnable implements Runnable{
    private BufferedReader inChannel;
    private BufferedWriter outChannel;
    private ServerSocket robot;
    private Socket robotClient;
    private BlockingQueue posCmdQueue;
    private BlockingQueue jointStateQueue;

    public RobotClientRunnable(ServerSocket robotClientSocket, BlockingQueue posCmdQueue, BlockingQueue jointStateQueue) {
        robot = robotClientSocket;
        this.posCmdQueue = posCmdQueue;
        this.jointStateQueue = jointStateQueue;



    }

    @Override
    public void run() {

        try {
            while (true) {
                robotClient = robot.accept();
                System.out.println("robot client accepted" + robotClient);
                inChannel = new BufferedReader(new InputStreamReader(robotClient.getInputStream(), StandardCharsets.UTF_8));
                outChannel = new BufferedWriter(new OutputStreamWriter(robotClient.getOutputStream(), StandardCharsets.UTF_8));

                String jointStateFromRobot;
                while (true){
                    String pos = posCmdQueue.take();
                    outChannel.write(pos);
                    outChannel.flush();
                    System.out.println("sending pos to robot: " +  pos);

                    if ((jointStateFromRobot = inChannel.readLine()) != null) {
                        System.out.println("pos recived from unity: " + jointStateFromRobot);
                        jointStateQueue.add(jointStateFromRobot);

                    } else {
                        System.out.println("Robot lost connection!! ");
                        break;

                    }

                }
                robotClient.close();
            }

        } catch (IOException ioe) {
            ioe.printStackTrace();
        } catch (InterruptedException itrpte){
            System.err.println(itrpte.getMessage());
        }

        try {
            robotClient.close();
            robot.close();
        } catch (IOException e) {
            e.printStackTrace();
        }

    }
}
