import json
import sys
from transliterate import translit
from collections import OrderedDict

fileName = sys.argv[1]
events = None


with open(fileName, 'r', encoding='utf8') as eventsFile:
	with open('export/' + fileName.split('.')[0] + '.local.json', 'w', encoding='utf8') as localFile:

########################################
		i = 0
		def processEvent(event):
			global i
			title = ''
			event['Id'] = fileName.split('.')[0] + '.' + str(i)

			if 'Title' in event:
				title = event['Title']
			description = event['Description']

			options = []
			optionKeys = []
			j = 0
			if 'Options' in event:
				for option in event['Options']:
					options.append(option['Title'])
					option['Title'] = '$$' + event['Id'] + '.' + str(j)
					optionKeys.append(option['Title'])
					j += 1

			i += 1
			event.move_to_end('Id', last=False)

			event['Title'] = '$$' + event['Id'] + '.title'
			event['Description'] = '$$' + event['Id'] + '.description'

			localized[event['Title']] = title
			localized[event['Description']] = description
			
			for h in range(0, len(options)):
				localized[optionKeys[h]] = options[h]

			if 'TLDR' in event:
				for nestedEvent in event['TLDR']:
					processEvent(nestedEvent)

################################################

		events = json.loads(eventsFile.read(), object_pairs_hook=OrderedDict)
		
		localized = {}

		for event in events:
			processEvent(event)

		json.dump(localized, localFile, ensure_ascii=False, indent=4)

with open('export/local.'+fileName, 'w', encoding='utf8') as localEventsFile:
	json.dump(events, localEventsFile, ensure_ascii=False,indent=4)
