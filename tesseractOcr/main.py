import pytesseract
from PIL import Image
from os import listdir, makedirs
from os.path import isfile, dirname, isdir

images_path = '/home/images'
texts_path = '/home/texts'


def files(path):
    result = []
    paths = [path]
    while len(paths) > 0:
        p = paths.pop()
        items = listdir(p)
        for item in items:
            item_path = f'{p}/{item}'
            if isfile(item_path):
                result.append(item_path)
            else:
                paths.append(item_path)
    return result


def convert_to_text(image_path):
    image = Image.open(image_path)
    text = pytesseract.image_to_string(image, lang='spa')
    return text


if __name__ == '__main__':
    images_to_convert = files(images_path)
    for image_to_convert in images_to_convert:
        text_path = image_to_convert.replace(images_path, texts_path).replace('.jpeg', '.txt')
        if isfile(text_path):
            continue

        print(f'Convert {image_to_convert} to {text_path}')

        image_text = convert_to_text(image_to_convert)

        text_path_dir = dirname(text_path)
        if not isdir(text_path_dir):
            makedirs(text_path_dir)

        open(text_path, 'w').write(image_text)
