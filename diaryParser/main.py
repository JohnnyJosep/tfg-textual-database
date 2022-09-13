import re

MaleTreatment = 'El señor '
FemaleTreatment = 'La señora '

PresidencyPattern = 'PRESIDENCIA ((DEL EXCMO\. SR\. D\.)|(DE LA EXCMA\. SRA\. D\.\ª)) [A-Za-zÀàÄäÁáÈèËëÉéÌìÏïÍíÒòÖöÓóÙùÜüÚúÑñ\·\- ]+'
WeekDayPattern = '(lunes|martes|miércoles|jueves|viernes|sábado|domingo)'
MonthPattern = '(enero|febrero|mayo|abril|marzo|junio|julio|agosto|septiembre|setiembre|octubre|noviembre|diciembre)'
DatePattern = r'\d{1,2} de ' + MonthPattern + r' de \d{4}'
CelebratedPattern = 'celebrada el ' + WeekDayPattern + ' ' + DatePattern

OpenSessionPattern = r'Se (abre|reanuda) la sesión a (.+) (de la mañana|del mediodía|de la tarde|de la noche|horas)( y (.+) minutos)?\.\n'
EndSessionPattern = r'(Eran las|Era la) (.+?) (de la mañana|del mediodía|de la tarde|de la noche)\.\s*\n'

SpeakerTreatmentPattern = r'^(' + MaleTreatment + '|' + FemaleTreatment + ')'
SpeakerNamePattern = r'[A-ZÀÄÁÈËÉÌÏÍÒÖÓÙÜÚÑ\·\- ,]+'
SpeakerTitlePattern = r'( \([A-Za-zÀàÄäÁáÈèËëÉéÌìÏïÍíÒòÖöÓóÙùÜüÚúÑñ\·\- ]*?\))?'
SpeakerPattern = SpeakerTreatmentPattern + SpeakerNamePattern + SpeakerTitlePattern + '\:'

InterruptionPattern = r' \((.+?)\)'


def _parse_presidency(text):
    search = re.search(PresidencyPattern, text)
    if search:
        found = search.group()
        if found.startswith('PRESIDENCIA DEL EXCMO. SR. D. '):
            return found[30:]
        else:
            return found[34:]
    return None


def _parse_date(text):
    search = re.search(CelebratedPattern, text)
    if search:
        found_split = search.group()[13:].split()
        return f'{found_split[5]}-{_parse_month(found_split[3]):02d}-{int(found_split[1]):02d}'
    return None


def _parse_month(month):
    switcher = {
        'enero': 1,
        'febrero': 2,
        'mayo': 3,
        'abril': 4,
        'marzo': 5,
        'junio': 6,
        'julio': 7,
        'agosto': 8,
        'septiembre': 9,
        'setiembre': 9,
        'octubre': 10,
        'noviembre': 11,
        'diciembre': 12
    }

    return switcher.get(month, 0)


def _is_title(paragraph):
    title = re.sub(r'\(.*\)', '', paragraph)
    return title.isupper()


def _parse_speech_text(text):
    interruptions_positions = [(m.start(0), m.end(0)) for m in re.finditer(InterruptionPattern, text)]
    if len(interruptions_positions) == 0:
        return {'text': text}

    interruptions = []
    start = 0
    for interruptions_position in interruptions_positions:
        init = interruptions_position[0] - start
        end = interruptions_position[1] - start
        interruption = {'text': text[init + 2: end - 1], 'position': init}

        text = text[:init] + text[end:]
        start += (end - init)

        interruptions.append(interruption)

    return {'text': text, 'interruptions': interruptions}


def _parse_speeches(title, text):
    speeches = []
    paragraphs = text.split('\n')

    speech_text = ''
    speech = {'title': title, 'order': 0}
    speech_order = 1

    for paragraph in paragraphs:
        search = re.search(SpeakerPattern, paragraph)
        if search:
            if speech_text != '':
                speech |= _parse_speech_text(speech_text)
                speeches.append(speech)
                speech = {'title': title, 'order': speech_order}
                speech_order += 1

            found = search.group()
            name_search = re.search(SpeakerTreatmentPattern + SpeakerNamePattern, found)
            name_found = name_search.group()
            is_male = found.startswith(MaleTreatment)
            speech['gender'] = 'male' if is_male else 'female'
            speech['name'] = name_found[len(MaleTreatment):] if is_male else name_found[len(FemaleTreatment):]

            title_treatment = found[len(name_found):-1]
            if title_treatment != '':
                speech['explanatory'] = title_treatment.strip('()')

            speech_text = paragraph[len(found):]
        else:
            speech_text += '\n' + paragraph

    speech |= _parse_speech_text(speech_text)
    speeches.append(speech)
    return speeches


def _parse_session(text):
    it = [m.end(0) for m in re.finditer(OpenSessionPattern, text)]
    session_text = text[it[-1]:]
    session_text = re.sub(EndSessionPattern, '', session_text)

    lines = session_text.split('\n')
    paragraphs = []
    paragraph = ''
    for line in lines:
        if line == '' and paragraph != '':
            paragraphs.append(paragraph)
            paragraph = ''
        elif paragraph != '':
            paragraph += ' ' + line
        else:
            paragraph = line

    speech_title = ''
    speech_text = ''
    speeches = []
    for paragraph in paragraphs:
        if _is_title(paragraph):
            if speech_title != '':
                speeches += _parse_speeches(speech_title, speech_text)
            speech_title = paragraph
            speech_text = ''
        else:
            speech_text += paragraph if speech_text == '' else '\n' + paragraph

    speeches += _parse_speeches(speech_title, speech_text)
    return speeches


def parse_diary(text, source, legislature, session):
    text = text.replace('\f', '')

    data = {
        'presidency': _parse_presidency(text),
        'date': _parse_date(text),
        'source': source,
        'legislature': legislature,
        'session': session
    }

    speeches = _parse_session(text)
    for speech in speeches:
        speech |= data

    return speeches


if __name__ == '__main__':

    with open('../.data/texts/dss-12-005.txt', 'r', encoding="utf-8") as file:
        diary = file.read()
        print(parse_diary(diary, 'dss', 11, 2))

