package tfg.textual_database;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.net.Socket;
import java.nio.charset.StandardCharsets;

public class FreelingService {
    private static final String FREELING_HOST = "freeling";
    private static final int FREELING_PORT = 50005;

    private static final String MESSAGE_SERVER_READY = "FL-SERVER-READY";
    private static final String MESSAGE_RESET_STATS = "RESET_STATS";
    private static final String MESSAGE_FLUSH_BUFFER = "FLUSH_BUFFER";
    private static final String ENCODING = "UTF8";
    private final static int BUF_SIZE = 2048;

    Socket socket;
    DataInputStream dataInputStream;
    DataOutputStream dataOutputStream;

    public FreelingService() {
        try {
            socket = new Socket(FREELING_HOST, FREELING_PORT);
            socket.setSoLinger(true, 10);
            socket.setKeepAlive(true);
            socket.setSoTimeout(10000);
            dataInputStream = new DataInputStream(socket.getInputStream());
            dataOutputStream = new DataOutputStream(socket.getOutputStream());

            writeMessage(MESSAGE_RESET_STATS);

            StringBuffer sb = readMessage();
            if (sb.toString().replaceAll("\0", "").compareTo(MESSAGE_SERVER_READY) != 0)
                System.err.println("SERVER NOT READY!");
            writeMessage(MESSAGE_FLUSH_BUFFER);
            readMessage();

        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public String processSegment(String text) throws IOException {
        writeMessage(text);
        StringBuffer sb = readMessage();
        writeMessage(MESSAGE_FLUSH_BUFFER);
        readMessage();
        return sb.toString();
    }

    private void writeMessage(String message) throws IOException {
        dataOutputStream.write(message.getBytes(ENCODING));
        dataOutputStream.write(0);
        dataOutputStream.flush();
    }

    public void close() throws IOException {
        socket.close();
    }

    private synchronized StringBuffer readMessage() throws IOException {

        byte[] buffer = new byte[BUF_SIZE];
        int bl = 0;
        StringBuffer sb = new StringBuffer();

        do {
            bl = dataInputStream.read(buffer, 0, BUF_SIZE);
            if (bl > 0) sb.append(new String(buffer, 0, bl));
        } while (bl > 0 && buffer[bl - 1] != 0);

        return sb;
    }

}
