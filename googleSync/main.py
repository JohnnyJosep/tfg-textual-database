# https://code.luasoftware.com/tutorials/python/python-google-drive-api-to-sync-folder/
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
    '.txt': 'text/plain'
}


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
                parent = path[len(base_folder):-len(name)-1]
                if parent not in drive_files:
                    # TODO: make recursive folders creation
                    folder_metadata = {
                        'name': parent,
                        'mimeType': GDRIVE_FOLDER_MIMETYPE,
                        'parents': [root_id]
                    }
                    new_folder = drive_service.files().create(body=folder_metadata, fields='id').execute()
                    drive_files[parent] = new_folder.get('id')

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
        local_folder='.data', gdrive_folder_name='TFG-DATA', token_file=TOKEN_FILE, credential_file=CREDENTIAL_FILE):

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
        print(f"'{gdrive_folder_name} not found, create new")
        file = drive_service.files().create(body=file_metadata, fields='id').execute()
    else:
        file = items[0]

    folder_id = file.get('id')

    # check files on gdrive
    drive_filenames = _check_files(drive_service, folder_id)

    # only upload new files
    _upload_new_files(drive_service, drive_filenames, local_folder, local_folder + '/', folder_id)

    # download files
    _download_files(drive_service, drive_filenames, local_folder)


if __name__ == '__main__':
    sync_folder('../.data', token_file='../gdrive_sync_token.json', credential_file='../gdrive_sync_credential.json')