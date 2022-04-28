import json
import random
import string
from azure.iot.hub import IoTHubRegistryManager
import csv


# Load config
with open('config.json') as json_file:
    config = json.load(json_file)

# Create IoTHubRegistryManager
iothub_registry_manager = IoTHubRegistryManager(
    config["iot_hub_owner_connection_string"])

connectionStringList = []
current_device_number = 1

for _ in range(config['n_devices_to_create']):
    try:
        device_id = '{basename}{number}'.format(
            basename=config["device_basename"], number=f'{current_device_number:06}')
        print('registering device %s' % device_id)

        # Create a device
        primary_key = ''.join(random.choice(
            string.ascii_uppercase + string.digits) for _ in range(44))
        secondary_key = ''.join(random.choice(
            string.ascii_uppercase + string.digits) for _ in range(44))
        device_state = "enabled"
        new_device = iothub_registry_manager.create_device_with_sas(
            device_id, primary_key, secondary_key, device_state
        )
        device_connection_string = "HostName={iot_hub_hostname};DeviceId={device_id};SharedAccessKey={device_primary_key}".format(
            iot_hub_hostname=config["iot_hub_hostname"],
            device_id=device_id,
            device_primary_key=primary_key)
        current_device_number += 1
        connectionStringList.append(device_connection_string)

    except Exception as ex:
        print(ex)

with open('connectionStringList.csv', 'w') as result_file:
    wr = csv.writer(result_file, delimiter='\n')
    wr.writerow(connectionStringList)

print(
    f"Created {len(connectionStringList)} devices and saved connection strings to csv file")
