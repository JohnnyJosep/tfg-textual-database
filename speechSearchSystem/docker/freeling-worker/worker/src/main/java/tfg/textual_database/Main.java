package tfg.textual_database;

import com.google.gson.Gson;
import com.rabbitmq.client.Channel;
import com.rabbitmq.client.Connection;
import com.rabbitmq.client.ConnectionFactory;
import com.rabbitmq.client.DeliverCallback;

import java.io.IOException;
import java.io.OutputStreamWriter;
import java.net.HttpURLConnection;
import java.net.URL;
import java.nio.charset.StandardCharsets;

public class Main {

    private static final String TASK_QUEUE_NAME = "speeches";

    public static void main(String[] args) throws Exception {
        Thread.sleep(30 * 1000);

        ConnectionFactory factory = new ConnectionFactory();
        factory.setHost("api-rabbitmq");
        factory.setUsername("myuser");
        factory.setPassword("mypassword");

        Connection connection = factory.newConnection();
        Channel channel = connection.createChannel();
        channel.queueDeclare(TASK_QUEUE_NAME, true, false, false, null);
        channel.basicQos(1);

        System.out.println("Worker started.");
        DeliverCallback deliverCallback = (consumerTag, delivery) -> {
            String message = new String(delivery.getBody(), StandardCharsets.UTF_8);
            try {
                doWork(message);
            } finally {
                channel.basicAck(delivery.getEnvelope().getDeliveryTag(), false);
            }
        };
        channel.basicConsume(TASK_QUEUE_NAME, false, deliverCallback, consumerTag -> {
        });
    }

    private static void doWork(String task) throws IOException {
        System.out.println(task);
        SpeechMessage speechMessage = new Gson().fromJson(task, SpeechMessage.class);

        FreelingService freelingService = new FreelingService();
        String response = freelingService.processSegment(speechMessage.getText());
        freelingService.close();
        System.out.println(response);

        String json = new Gson().toJson(new PutSpeechMorphologicalAnalysis(speechMessage.getId(), response));

        URL url = new URL("http://speech-search-system/api/speech/" + speechMessage.getId());
        HttpURLConnection connection = (HttpURLConnection) url.openConnection();
        connection.setDoOutput(true);
        connection.setRequestMethod("PUT");
        connection.setRequestProperty("Content-Type", "application/json");
        connection.setRequestProperty("Accept", "application/json");
        OutputStreamWriter osw = new OutputStreamWriter(connection.getOutputStream());
        osw.write(json);
        osw.flush();
        osw.close();
    }
}