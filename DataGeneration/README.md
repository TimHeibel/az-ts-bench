# General
The DataGeneration folder includes scripts to send messages to an IoT Hub.
In the first step, devices are created on the IoT Hub and connection strings for the devices are saved in a csv file. In the second step, the script simulates these devices and sends messages to the IoT Hub.

# Prerequisites
An IoT Hub needs to have been created beforehand, as devices are registered on it. For setting up the IoT Hub have a look [here](../Setup/IoTHub/README.md)


# Create Devices
1. Set the _n_devices_to_create_, _basename_, _iot_hub_hostname_,  _iot_hub_owner_connection_string_ parameters in ```config.json``` 
2. Run the ```CreateDevices.py``` script

# Send Messages to the IoT Hub
1. After the devices have been created and the ```connectionStringList.csv``` has been populated with the count of connection strings as specified via the _n_devices_to_create_ parameter. The remaining parameters in ```config.json``` need to be set. 
    - if the _n_messages_per_sec_per_device_ is set to 0, the scripts sends at maximum speed
    - if _n_cores_ is less or equal to 0, all available cores are used.
2. The IoT Telemetry Simulator can be started. Run the ```IotTelemetrySimulator.py``` script

# Misc
- The script was used to send up to an average of 35.000 Messages per second using a VM with 64 vcpus and 256 GiB memory (Azure Standard D64as v5). 
- Python multiprocessing currently has a limit of 61 Cores used
- It was found to be best to use 50 of the 61 cores on this specific machine