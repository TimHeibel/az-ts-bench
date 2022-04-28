import asyncio
import datetime as dt
from math import floor
import sys
import time
import csv
import json
import random as rd
import concurrent.futures
from multiprocessing import cpu_count

from azure.iot.device.aio import IoTHubDeviceClient
from azure.iot.device import Message


async def create_and_connect_clients(config, start, end, i):
    # Get Connection Strings
    ''' 
    Creates and connects clients,
    start and end are set beforehand, depending
    on the Devices per core and number of cores.
    Furthermore saves the clientId with the client, 
    to ensure no two deviceIDs can randomly send with
    the same timestamp.
    Every Process creates it's own clients, as they
    are not pickleable and cannot be passed to the 
    processes easily.
    '''
    connectionStringList = []
    deviceIdList = []
    with open('connectionStringList.csv', newline='') as f:
        count = 0
        for row in csv.reader(f):
            if count < start:
                count += 1
                continue
            if count > end:
                break
            connectionStringList.append(row[0])
            deviceIdList.append(row[0].split(';')[1].split('=')[1])
            count += 1
            if count == config['n_devices']:
                break
    connectionString_Device_List = zip(connectionStringList, deviceIdList)

    # Connect Clients
    clients = []
    for connectionString_Device in connectionString_Device_List:
        client = IoTHubDeviceClient.create_from_connection_string(
            connectionString_Device[0])
        await asyncio.sleep(1/rd.randint(200, 1000))
        await client.connect()
        clients.append((client, connectionString_Device[1]))

    print(f'Process {i}: Created and connected {len(clients)} clients')
    return clients


async def send_messages(config, client):
    '''
    Sends messages for each client, using the async 
    IoTHubDeviceClient library
    '''
    # Send Messages
    sent_messages = 0
    start_client = time.time()
    start = time.time()
    messages_per_100_seconds = []
    while sent_messages < config['n_messages_per_device']:
        if sent_messages % 100 == 0 and sent_messages != 0:
            messages_per_100_seconds.append(100/(time.time() - start))
            start = time.time()
        msg_txt_formatted = '{{"time":"{timestamp}", "value": {value}, "deviceId": "{deviceId}"}}'.format(
            timestamp=dt.datetime.now().strftime('%Y-%m-%d %H:%M:%S.%f'), value=round(rd.random()*25, 4), deviceId=client[1])
        message = Message(msg_txt_formatted)
        await client[0].send_message(message)
        sent_messages += 1
        # if 'n_messages_per_sec_per_device' is set to 0, don't sleep
        if config['n_messages_per_sec_per_device'] > 0:
            await asyncio.sleep(1/config['n_messages_per_sec_per_device'])
    return messages_per_100_seconds


async def disconnect_clients(clients):
    for client in clients:
        await client[0].shutdown()
    print(f'Shut down {len(clients)} clients')


async def run_process(config, start, end, i):
    # Create Clients
    clients = await create_and_connect_clients(config, start, end, i)

    # Start sending Messages for Clients
    start_send_messages = dt.datetime.now()
    await asyncio.gather(*[send_messages(config, client) for client in clients])
    end_send_message = dt.datetime.now()

    # When all Clients have sent their messages, disconnect the clients
    await disconnect_clients(clients)
    return (start_send_messages, end_send_message)


def start_run_process(config, start_index, end, i):
    '''
    Each process start a coroutine (run_process), that in turn gathers many coroutines,
    that send the messages (send_messages) for the clients
    '''
    # https://github.com/encode/httpx/issues/914#issuecomment-622586610
    if sys.version_info[0] == 3 and sys.version_info[1] >= 8 and sys.platform.startswith('win'):
        asyncio.set_event_loop_policy(asyncio.WindowsSelectorEventLoopPolicy())

    print(f"Process {i} starting...")
    start = time.time()
    start_and_end_send_message_tupel = asyncio.run(
        run_process(config, start_index, end, i))
    print(
        f"Process {i} finished. Process Run Duration: {round(time.time() - start, 2)} seconds.")
    return start_and_end_send_message_tupel


if __name__ == "__main__":
    with open('config.json') as json_file:
        config = json.load(json_file)

    if config['n_cores'] > cpu_count() or config['n_cores'] <= 0:
        NUM_CORES = cpu_count()
    else:
        NUM_CORES = config['n_cores']

    NUM_DEVICES = config['n_devices']

    DEVICES_PER_CORE = floor(NUM_DEVICES/NUM_CORES)

    futures = []

    with concurrent.futures.ProcessPoolExecutor(NUM_CORES) as executor:
        for i in range(NUM_CORES - 1):
            futures.append(executor.submit(start_run_process, config=config,
                           start_index=DEVICES_PER_CORE*i, end=DEVICES_PER_CORE*(i+1)-1, i=i))

        futures.append(executor.submit(start_run_process, config=config,
                       start_index=DEVICES_PER_CORE*(NUM_CORES-1), end=NUM_DEVICES, i=NUM_CORES-1))

    concurrent.futures.wait(futures)

    merged_list = [v for f in futures for v in f.result()]
    print(*merged_list)
    earliest_start = min(merged_list)
    latest_end = max(merged_list)
    print("Ealiest start: ", earliest_start.strftime("%H:%M:%S.%f"))
    print("Latest End: ", latest_end.strftime("%H:%M:%S.%f"))
    print(f"Total Duration: {latest_end-earliest_start}")
