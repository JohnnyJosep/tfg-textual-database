import pika
from time import sleep
from os.path import basename, isdir
from os import listdir, makedirs, environ
from pdf2image import convert_from_path

left_margin = 175
top_header_margin = 790
top_regular_margin = 296
right_margin = 1480
bottom_margin = 2160

data_path = environ['DATA_PATH']


def convert(filename):
    pdf_path = f'{data_path}/{filename}'
    img_folder = f'{data_path}/images/{basename(filename)[:-4]}'
    if isdir(img_folder):
        return

    makedirs(img_folder)
    pages = convert_from_path(pdf_path)
    for idx, page in enumerate(pages):
        top_margin = top_header_margin if idx == 0 else top_regular_margin
        box = (left_margin, top_margin, right_margin, bottom_margin)
        img_path = f'{img_folder}/{basename(pdf_path)[:-4]}-{idx:03d}.jpeg'

        page.crop(box).save(img_path, 'JPEG')


def callback(ch, method, properties, body):
    filename = body.decode()
    print(filename)

    convert(filename)

    ch.basic_ack(delivery_tag=method.delivery_tag)


if __name__ == '__main__':
    sleep(20)

    host = 'my-rabbitmq'
    port = 5672
    user = 'myuser'
    password = 'mypassword'
    queue_name = 'pdf-to-img'

    print(f'connecting to {user}:{password}@{host}:{port} -> {queue_name}')

    connection = pika.BlockingConnection(pika.ConnectionParameters(
        host=host,
        port=port,
        credentials=pika.PlainCredentials(user, password)
    ))
    channel = connection.channel()
    channel.queue_declare(queue=queue_name, durable=True)
    channel.basic_qos(prefetch_count=1)
    channel.basic_consume(queue=queue_name, on_message_callback=callback)

    channel.start_consuming()
