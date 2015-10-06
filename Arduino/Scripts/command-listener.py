#!/usr/bin/env python

import sys
import json
import urllib2
import logging
import logging.handlers
import traceback
import ConfigParser

from azure.servicebus import ServiceBusService, Rule
from urllib2 import URLError, HTTPError

LOG_FILENAME = './command-listener.log'

# Set up a specific logger with our desired output level
logger = logging.getLogger('command-listener')
logger.setLevel(logging.DEBUG)

formatter = logging.Formatter('%(asctime)s - %(levelname)s - %(message)s')

# Add the log message handler to the logger
handler = logging.handlers.TimedRotatingFileHandler(
              LOG_FILENAME, when='midnight', backupCount=5, utc=True)
handler.setFormatter(formatter)
logger.addHandler(handler)
logger.info("Starting commands listener.")

config = ConfigParser.SafeConfigParser()
config.read('/root/config.ini')

namespace = config.get('ServiceBus', 'Namespace')
key_name = config.get('ServiceBus', 'KeyName')
key_value = config.get('ServiceBus', 'KeyValue')
arduino_username = config.get('Arduino', 'Username')
arduino_password = config.get('Arduino', 'Password')

pwd_mgr = urllib2.HTTPPasswordMgrWithDefaultRealm()
pwd_mgr.add_password("arduino", "http://localhost/mailbox/", arduino_username, arduino_password)
handler = urllib2.HTTPBasicAuthHandler(pwd_mgr)
opener = urllib2.build_opener(handler)
urllib2.install_opener(opener)

try:
	sbs = ServiceBusService(namespace,
	                        shared_access_key_name=key_name,
	                        shared_access_key_value=key_value)

	sbs.create_subscription('commands', 'arduino')

	while True:
		msg = sbs.receive_subscription_message('commands', 'arduino')

		if msg.body:
			logger.info('Received ' + msg.body)

			cmd = json.loads(msg.body)		
			logger.debug(cmd)

			path = str(cmd["Red"]) + "," + str(cmd["Green"]) + "," + str(cmd["Blue"])

			urllib2.urlopen('http://localhost/mailbox/' + path)

			msg.delete()

except Exception as e:
	logger.error('Error: ' + traceback.format_exc())
