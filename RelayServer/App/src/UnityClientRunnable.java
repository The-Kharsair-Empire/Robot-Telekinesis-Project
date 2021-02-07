import java.io.*;
import java.net.ServerSocket;
import java.net.Socket;
import java.nio.charset.StandardCharsets;

public class UnityClientRunnable implements Runnable{
    private BufferedReader inChannel;
    private BufferedWriter outChannel;
    private ServerSocket unity;
    private Socket unityClient;
    private BlockingQueue posCmdQueue;
    private BlockingQueue jointStateQueue;

    public UnityClientRunnable(ServerSocket unityServerSocket, BlockingQueue posCmdQueue, BlockingQueue jointStateQueue) {
        unity = unityServerSocket;
        this.posCmdQueue = posCmdQueue;
        this.jointStateQueue = jointStateQueue;

    }

    @Override
    public void run() {

        try {
            while (true) {
                unityClient = unity.accept();
                System.out.println("unity client accepted" + unityClient);
                inChannel = new BufferedReader(new InputStreamReader(unityClient.getInputStream(), StandardCharsets.UTF_8));
                outChannel = new BufferedWriter(new OutputStreamWriter(unityClient.getOutputStream(), StandardCharsets.UTF_8));

                String posFromUnity;
                
                while ((posFromUnity = inChannel.readLine()) != null){

                    System.out.println("pos recived from unity: " + posFromUnity);
                    posCmdQueue.add(posFromUnity);

                    outChannel.write(jointStateQueue.take());
                    outChannel.flush();

                }
                unityClient.close();
            }

        } catch (IOException ioe) {
            ioe.printStackTrace();
        }catch (InterruptedException itrpte){
            System.err.println(itrpte.getMessage());
        }

        try {
            unityClient.close();
            unity.close();
        } catch (IOException e) {
            e.printStackTrace();
        }




    }
}
