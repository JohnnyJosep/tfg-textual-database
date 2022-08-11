import requests
import os

success_status_code = 200


def download(url, destination, name):
    path = f"{destination}/{name}.pdf"
    if os.path.exists(path):
        return True

    response = requests.get(url)
    if response.status_code != success_status_code:
        return False

    open(path, "wb").write(response.content)
    return True

