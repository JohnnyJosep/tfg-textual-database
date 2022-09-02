from os.path import basename, isdir
from os import listdir, makedirs, environ
from pdf2image import convert_from_path

left_margin = 175
top_header_margin = 790
top_regular_margin = 296
right_margin = 1480
bottom_margin = 2160

pdf_path = environ['PDF_PATH']
image_path = environ['IMG_PATH']


if __name__ == '__main__':
    print(f'pdfs {pdf_path} to jpegs {image_path}')

    pdfs = [f'{pdf_path}/{file}' for file in listdir(pdf_path)]

    for pdf in pdfs:
        folder = f'{image_path}/{basename(pdf)[:-4]}'
        if isdir(folder):
            continue

        makedirs(folder)

        pages = convert_from_path(pdf)
        for idx, page in enumerate(pages):
            top_margin = top_header_margin if idx == 0 else top_regular_margin
            box = (left_margin, top_margin, right_margin, bottom_margin)
            img_path = f'{folder}/{basename(pdf)[:-4]}-{idx:03d}.jpeg'

            page.crop(box).save(img_path, 'JPEG')
