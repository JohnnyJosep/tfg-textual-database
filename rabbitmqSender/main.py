import pika


class RabbitSender:
    def __init__(self, rabbit_host, rabbit_port, rabbit_user, rabbit_pass, queue_name):
        self.queue_name = queue_name
        credentials = pika.PlainCredentials(rabbit_user, rabbit_pass)
        self.connection = pika.BlockingConnection(pika.ConnectionParameters(
            host=rabbit_host,
            port=rabbit_port,
            credentials=credentials
        ))
        self.channel = self.connection.channel()

        self.channel.queue_declare(queue=queue_name, durable=True)

    def send(self, body):
        self.channel.basic_publish(
            exchange='',
            routing_key=self.queue_name,
            body=body
        )

    def close(self):
        self.connection.close()


