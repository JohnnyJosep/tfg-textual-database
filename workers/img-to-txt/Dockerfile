FROM python:3.10.2-alpine

WORKDIR /usr/src/app

RUN apk update
RUN /usr/local/bin/python -m pip install --upgrade pip
RUN apk add --update tesseract-ocr tesseract-ocr-data-cat tesseract-ocr-data-spa jpeg-dev

COPY requirements.txt ./
RUN pip install --no-cache-dir -r requirements.txt

COPY main.py ./
CMD [ "python", "-u", "main.py" ]
