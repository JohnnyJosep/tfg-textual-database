import pytesseract
import pika
from time import sleep
from PIL import Image
from os import listdir, mkdir, environ
from os.path import isdir, exists

data_path = environ['DATA_PATH']


def convert_to_text(image_path):
    image = Image.open(image_path)
    print(f'\t\tconverting to text {image_path}')
    text = pytesseract.image_to_string(image, lang='spa')
    return text


def convert(images_folder_name):
    txt_path = f'{data_path}/texts/{images_folder_name}.txt'

    if exists(txt_path):
        return

    images = sorted(listdir(f'{data_path}/images/{images_folder_name}'))
    texts = [convert_to_text(f'{data_path}/images/{images_folder_name}/{img}') for img in images]
    text = '\n\n'.join(texts)

    with open(txt_path, 'w', encoding='utf-8') as txt_file:
        txt_file.write(text)

    print(f'\t{txt_path}')


def callback(ch, method, properties, body):
    images_folder_name = body.decode()
    print(images_folder_name)

    convert(images_folder_name)

    ch.basic_ack(delivery_tag=method.delivery_tag)


if __name__ == '__main__':
    if not isdir(f'{data_path}/texts'):
        mkdir(f'{data_path}/texts')

    sleep(20)

    host = 'my-rabbitmq'
    port = 5672
    user = 'myuser'
    password = 'mypassword'
    queue_name = 'img-to-txt'

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
