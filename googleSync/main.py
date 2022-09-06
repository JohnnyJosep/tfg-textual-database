
import shutil
from os import listdir, makedirs
from os.path import isdir, splitext, basename, exists
import io
from googleapiclient.discovery import build
from googleapiclient.http import MediaFileUpload, MediaIoBaseDownload
from httplib2 import Http
from oauth2client import file as oauth2file, client, tools

SCOPES = 'https://www.googleapis.com/auth/drive'
CREDENTIAL_FILE = 'gdrive_sync_credential.json'
TOKEN_FILE = 'gdrive_sync_token.json'

GDRIVE_FOLDER_MIMETYPE = 'application/vnd.google-apps.folder'
MIMETYPES = {
    '.pdf': 'application/pdf',
    '.txt': 'text/plain',
    '.jpeg': 'image/jpeg',
    '.pkl': 'application/octet-stream'
}


def _gdrive_folder_content(drive_service, folder_id, parent_folder='', page_token=None):
    response = drive_service.files().list(q=f"'{folder_id}' in parents", pageToken=page_token).execute()
    files = {}
    folders = {}

    for file in response.get('files', []):
        file_id = file.get('id')
        file_name = f"{parent_folder}{file.get('name')}"
        if file['mimeType'] == GDRIVE_FOLDER_MIMETYPE:
            folders[file_name] = file_id
        else:
            files[file_name] = file_id

    next_page_token = response.get('nextPageToken')
    if next_page_token is not None:
        next_files, next_folders = _gdrive_folder_content(drive_service, folder_id, parent_folder, next_page_token)
        files |= next_files
        folders |= next_folders

    return files, folders


def _gdrive_download(drive_service, file_id, path):
    name = basename(path)
    folder = path[:-len(name)-1]
    if not exists(folder):
        makedirs(folder)

    request = drive_service.files().get_media(fileId=file_id)
    fh = io.BytesIO()
    downloader = MediaIoBaseDownload(fh, request, chunksize=204800)
    done = False
    while not done:
        status, done = downloader.next_chunk()
    fh.seek(0)
    with open(path, 'wb') as file:
        shutil.copyfileobj(fh, file)

    print('Downloaded', path)


def _check_files(drive_service, folder_id, parent_folder='', page_token=None):
    response = drive_service.files().list(q=f"'{folder_id}' in parents", pageToken=page_token).execute()
    drive_filenames = {}
    for _file in response.get('files', []):
        file_id = _file.get('id')
        file_name = f"{parent_folder}{_file.get('name')}"
        drive_filenames[file_name] = _file.get('id')
        if _file['mimeType'] == GDRIVE_FOLDER_MIMETYPE:
            folder_drive_filenames = _check_files(drive_service, file_id, parent_folder=file_name+"/")
            drive_filenames |= folder_drive_filenames

    next_page_token = response.get('nextPageToken')
    if next_page_token is not None:
        next_page_drive_filenames = _check_files(drive_service, folder_id, parent_folder, next_page_token)
        drive_filenames |= next_page_drive_filenames

    return drive_filenames


def _upload_new_files(drive_service, drive_files, folder, base_folder, root_id):
    files = listdir(folder)
    for file in files:
        path = f'{folder}/{file}'
        if isdir(path):
            _upload_new_files(drive_service, drive_files, path, base_folder, root_id)
        elif path[len(base_folder):] not in drive_files:
            file_extension = splitext(path)[1]
            name = basename(path)
            if file_extension in MIMETYPES:
                print(f'Upload: {path}')
                mimetype = MIMETYPES[file_extension]
                parent_folders = path[len(base_folder):-len(name)-1].split('/')
                parent = ''
                parent_id = root_id
                for parent_folder in parent_folders:
                    if parent == '':
                        parent = parent_folder
                    else:
                        parent += f"/{parent_folder}"

                    if parent not in drive_files:
                        folder_metadata = {
                            'name': parent_folder,
                            'mimeType': GDRIVE_FOLDER_MIMETYPE,
                            'parents': [parent_id]
                        }
                        new_folder = drive_service.files().create(body=folder_metadata, fields='id').execute()
                        drive_files[parent] = new_folder.get('id')

                    parent_id = drive_files[parent]

                file_metadata = {
                    'name': name,
                    'parents': [drive_files[parent]],
                }
                media = MediaFileUpload(path, mimetype=mimetype)
                file = drive_service.files().create(body=file_metadata, media_body=media, fields='id').execute()
                print(f"Uploaded: {file.get('id')}")


def _download_files(drive_service, drive_files, base_folder):
    for drive_file in drive_files:
        path = f"{base_folder}/{drive_file}"

        # check if extension is in accepted mimetypes
        file_extension = splitext(path)[1]
        if file_extension not in MIMETYPES:
            continue

        # create folders if not exists
        name = basename(path)
        folder = path[:-len(name)-1]
        if not exists(folder):
            makedirs(folder)

        # download file if not exits
        if not exists(path):
            request = drive_service.files().get_media(fileId=drive_files[drive_file])

            fh = io.BytesIO()
            downloader = MediaIoBaseDownload(fh, request, chunksize=204800)
            done = False
            while not done:
                status, done = downloader.next_chunk()
            fh.seek(0)
            with open(path, 'wb') as file:
                shutil.copyfileobj(fh, file)

            print(f"Download: {path}")


def sync_folder(
        local_folder='.data',
        gdrive_folder_name='TFG-DATA',
        token_file=TOKEN_FILE,
        credential_file=CREDENTIAL_FILE):
    """
    Sync local_folder with gdrive_folder
    :param local_folder: local folder path
    :param gdrive_folder_name: gdrive folder
    :param token_file: token file
    :param credential_file: credential file
    """

    store = oauth2file.Storage(token_file)
    creds = store.get()
    if not creds or creds.invalid:
        flow = client.flow_from_clientsecrets(credential_file, SCOPES)
        creds = tools.run_flow(flow, store)

    service = build('drive', 'v3', http=creds.authorize(Http()))
    drive_service = service

    file_metadata = {
        'name': gdrive_folder_name,
        'mimeType': GDRIVE_FOLDER_MIMETYPE
    }

    response = drive_service.files().list(q=f"name='{gdrive_folder_name}'").execute()

    items = response.get('files', [])
    if not items:
        print(f"'{gdrive_folder_name}' not found, create new")
        file = drive_service.files().create(body=file_metadata, fields='id').execute()
    else:
        file = items[0]

    # synchronize folders
    current_folder = [{'local_folder_path': local_folder, 'gdrive_folder_id': file.get('id')}]

    while len(current_folder) > 0:
        folder = current_folder.pop()
        local_folder_path = folder['local_folder_path']
        gdrive_folder_id = folder['gdrive_folder_id']

        gdrive_files, gdrive_folders = _gdrive_folder_content(drive_service, gdrive_folder_id)

        # download none existing local files
        for gdrive_file in gdrive_files:
            local_file_path = f"{local_folder_path}/{gdrive_file}"
            if not exists(local_file_path):
                _gdrive_download(drive_service, gdrive_files[gdrive_file], local_file_path)

        # upload none existing gdrive files

        # append gdrive folders to scan
        for gdrive_folder in gdrive_folders:
            new_gdrive_folder_id = gdrive_folders[gdrive_folder]
            new_local_folder_path = f"{local_folder_path}/{gdrive_folder}"
            current_folder.append({'local_folder_path': new_local_folder_path, 'gdrive_folder_id': new_gdrive_folder_id})


if __name__ == '__main__':
    sync_folder('../.data', token_file='../gdrive_sync_token.json', credential_file='../gdrive_sync_credential.json')