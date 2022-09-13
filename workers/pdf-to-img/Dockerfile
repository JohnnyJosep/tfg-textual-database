FROM python:3.10.6-alpine

WORKDIR /usr/src/app

RUN apk update
RUN /usr/local/bin/python -m pip install --upgrade pip
RUN apk add --update jpeg-dev zlib-dev libjpeg gcc musl-dev poppler poppler-utils

COPY requirements.txt ./
RUN pip install --no-cache-dir -r requirements.txt

COPY . .
CMD [ "python", "-u", "main.py" ]