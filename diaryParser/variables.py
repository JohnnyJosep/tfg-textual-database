# Regex cleaning

# Agenda = '(ORDEN DEL DÍA:|Preguntas)\n?(.+\n)*?(.*\.\.+\s\d+)'
# LastDigit = '\d+$'
# OpenSession = r'Se (abre|reanuda) la sesión a (.+?) (de la mañana|del mediodía|de la tarde|de la noche)\.\n'
# EndSession = r'(Eran las|Era la) (.+?) (de la mañana|del mediodía|de la tarde|de la noche)\.\s*\n'
#
# WeekDay = '(lunes|martes|miércoles|jueves|viernes|sábado|domingo)'
# Date = r'\d{1,2} de (enero|febrero|mayo|abril|marzo|junio|julio|agosto|septiembre|setiembre|octubre|noviembre|diciembre) de \d{4}'
# PagHeader = 'Núm. \d+ ' + Date + r' Pág. \d+'
# Header = r'(.*?)DIARIO DE SESIONES DEL CONGRESO DE LOS DIPUTADOS\nPLENO Y DIPUTACIÓN PERMANENTE\n' + PagHeader
#
# Celebrated = 'celebrada el ' + WeekDay + ' ' + Date
# Legislature = r'[A-Z]+ LEGISLATURA'
# PresidencyPattern = 'PRESIDENCIA ((DEL EXCMO\. SR\. D\.)|(DE LA EXCMA\. SRA\. D\.\ª)) [A-Za-zÀàÄäÁáÈèËëÉéÌìÏïÍíÒòÖöÓóÙùÜüÚúÑñ\·\- ]+'
#
# DossierNumber = r'\(Número\s+de\s+(E|e)xpediente\s+(.*?)\)\.?'
#
# SpeackerTitlePattern = r'[A-ZÀÄÁÈËÉÌÏÍÒÖÓÙÜÚÑ\·\- ,]+'
# SpeackerNamePattern = r' \([A-Za-zÀàÄäÁáÈèËëÉéÌìÏïÍíÒòÖöÓóÙùÜüÚúÑñ\·\- ]*?\)'
# SpeackerTreatmentPattern = r'(La señora |El señor )'
# SpeackerPattern = SpeackerTreatmentPattern + '((' + SpeackerTitlePattern + SpeackerNamePattern + r')|(' + SpeackerTitlePattern + ')):'
# SpeachPattern = SpeackerPattern + '(.*?)((' + SpeackerPattern + ')|$)'