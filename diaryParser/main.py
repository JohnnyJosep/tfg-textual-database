import re

MaleTreatment = 'El señor '
FemaleTreatment = 'La señora '

PresidencyPattern = 'PRESIDENCIA ((DEL EXCMO\. SR\. D\.)|(DE LA EXCMA\. SRA\. D\.\ª)) [A-Za-zÀàÄäÁáÈèËëÉéÌìÏïÍíÒòÖöÓóÙùÜüÚúÑñ\·\- ]+'
WeekDayPattern = '(lunes|martes|miércoles|jueves|viernes|sábado|domingo)'
MonthPattern = '(enero|febrero|mayo|abril|marzo|junio|julio|agosto|septiembre|setiembre|octubre|noviembre|diciembre)'
DatePattern = r'\d{1,2} de ' + MonthPattern + r' de \d{4}'
CelebratedPattern = 'celebrada el ' + WeekDayPattern + ' ' + DatePattern

OpenSessionPattern = r'Se (abre|reanuda) la sesión a (.+) (de la mañana|del mediodía|de la tarde|de la noche)\.\n'
EndSessionPattern = r'(Eran las|Era la) (.+?) (de la mañana|del mediodía|de la tarde|de la noche)\.\s*\n'

SpeakerTreatmentPattern = r'^(' + MaleTreatment + '|' + FemaleTreatment + ')'
SpeakerNamePattern = r'[A-ZÀÄÁÈËÉÌÏÍÒÖÓÙÜÚÑ\·\- ,]+'
SpeakerTitlePattern = r'( \([A-Za-zÀàÄäÁáÈèËëÉéÌìÏïÍíÒòÖöÓóÙùÜüÚúÑñ\·\- ]*?\))?'
SpeakerPattern = SpeakerTreatmentPattern + SpeakerNamePattern + SpeakerTitlePattern + '\:'


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

    point_title = ''
    point_text = ''
    points = []
    for paragraph in paragraphs:
        if _is_title(paragraph):
            if point_title != '':
                points.append({'title': point_title, 'text': point_text})
                point_text = ''
            point_title = paragraph
        elif point_text == '':
            point_text = paragraph
        else:
            point_text += '\n' + paragraph

    points.append({'title': point_title, 'text': point_text})

    for point in points:
        point_text_lines = point['text'].split('\n')
        for point_text_line in point_text_lines:
            search = re.search(SpeakerPattern, point_text_line)
            if search:
                found = search.group()
                print('-', found)
                name_search = re.search(SpeakerTreatmentPattern + SpeakerNamePattern, found)
                name_found = name_search.group()
                if found.startswith(MaleTreatment):
                    print('\t', 'male')
                    print('\t', name_found[len(MaleTreatment):])
                else:
                    print('\t', 'female')
                    print('\t', name_found[len(FemaleTreatment):])

                title_treatment = found[len(name_found):-1]
                if title_treatment != '':
                    print('\t', title_treatment.strip('()'))

    return points


def parse_diary(text, source, legislature, session):
    text = text.replace('\f', '')

    data = {
        'presidency': _parse_presidency(text),
        'date': _parse_date(text),
        'source': source,
        'legislature': legislature,
        'session': session
    }

    session_text = _parse_session(text)

    return data, session_text


if __name__ == '__main__':
    with open('../.data/texts/full_dscd-11-002.txt', 'r', encoding="utf-8") as file:
        diary = file.read()
        print(parse_diary(diary, 'dscd', 11, 2))

