import java.io.*;
import java.net.ServerSocket;
import java.net.Socket;
import java.nio.charset.StandardCharsets;
import java.util.Arrays;

public class RobotClientRunnable implements Runnable{
    private BufferedReader inChannel;
    private BufferedWriter outChannel;
    private InputStream is;
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
                is = robotClient.getInputStream();

                int numberRead;
                String jointStateFromRobot;
                byte[] data;
                while (true){
                    data = new byte[1024];
                    String pos = posCmdQueue.take();
                    outChannel.write(pos);
                    outChannel.flush();
                    System.out.println("sending pos to robot: " +  pos);


                    if ((numberRead = is.read(data, 0, data.length)) > 0) {
                        jointStateFromRobot = new String(data, 0, numberRead);
//                        int i = Arrays.toString(jointStateFromRobot).indexOf('\\');
//                        String remnant = Arrays.toString(jointStateFromRobot).substring(0, i);
//                        System.out.println(remnant);
                        System.out.println("joint state recived from robot: " + jointStateFromRobot);
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
