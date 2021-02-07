import java.io.*;
import java.net.ServerSocket;
import java.net.Socket;

public class TestServer {
    public static int server_port = 21;


    public static ServerSocket server;
    public static Socket client;
    public static InputStream in ;
    public static OutputStream out ;

    public static void main(String[] args) {
        try {
            startServer(server_port);
        } catch (IOException e) {
            e.printStackTrace();
        }

    }

    private static void startServer(int server_port) throws IOException {
        server = new ServerSocket(server_port);

        while(true){
            client = server.accept();
            System.out.println("accepted");
            System.out.println(client);
            System.out.println(client.getRemoteSocketAddress());
            System.out.println(client.getLocalAddress());
            System.out.println(client.getRemoteSocketAddress());
            System.out.println(client.getRemoteSocketAddress());

/*        in = client.getInputStream();
        out = client.getOutputStream();*/

            BufferedReader inChannel = new BufferedReader(new InputStreamReader(client.getInputStream(), "UTF-8"));
            BufferedWriter outChannel = new BufferedWriter(new OutputStreamWriter(client.getOutputStream(), "UTF-8"));

            String msgFromClient;
            while ((msgFromClient = inChannel.readLine()) != null){

                System.out.println(msgFromClient);
                outChannel.write("fuck you back client!\n");
                outChannel.flush();

            }

        }




       // server.close();
    }

/*    private String recvAndSend(String response) throws IOException{

        byte[] len = new byte[4];
        in.read(len, 0, 4);
        int msg_len  = (((len[3] & 0xff) << 24) | ((len[2] & 0xff) << 16) |
                ((len[1] & 0xff) << 8) | (len[0] & 0xff));

        byte[] data = new byte[msg_len];
        in.read(data, 0, msg_len);

        String req = new String(data, 0, msg)

    }*/

}
